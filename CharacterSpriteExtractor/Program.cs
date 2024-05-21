using LOGExtractor.Gba;
using System;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CharacterSpriteExtractor
{
    internal static class Program
    {
        static void Main()
        {
            var rom = ROM.FromFile(Path.Combine(Environment.CurrentDirectory, "buus.gba"));
            var palette = Palette.FromFile(Path.Combine(Environment.CurrentDirectory, "palettes", "buus_obj_pal.act"));

            const int char_table_addr = 0x74d380;
            const int sprite_table_addr = 0x6b6bb4;
            const int portrait_table_addr = 0x6f393c;
            const int text_table_addr = 0x06bce8;

            const int char_count = 232;
            string[] anim_names = ["stand", "walk", "run", "attack1", "attack2", "attack3", "attack4", "block", "idle", "hurt", "transform", "unknown1", "arms_up", "pickup", "takeoff", "flying", "mode7_fly", "dead", "special", "teleport", "unknown4", "fusion", "unknown5"];

            string outRoot = Path.Combine(Environment.CurrentDirectory, "characters");

            for (int s = 0; s < char_count; s++)
            {
                int addr = char_table_addr + (s * 16);
                rom.Seek(addr);
                int nameIndex = rom.ReadShort();
                rom.Skip(0x2); // description
                int spriteIndex = rom.ReadInt();
                int portraitIndex = rom.ReadInt();

                rom.Seek(text_table_addr + (nameIndex * 4));
                rom.Seek(rom.ReadPointer());
                string name = rom.ReadUnicodeString();

                rom.Seek(portrait_table_addr + (portraitIndex * 4));
                int portraitPtr = rom.ReadPointer();

                rom.Seek(sprite_table_addr + (spriteIndex * 4));
                rom.Seek(rom.ReadPointer());
                int numberOfAnims = rom.ReadByte();
                rom.Skip(0x53);

                string outDirectory = Path.Combine(outRoot, name);
                int diri = 2;
                while (Directory.Exists(outDirectory))
                {
                    outDirectory = Path.Combine(outRoot, $"{name} {diri++}");
                }
                Directory.CreateDirectory(outDirectory);

                for (int a = 0; a < numberOfAnims; a++)
                {
                    int animPtr = rom.ReadPointer();
                    if (animPtr == 0x0) continue;

                    rom.PushPosition(animPtr);
                    int numberOfFrames = rom.ReadInt() << 2;
                    var frames = new Bitmap[numberOfFrames];

                    for (int f = 0; f < numberOfFrames; f++)
                    {
                        rom.PushPosition(rom.ReadPointer());
                        int numberOfParts = rom.ReadInt();
                        int padX = rom.ReadByte();     // not really sure what this is for
                        int padY = rom.ReadByte();
                        int frameWidth = rom.ReadByte();
                        int frameHeight = rom.ReadByte();

                        if (numberOfParts == 1)
                        {
                            rom.Skip(0x7);
                            bool flipped = (rom.ReadByte() & 0x10) != 0; // there might be more bitflags in here
                            byte[] frameBytes = JCALG1.Decompress(rom, rom.ReadPointer());
                            frames[f] = CreateImage(frameBytes, frameWidth, frameHeight, 8, flipped, palette);
                        }
                        else
                        {
                            var frameImage = new Bitmap(frameWidth, frameHeight);
                            using var g = Graphics.FromImage(frameImage);
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                            for (int n = 0; n < numberOfParts; n++)
                            {
                                int x = rom.ReadByte() - padX;
                                int y = rom.ReadByte() - padY;
                                int width = rom.ReadByte();
                                int height = rom.ReadByte();
                                rom.Skip(3);
                                bool flipped = (rom.ReadByte() & 0x10) != 0; // there might be more bitflags in here
                                byte[] frameBytes = JCALG1.Decompress(rom, rom.ReadPointer());
                                var part = CreateImage(frameBytes, width, height, 8, flipped, palette);
                                g.DrawImage(part, x, y, width, height);
                            }

                            frames[f] = frameImage;
                        }
                        
                        rom.PopPosition();
                    }

                    CreateAnimStrip(frames).Save(Path.Combine(outDirectory, $"{anim_names[a]}.png"));

                    rom.PopPosition();
                }

                if (portraitPtr != 0x0)
                {
                    byte[] portraitBytes = JCALG1.Decompress(rom, portraitPtr);
                    var portraitImage = CreateImage(portraitBytes, 64, 64, 64, false, palette);
                    portraitImage.Save(Path.Combine(outDirectory, "portrait.png"));
                }

            }
        }

        private static Bitmap CreateAnimStrip(Bitmap[] frames)
        {
            int widest = 0;
            int tallest = 0;

            foreach (var image in frames)
            {
                widest = Math.Max(widest, image.Width);
                tallest = Math.Max(tallest, image.Height);
            }

            int width = widest * frames.Length;
            int height = tallest;

            var strip = new Bitmap(width, height);
            using var g = Graphics.FromImage(strip);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            int x = 0;

            foreach (var image in frames)
            {
                int offx = (widest - image.Width) / 2;
                int offy = tallest - image.Height;
                g.DrawImage(image, x + offx, offy);
                x += widest;
            }
            return strip;
        }

        private static Bitmap CreateImage(byte[] bytes, int width, int height, int tileSize, bool flipped, Color[] pal)
        {
            var bitmap = new Bitmap(width, height);
            int r = 0;
            int dx = 0;
            int dy = 0;
            int step = tileSize * tileSize;

            while (r + step <= bytes.Length)
            {
                for (int x = 0; x < tileSize; x++)
                {
                    for (int y = 0; y < tileSize; y++)
                    {
                        int index = y * tileSize + x;
                        bitmap.SetPixel(dx + x, dy + y, pal[bytes[r + index]]);
                    }
                }
                r += 64;
                dx += tileSize;
                if (dx >= width)
                {
                    dx = 0;
                    dy += tileSize;
                    if (dy >= height) break;
                }
            }

            if (flipped)
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }

            return bitmap;
        }
    }
}