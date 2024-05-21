using LOGExtractor.Gba;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer
{
    internal static class MapRenderer
    {
        private const int tile_size = 8;
        private const int chunk_size_tiles = 32;
        private const int chunk_size_pixels = tile_size * chunk_size_tiles;

        private static readonly Dictionary<int, Bitmap> tilesetCache = [];
        private static readonly Dictionary<int, Bitmap> layerCache = [];

        public static void ResetCache()
        {
            tilesetCache.Clear();
            layerCache.Clear();
        }

        public static Dictionary<Range, Bitmap> DrawTileset(ROM rom, int mapOffset)
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

        public static Bitmap GetTilesetImage(ROM rom, int tileId, Color[] palette)
        {
            const int start_addr = 0x179764;
            const int tile_per_image = 16 * 16;
            //const int num_images = 0xBB;

            int tilesetId = tileId / tile_per_image;

            if (tilesetCache.TryGetValue(tilesetId, out var bitmap))
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
            tilesetCache.Add(tilesetId, bitmap);
            return bitmap;
        }

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

        public static Bitmap DrawLayer(ROM rom, int address, Dictionary<Range, Bitmap> tilesets)
        {
            if (layerCache.TryGetValue(address, out var layerImage))
            {
                return layerImage;
            }

            if (address == 0x0707FC)
            {
                // do nothing function?
                return new Bitmap(1, 1);
            }

            rom.PushPosition(address);
            int layerType = rom.ReadPointer();

            if (layerType == 0x897B)
            {
                // image
                rom.Seek(address + 0x1C);
                var chunk = DrawChunk(rom, rom.ReadPointer(), tilesets);
                rom.PopPosition();
                return chunk;
            }

            if (layerType != 0x8087)
            {
                // IDK
                Debug.WriteLine($"unsupported layer type: {layerType:X4} @ 0x{address:X6}");
                rom.PopPosition();
                return new Bitmap(1, 1);
            }

            rom.Skip(0x9);
            int offX = rom.ReadShort() / 4;
            rom.Skip(0x2);
            int offY = rom.ReadShort() / 4;
            rom.Skip(0x1);
            int numCols = rom.ReadByte();
            int numRows = rom.ReadByte();

            // this is wrong
            if (offX >= 0x3F00)
            {
                offX -= 0x3F00;
                offX *= -1;
            }
            if (offY >= 0x3F00)
            {
                offY -= 0x3F00;
                offY *= -1;
            }

            layerImage = new Bitmap((numCols * chunk_size_pixels) - offX, (numRows * chunk_size_pixels) - offY);
            using var g = Graphics.FromImage(layerImage);
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

            layerCache.Add(address, layerImage);
            rom.PopPosition();
            return layerImage;
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
