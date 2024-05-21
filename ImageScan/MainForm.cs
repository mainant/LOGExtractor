using LOGExtractor.Gba;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ImageScan
{
    public partial class MainForm : Form
    {

        ROM rom;
        Color[] bgPalette;
        Color[] objPalette;
        int currentAddress;
        Bitmap canvas;
        int prioResults;
        List<ScanResult> scanResults;
        readonly Dictionary<int, byte[]> cache = new();

        private int zoomFactor = 1;
        private Point? lastDragPosition;
        private Point offset = Point.Empty;

        private int subImageWidth;
        private int subImageHeight;

        class ScanResult
        {
            public ScanResult(int address, int priority)
            {
                Address = address;
                Priority = priority;
            }

            public int Address { get; }
            public int Priority { get; }
        }

        public MainForm()
        {
            InitializeComponent();

            uxImage.Paint += UxImage_Paint;
            uxImage.MouseClick += UxImage_MouseClick;
            uxImage.MouseWheel += UxImage_MouseWheel;
            uxImage.MouseMove += UxImage_MouseMove;
        }

        private void UxImage_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Middle) != 0)
            {
                if (lastDragPosition != null)
                {
                    int x = lastDragPosition.Value.X - e.X;
                    int y = lastDragPosition.Value.Y - e.Y;

                    offset = new Point(offset.X - x, offset.Y - y);

                    uxImage.Invalidate();
                }

                lastDragPosition = e.Location;
            }
            else
            {
                lastDragPosition = null;
            }
        }

        private void UxImage_MouseWheel(object sender, MouseEventArgs e)
        {
            int oldZoom = zoomFactor;

            zoomFactor = Math.Clamp(zoomFactor + Math.Sign(e.Delta), 1, 10);

            if (oldZoom != zoomFactor)
            {
                uxImage.Invalidate();
            }
        }

        private void UxImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                offset = Point.Empty;
                zoomFactor = 1;
                uxImage.Invalidate();
            }
            else if (e.Button == MouseButtons.Left && UxEnableTileIndexOption.Checked)
            {
                var bounds = new Rectangle(offset.X, offset.Y, subImageWidth * zoomFactor, subImageHeight * zoomFactor);
                if (bounds.Contains(e.Location))
                {
                    int tileSize = (int)uxTileSize.Value * zoomFactor;
                    int tx = (e.X - offset.X) / tileSize;
                    int ty = (e.Y - offset.Y) / tileSize;

                    int oldIndex = (int)uxTileIndex.Value;
                    int newIndex = (ty * (int)uxImageWidth.Value) + tx;

                    if (oldIndex != newIndex)
                    {
                        uxTileIndex.Value = newIndex;
                        uxImage.Invalidate();
                    }
                }
            }
        }

        private void UxImage_Paint(object sender, PaintEventArgs e)
        {
            if (subImageWidth == 0 || subImageHeight == 0)
            {
                return;
            }

            var g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            int x = offset.X;
            int y = offset.Y;
            int width = canvas.Width * zoomFactor;
            int height = canvas.Height * zoomFactor;

            g.DrawImage(canvas, x, y, width, height);
            g.DrawRectangle(Pens.Green, x, y, subImageWidth * zoomFactor + 1, subImageHeight * zoomFactor + 1);

            if (UxEnableTileIndexOption.Checked)
            {
                int tilesize = (int)uxTileSize.Value;
                int tilesPerRow = (int)uxImageWidth.Value;

                int tile = (int)uxTileIndex.Value;
                int tx = (tile % tilesPerRow) * tilesize;
                int ty = (tile / tilesPerRow) * tilesize;
                var pen = new Pen(Color.Magenta, 3);
                g.DrawRectangle(pen, offset.X + tx * zoomFactor + 1, offset.Y + ty * zoomFactor + 1, tilesize * zoomFactor - 1, tilesize * zoomFactor - 1);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            rom = ROM.FromFile(Path.Combine(Environment.CurrentDirectory, "buus.gba"));
            //if (!rom.GameCode.Equals("BG3E"))
            //    throw new Exception("Invalid rom");

            string palDir = Path.Combine(Environment.CurrentDirectory, "palettes");

            bgPalette = Palette.FromFile(Path.Combine(palDir, "buus_bg_pal.act"));
            objPalette = Palette.FromFile(Path.Combine(palDir, "buus_obj_pal.act"));
            canvas = new Bitmap(256, 256);
            Redraw();
            uxImageWidth.Minimum = 1;
            uxImageWidth.Maximum = 256 / 8;
            uxCurrentResult.Enabled = false;

            uxImage.BackColor = uxBackColor.BackColor;
        }

        void ScanForImages()
        {
            scanResults = new List<ScanResult>();
            prioResults = 0;

            for (int i = 0x200; i < rom.Length - 3; i += 1)
            {
                rom.Seek(i);
                int magic = rom.ReadInt();
                if (magic != 0x1) continue;
                int length = rom.ReadInt();
                if (length > 0 && length < 0x20000 && length % 4 == 0)
                {
                    try
                    {
                        byte[] decompressed = JCALG1.Decompress(rom, i);
                        if (decompressed.Length > 0)
                        {
                            bool isPrio = decompressed.Length == 16 * 16;
                            scanResults.Add(new ScanResult(i, isPrio ? 1 : 9));
                            cache.TryAdd(i, decompressed);
                            if (isPrio)
                            {
                                prioResults++;
                            }
                        }
                    }
                    catch { }
                }
            }

            scanResults.Sort((x, y) =>
            {
                int prio = x.Priority - y.Priority;
                if (prio != 0) return prio;
                return x.Address - y.Address;
            });
        }

        void Redraw()
        {
            using var g = Graphics.FromImage(canvas);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.SmoothingMode = SmoothingMode.None;
            g.Clear(uxBackColor.BackColor);

            if (currentAddress > 0)
            {
                if (!cache.TryGetValue(currentAddress, out var data))
                {
                    try
                    {
                        byte[] decompressed = JCALG1.Decompress(rom, currentAddress);
                        if (decompressed.Length > 0)
                        {
                            cache.TryAdd(currentAddress, decompressed);
                            data = decompressed;
                        }
                    }
                    catch { }
                }

                //Debug.WriteLine($"drawing image from 0x{currentAddress:X} with {data?.Length} bytes");

                if (data != null)
                {
                    int tilesize = (int)uxTileSize.Value;
                    int tilesPerRow = (int)uxImageWidth.Value;
                    var pal = uxOBJPalette.Checked ? objPalette : bgPalette;
                    var image = CreateBitmap(data,
                                                pal,
                                                tilesPerRow * tilesize,
                                                canvas.Height,
                                                tilesize, (tilesize < 160) ? tilesize : 160, out int usedHeight);
                    int width = image.Width;
                    int height = image.Height;

                    subImageWidth = width;
                    subImageHeight = usedHeight;

                    g.DrawImage(image, 0, 0, width, height);

                    uxSize.Text = data.Length.ToString();

                    try
                    {
                        uxText.Clear();
                        if (uxTextFormat.Checked)
                        {
                            var sb = new StringBuilder();
                            string byteString = BitConverter.ToString(data).Replace("-", " ");
                            int pos = 0;
                            while (true)
                            {
                                int length = Math.Min(byteString.Length - pos, 48);
                                if (length == 0) break;
                                sb.AppendLine(byteString.Substring(pos, length).TrimEnd());
                                pos += length;
                            }
                            uxText.Text = sb.ToString();
                        }
                        else
                        {
                            uxText.Text = Encoding.Unicode.GetString(data);
                        }
                    }
                    catch { }
                }
                else
                {
                    subImageWidth = 0;
                    subImageHeight = 0;
                }
            }

            uxImage.Invalidate();
        }

        public static Bitmap CreateBitmap(byte[] data, Color[] pal, int width, int maxHeight, int tileWidth, int tileHeight, out int usedHeight)
        {
            var bitmap = new Bitmap(width, maxHeight);
            int r = 0;
            int dx = 0;
            int dy = 0;
            int step = tileWidth * tileHeight;
            int rows = 1;

            while (r + step <= data.Length)
            {
                for (int x = 0; x < tileWidth; x++)
                {
                    for (int y = 0; y < tileHeight; y++)
                    {
                        int index = y * tileWidth + x;
                        bitmap.SetPixel(dx + x, dy + y, pal[data[r + index]]);
                    }
                }
                r += step;
                dx += tileWidth;
                if (dx + tileWidth > width)
                {
                    dx = 0;
                    dy += tileHeight;
                    rows++;
                    if (dy + tileHeight > maxHeight) break;
                }
            }
            if (dx == 0)
            {
                rows--;
            }
            usedHeight = rows * tileHeight;
            return bitmap;
        }

        void UpdateAddress()
        {
            string text = uxAddress.Text.Trim().ToUpper();
            var style = NumberStyles.Integer;
            if (text.StartsWith("0X"))
            {
                style = NumberStyles.HexNumber;
                text = text[2..];
            }
            else if ((text.IndexOfAny(new[] { 'A', 'B', 'C', 'D', 'E', 'F' }) > -1) || text.StartsWith("08"))
            {
                style = NumberStyles.HexNumber;
            }
            if (int.TryParse(text, style, null, out int result))
            {
                if ((result & 0x8000000) != 0)
                {
                    result -= 0x8000000;
                }
                if (result > 0 && result < rom.Length)
                {
                    currentAddress = result;
                    Redraw();
                }
            }
            uxAddress.Text = $"0x{currentAddress:X}";
        }

        private void OnBackColorMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                using var cd = new ColorDialog()
                {
                    Color = uxBackColor.BackColor,
                    AnyColor = true,
                    SolidColorOnly = true,
                    FullOpen = true,
                    ShowHelp = false,
                    CustomColors = new[] { ColorTranslator.ToOle(Color.Black), ColorTranslator.ToOle(Color.Magenta) }
                };
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    uxBackColor.BackColor = cd.Color;
                    uxImage.BackColor = cd.Color;
                    Redraw();
                }
            }
        }

        private void OnAddressKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateAddress();
            }
        }

        private void OnAddressLeave(object sender, EventArgs e)
        {
            UpdateAddress();
        }

        private void OnImageWidthValueChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void OnCurrentResultValueChanged(object sender, EventArgs e)
        {
            if (uxCurrentResult.Value > 0)
            {
                uxAddress.Text = $"0x{scanResults[(int)uxCurrentResult.Value - 1].Address:X}";
                UpdateAddress();
                Redraw();
            }
        }

        private void OnPaletteCheckedChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void OnZoomCheckedChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void OnTextFormatCheckedChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void OnTileSizeValueChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void OnScanNowClick(object sender, EventArgs e)
        {
            uxScanNow.Enabled = false;
            uxCurrentResult.Enabled = false;
            uxResultsFound.Text = "Scanning...";
            uxResultsPrio.Text = "0";
            Task.Run(() =>
            {
                Thread.Sleep(1);
                ScanForImages();

                Invoke(new Action(() =>
                {
                    uxResultsFound.Text = scanResults.Count.ToString();
                    uxCurrentResult.Maximum = scanResults.Count;
                    uxResultsPrio.Text = prioResults.ToString();
                    uxCurrentResult.Enabled = true;
                    uxScanNow.Enabled = true;
                }));
            }).ConfigureAwait(false);
        }

        private void uxTileIndex_ValueChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!cache.TryGetValue(currentAddress, out var data))
            {
                return;
            }

            int tilesize = (int)uxTileSize.Value;
            int tilesPerRow = (int)uxImageWidth.Value;
            var pal = uxOBJPalette.Checked ? objPalette : bgPalette;
            int width = tilesPerRow * tilesize;
            int height = data.Length / width;
            var image = CreateBitmap(data,
                                        pal,
                                        width,
                                        height,
                                        tilesize, (tilesize < 160) ? tilesize : 160, out int usedHeight);
            image.Save(Path.Combine(Environment.CurrentDirectory, $"{currentAddress}.png"));
        }

        private void UxEnableTileIndexOption_CheckedChanged(object sender, EventArgs e)
        {
            uxTileIndex.Enabled = UxEnableTileIndexOption.Checked;
            uxImage.Invalidate();
        }
    }
}
