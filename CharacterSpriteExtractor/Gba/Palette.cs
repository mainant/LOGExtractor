using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGExtractor.Gba
{
    public static class Palette
    {

        public static Color[] FromFile(string file)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException(file);
            }
            using var br = new BinaryReader(File.OpenRead(file));
            string ext = Path.HasExtension(file) ? Path.GetExtension(file).Substring(1).ToLower() : string.Empty;
            var format = PaletteFileFormat.Unsupported;
            int entries = 256;

            if (ext.Equals("pal") || br.ReadInt32() == 0x46464952)
            {
                format = PaletteFileFormat.RIFFPalette;
                br.BaseStream.Position = 0x16;
                entries = br.ReadInt16();
            }
            else if (ext.Equals("act") || br.BaseStream.Length == 768 || br.BaseStream.Length == 772)
            {
                format = PaletteFileFormat.AdobeColorTable;
                br.BaseStream.Position = 0;
            }

            if (format == PaletteFileFormat.Unsupported)
            {
                throw new Exception("Unsupported palette format");
            }

            var colors = new Color[entries];
            for (int i = 0; i < entries; i++)
            {
                byte r = br.ReadByte();
                byte g = br.ReadByte();
                byte b = br.ReadByte();
                if (format == PaletteFileFormat.RIFFPalette)
                {
                    br.ReadByte();
                }
                colors[i] = Color.FromArgb(r, g, b);
            }

            colors[0] = Color.FromArgb(0, 0, 0, 0);

            return colors;
        }

    }
}
