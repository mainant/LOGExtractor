using LOGExtractor.Gba;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace MapViewer
{
    public partial class MainForm : Form
    {
        private record MapItem(string DisplayName, int Offset);
        private record MapConnector(int Left, int Top, int Right, int Bottom, int DestinationId);

        private ROM? rom;
        private MapItem? selectedMapItem;

        public MainForm()
        {
            InitializeComponent();
            AllowDrop = true;
            UxEventsToggle.Visible = false;
            UxMapList.Enabled = false;

            UxMapCanvas.Paint += UxMapCanvas_Paint;
            UxMapCanvas.ZoomChanged += UxMapCanvas_ZoomChanged;

            UxZoomOption.Items.Clear();
            foreach (var mode in Canvas.ZoomModes)
            {
                UxZoomOption.Items.Add($"{(mode * 100f)}%");
            }
            UxZoomOption.SelectedIndex = UxMapCanvas.ZoomMode;
        }

        private void UxMapCanvas_ZoomChanged(int newZoom)
        {
            UxZoomOption.SelectedIndex = newZoom;
        }

        private void UxMapCanvas_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            if (rom == null) return;
            if (selectedMapItem == null) return;

            try
            {
                rom.Seek(selectedMapItem.Offset + 0x2C);
                rom.Seek(rom.ReadPointer());

                int mapOffset = rom.ReadPointer();

                // map tileset
                var tilesets = MapRenderer.DrawTileset(rom, mapOffset);

                // layers
                rom.Seek(mapOffset + 0x14);
                var layer0 = MapRenderer.DrawLayer(rom, rom.ReadPointer(), tilesets);
                var layer1 = MapRenderer.DrawLayer(rom, rom.ReadPointer(), tilesets);
                var layer2 = MapRenderer.DrawLayer(rom, rom.ReadPointer(), tilesets);
                var layer3 = MapRenderer.DrawLayer(rom, rom.ReadPointer(), tilesets);

                // connectors
                rom.Seek(mapOffset + 0x40);
                var connectors = GetConnectors(rom);

                // map size
                rom.Seek(mapOffset + 0x8);
                int width = rom.ReadShort() + 240 - 1;  // gba viewport width
                int height = rom.ReadShort() + 160 - 1; // gba viewport height

                if (UxBG3Toggle.Checked)
                {
                    g.DrawImage(layer3, 0, 0);
                }
                if (UxBG2Toggle.Checked)
                {
                    g.DrawImage(layer2, 0, 0);
                }
                if (UxBG1Toggle.Checked)
                {
                    g.DrawImage(layer1, 0, 0);
                }
                if (UxBG0Toggle.Checked)
                {
                    g.DrawImage(layer0, 0, 0);
                }

                if (UxConnectorsToggle.Checked)
                {
                    foreach (var connector in connectors)
                    {
                        // +8 to rectangle X seems right
                        var rect = Rectangle.FromLTRB(connector.Left, connector.Top, connector.Right, connector.Bottom);
                        rect.X += 8;
                        g.FillRectangle(Brushes.Red, rect);
                        g.DrawString($"{connector.DestinationId:X4}", Font, Brushes.White, rect.X, rect.Y);
                    }
                }

                g.DrawRectangle(new Pen(Color.Green, 2), 0, 0, width, height);
            }
            catch (Exception ex)
            {
                g.Clear(Color.Black);
                g.DrawString($"Could not render map:\r\n{selectedMapItem}\r\n{ex}", Font, Brushes.DarkRed, 5, 5);
                Debug.WriteLine(selectedMapItem);
            }
        }

        private static List<MapConnector> GetConnectors(ROM rom)
        {
            int count = rom.ReadInt();
            if (count == 0)
            {
                return [];
            }

            var list = new List<MapConnector>();

            rom.Seek(rom.ReadPointer());
            for (int i = 0; i < count; i++)
            {
                rom.PushPosition(rom.ReadPointer());
                rom.Skip(0x8);
                int left = rom.ReadShort();
                int top = rom.ReadShort();
                int right = rom.ReadShort();
                int bottom = rom.ReadShort();
                rom.Skip(0x1);
                int destId = rom.ReadShortBigEndian();
                rom.PopPosition();

                list.Add(new(left, top, right, bottom, destId));
            }

            return list;
        }

        private void UxMapList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UxMapList.SelectedItem is MapItem map)
            {
                if (map == selectedMapItem)
                {
                    return;
                }
                UxMapOffsetLabel.Text = $"Selected Map Offset: 0x{(map.Offset | 0x08000000):X}";
                selectedMapItem = map;
            }
            else
            {
                UxMapOffsetLabel.Text = "Selected Map Offset: 0x0";
                selectedMapItem = null;
            }

            UxMapCanvas.Invalidate();
        }

        private void ReadMapsFromROM(string path)
        {
            rom = null;
            selectedMapItem = null;
            UxMapCanvas.Invalidate();

            MapRenderer.ResetCache();
            UxMapList.Items.Clear();
            UxMapList.Enabled = false;

            try
            {
                const int map_count = 0x1c4;

                rom = ROM.FromFile(path);
                int currentAddr = 0x08e2e0;

                var itemsList = new List<MapItem>();

                for (int i = 0; i < map_count; i++)
                {
                    rom.Seek(currentAddr);
                    int mapId = rom.ReadShortBigEndian();
                    rom.Skip(0xA);
                    int mapNameId = rom.ReadShort();
                    string mapName = string.Empty;
                    if (mapNameId != 0)
                    {
                        rom.PushPosition(0x06bce8 + (mapNameId * 4));
                        rom.Seek(rom.ReadPointer());
                        mapName = rom.ReadUnicodeString();
                        rom.PopPosition();
                    }
                    itemsList.Add(new MapItem($"{mapId:X4}: {mapName}", currentAddr));

                    currentAddr += 0x38;
                }

                UxMapList.Items.AddRange(itemsList.ToArray());
            }
            catch { }

            UxTotalMapsLabel.Text = $"Total Maps: {UxMapList.Items.Count}";
            UxMapList.DisplayMember = "DisplayName";
            UxMapList.Enabled = UxMapList.Items.Count > 0;
        }

        private void UxToggle_CheckChanged(object sender, EventArgs e)
        {
            UxMapCanvas.Invalidate();
        }

        private void UxZoomOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            UxMapCanvas.ZoomMode = UxZoomOption.SelectedIndex;
        }

        private void UxResetView_Click(object sender, EventArgs e)
        {
            UxMapCanvas.ResetView();
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data != null && drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                drgevent.Effect = DragDropEffects.Copy;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (drgevent.Data != null && drgevent.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                ReadMapsFromROM(files[0]);
            }
        }
    }
}
