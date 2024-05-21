using LOGExtractor.Gba;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGExtractor
{
    internal static class Program
    {

        const int tile_size = 8;
        const int chunk_size_tiles = 32;
        const int chunk_size_pixels = tile_size * chunk_size_tiles;

        private static readonly Dictionary<int, Bitmap> imageCache = [];

        static void Main(string[] args)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "buus.gba");
            var rom = ROM.FromFile(path);

            int mapOffset = 0x29139c;

            string mapDir = Path.Combine(Environment.CurrentDirectory, "buu_map_test", $"0x08{mapOffset:X6}");

            Directory.CreateDirectory(mapDir);

            // map tileset
            var tilesets = DrawTileset(rom, mapOffset);
            foreach (var tileset in tilesets)
            {
                tileset.Value.Save(Path.Combine(mapDir, $"tileset_{tileset.Key}.png"));
            }

            // layers
            rom.Seek(mapOffset + 0x14);
            var layer0 = DrawLayer(rom, rom.ReadPointer(), tilesets);
            var layer1 = DrawLayer(rom, rom.ReadPointer(), tilesets);
            var layer2 = DrawLayer(rom, rom.ReadPointer(), tilesets);
            var layer3 = DrawLayer(rom, rom.ReadPointer(), tilesets);

            // connectors
            rom.Seek(mapOffset + 0x40);
            var connectors = GetConnectors(rom);

            // map size
            rom.Seek(mapOffset + 0x8);
            int width = rom.ReadShort() + 240 - 1;  // gba viewport width
            int height = rom.ReadShort() + 160 - 1; // gba viewport height

            var map_image = new Bitmap(width, height);
            using var g = Graphics.FromImage(map_image);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            g.DrawImage(layer3, 0, 0);
            g.DrawImage(layer2, 0, 0);
            g.DrawImage(layer1, 0, 0);
            g.DrawImage(layer0, 0, 0);

            for (int i = 0; i < connectors.Count; i++)
            {
                g.FillRectangle(Brushes.Red, connectors[i]);
            }

            layer0.Save(Path.Combine(mapDir, "layer0.png"));
            layer1.Save(Path.Combine(mapDir, "layer1.png"));
            layer2.Save(Path.Combine(mapDir, "layer2.png"));
            layer3.Save(Path.Combine(mapDir, "layer3.png"));
            map_image.Save(Path.Combine(mapDir, "final_map.png"));
        }

        private static List<Rectangle> GetConnectors(ROM rom)
        {
            int count = rom.ReadInt();
            if (count == 0)
            {
                return [];
            }

            var list = new List<Rectangle>();

            rom.Seek(rom.ReadPointer());
            for (int i = 0; i < count; i++)
            {
                rom.PushPosition(rom.ReadPointer());
                rom.Skip(0x8);
                int left = rom.ReadShort();
                int top = rom.ReadShort();
                int right = rom.ReadShort();
                int bottom = rom.ReadShort();
                rom.PopPosition();

                list.Add(Rectangle.FromLTRB(left, top, right, bottom));
            }

            return list;
        }

        private static Dictionary<Range, Bitmap> DrawTileset(ROM rom, int mapOffset)
        {
            var pal = ReadBGPalette(rom);

            // first we pull the animated tiles
            rom.PushPosition(mapOffset + 0xC);
            int numberOfSequences = rom.ReadInt();
            int sequencePtr = rom.ReadPointer();
            var tilesets = new Dictionary<Range, Bitmap>();

            if (numberOfSequences > 0 && sequencePtr != 0x0)
            {
                rom.Seek(sequencePtr);

                for (int i = 0; i < numberOfSequences; i++)
                {
                    rom.PushPosition(rom.ReadPointer());

                    int numberOfStrips = rom.ReadByte();
                    int numberOfFrames = rom.ReadByte();
                    int vramOffset = rom.ReadShort();

                    var sequenceImage = new Bitmap(numberOfFrames * 8, numberOfStrips * 8);

                    for (int j = 0; j < numberOfStrips; j++)
                    {
                        var imgData = JCALG1.DecompressUnknownHeader(rom, rom.ReadPointer());

                        for (int src = 0; src < imgData.Length; src += 64)
                        {
                            int dx = (src / 64) * 8;
                            int dy = j * 8;
                            for (int x = 0; x < 8; x++)
                            {
                                for (int y = 0; y < 8; y++)
                                {
                                    int index = (y * 8) + x;
                                    sequenceImage.SetPixel(dx + x, dy + y, pal[imgData[src + index]]);
                                }
                            }
                        }
                    }

                    tilesets.Add(new Range(vramOffset, vramOffset + numberOfFrames - 1), sequenceImage);

                    rom.PopPosition();
                }

            }

            rom.PopPosition();

            // create tileset
            rom.PushPosition(mapOffset + 0x48);
            byte[] tilesetBytes = JCALG1.Decompress(rom, rom.ReadPointer());

            int tsColumns = 16;
            int imageWidth = tsColumns * 8;
            int imageHeight = (int)(Math.Ceiling((tilesetBytes.Length / 2f) / tsColumns)) * 8;

            var tileset = new Bitmap(imageWidth, imageHeight);
            using var ts = Graphics.FromImage(tileset);
            ts.InterpolationMode = InterpolationMode.NearestNeighbor;
            ts.PixelOffsetMode = PixelOffsetMode.Half;

            int currentIndex = 0;
            for (int i = 0; i < tilesetBytes.Length; i += 2)
            {
                int offset = tilesetBytes[i] | (tilesetBytes[i + 1] << 8);
                currentIndex += offset;

                var src = GetTilesetImage(rom, currentIndex, pal);

                int l = currentIndex % 256;
                int srcX = (l % 16) * 8;
                int srcY = (l / 16) * 8;
                int destX = ((i / 2) % tsColumns) * 8;
                int destY = ((i / 2) / tsColumns) * 8;

                ts.DrawImage(src, destX, destY, new Rectangle(srcX, srcY, 8, 8), GraphicsUnit.Pixel);

                currentIndex++;
            }

            tilesets.Add(new Range(0, (tilesetBytes.Length / 2)), tileset);

            rom.PopPosition();
            return tilesets;
        }

        private static Graphics GraphicsForImage(Bitmap image)
        {
            var g = Graphics.FromImage(image);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            return g;
        }

        private static Bitmap GetTilesetImage(ROM rom, int tileId, Color[] palette)
        {
            const int start_addr = 0x179764;
            const int tile_per_image = 16 * 16;
            //const int num_images = 0xBB;

            int tilesetId = tileId / tile_per_image;

            if (imageCache.TryGetValue(tilesetId, out var bitmap))
            {
                return bitmap;
            }

            bitmap = new Bitmap(128, 128);

            rom.PushPosition(start_addr + (tilesetId * 4));
            var imgData = JCALG1.Decompress(rom, rom.ReadPointer());

            for (int src = 0; src < imgData.Length; src += 64)
            {
                int dx = (src % 1024) / 8;
                int dy = (src / 1024) * 8;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        int index = (y * 8) + x;
                        bitmap.SetPixel(dx + x, dy + y, palette[imgData[src + index]]);
                    }
                }
            }

            rom.PopPosition();
            imageCache.Add(tilesetId, bitmap);
            return bitmap;
        }

        /*
        private static Bitmap DrawBigTileset(ROM rom)
        {
            const int num_images = 0xBB;

            var pal = ReadBGPalette(rom);

            var bigTileset = new Bitmap(128, num_images * 128);
            using var g = Graphics.FromImage(bigTileset);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            rom.PushPosition(0x179764);
            for (int i = 0; i < num_images; i++)
            {
                var imgData = JCALG1.Decompress(rom, rom.ReadPointer());

                for (int src = 0; src < imgData.Length; src += 64)
                {
                    int dx = (src % 1024) / 8;
                    int dy = (src / 1024) * 8;
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            int index = (y * 8) + x;
                            bigTileset.SetPixel(dx + x, dy + y + (i * 128), pal[imgData[src + index]]);
                        }
                    }
                }
            }

            rom.PopPosition();
            return bigTileset;
        }
        */

        private static Color[] ReadBGPalette(ROM rom)
        {
            const int bg_palette = 0x05632c;
            rom.PushPosition(bg_palette);
            var colors = new Color[256];

            for (int i = 0; i < colors.Length; i++)
            {
                int c = rom.ReadShort();
                int r = ((c & 0x1F) * 0x21) >> 2;
                int g = (((c >> 5) & 0x1F) * 0x21) >> 2;
                int b = (((c >> 10) & 0x1F) * 0x21) >> 2;

                colors[i] = Color.FromArgb(0xFF, r, g, b);
            }

            colors[0] = Color.FromArgb(0, 0, 0, 0);

            rom.PopPosition();
            return colors;
        }

        private static Bitmap DrawLayer(ROM rom, int address, Dictionary<Range, Bitmap> tilesets)
        {
            if (address == 0x0707FC)
            {
                // do nothing function?
                return new Bitmap(1, 1);
            }
            rom.PushPosition(address);

            rom.Skip(0xD);
            int offX = rom.ReadShort() / 4;
            rom.Skip(0x2);
            int offY = rom.ReadShort() / 4;
            rom.Skip(0x1);
            int numCols = rom.ReadByte();
            int numRows = rom.ReadByte();

            //offX = 0;
            //offY = 0;

            var layer_image = new Bitmap((numCols * chunk_size_pixels) - offX, (numRows * chunk_size_pixels) - offY);
            using var g = Graphics.FromImage(layer_image);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            rom.Skip(0x2);
            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < numCols; c++)
                {
                    var chunk = DrawChunk(rom, rom.ReadPointer(), tilesets);

                    g.DrawImage(chunk, (chunk_size_pixels * c) - offX, (chunk_size_pixels * r) - offY);
                }
            }

            rom.PopPosition();
            return layer_image;
        }

        private static Bitmap DrawChunk(ROM rom, int address, Dictionary<Range, Bitmap> tilesets)
        {
            var chunk_image = new Bitmap(chunk_size_pixels, chunk_size_pixels);

            if (address == 0x0)
            {
                return chunk_image;
            }

            using var g = Graphics.FromImage(chunk_image);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            byte[] bytes = JCALG1.Decompress(rom, address);
            int x = 0;
            int y = 0;

            for (int i = 0; i < bytes.Length; i += 2)
            {
                byte lsb = bytes[i];
                byte msb = bytes[i + 1];
                int entry = (msb << 8) + lsb;
                int tileId = entry & 0x3FF;
                bool flipX = (entry & (1 << 10)) != 0;
                bool flipY = (entry & (1 << 11)) != 0;

                var range = tilesets.Keys.Where(k => (tileId >= k.Start.Value && tileId <= k.End.Value)).First();
                var tileset = tilesets[range];
                int columns = tileset.Width / 8;

                int l = tileId - range.Start.Value;
                int srcX = (l % columns) * tile_size;
                int srcY = (l / columns) * tile_size;

                if (tileId != 0)
                {
                    if (flipX || flipY)
                    {
                        g.DrawImage(FlipTile(tileset, srcX, srcY, flipX, flipY), x, y);
                    }
                    else
                    {
                        g.DrawImage(tileset, x, y, new Rectangle(srcX, srcY, tile_size, tile_size), GraphicsUnit.Pixel);
                    }
                }

                x += tile_size;
                if (x >= chunk_size_pixels)
                {
                    x = 0;
                    y += tile_size;
                }
            }

            return chunk_image;
        }

        private static Bitmap FlipTile(Bitmap tileset, int srcX, int srcY, bool flipX, bool flipY)
        {
            var tile_image = new Bitmap(tile_size, tile_size);
            using var g = Graphics.FromImage(tile_image);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            g.DrawImage(tileset, 0, 0, new Rectangle(srcX, srcY, tile_size, tile_size), GraphicsUnit.Pixel);

            if (flipX && flipY)
            {
                tile_image.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            }
            else if (flipX)
            {
                tile_image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            else if (flipY)
            {
                tile_image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
            return tile_image;
        }

    }
}
