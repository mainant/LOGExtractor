using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGExtractor.Gba
{
    internal static class JCALG1
    {
        // https://github.com/jeremycollake/jcalg1/blob/master/CompressedData.cpp

        public static byte[] DecompressUnknownHeader(ROM rom, int address)
        {
            ArgumentNullException.ThrowIfNull(rom);
            ArgumentOutOfRangeException.ThrowIfNegative(address);

            rom.PushPosition(address);
            rom.Skip(0x4);

            const int buf_size = 1024 * 64;
            var (data, size) = DecompressInternal(rom, buf_size);
            Array.Resize(ref data, size);

            rom.PopPosition();
            return data;
        }

        public static byte[] Decompress(ROM rom, int address)
        {
            ArgumentNullException.ThrowIfNull(rom);
            ArgumentOutOfRangeException.ThrowIfNegative(address);

            rom.PushPosition(address);

            int skip = rom.ReadInt(); // data type/format or something?
            if (skip == 0)
            {
                return [];
            }
            int decompressedSize = rom.ReadInt();

            var (data, _) = DecompressInternal(rom, decompressedSize);

            rom.PopPosition();
            return data;
        }

        private static (byte[], int) DecompressInternal(ROM rom, int decompressedSize)
        {
            var state = new CompressionState();
            var source = new CompressionSource(rom);

            byte[] destination = new byte[decompressedSize];
            int w = 0;

            while (true)
            {
                if (source.GetBit() != 0)
                {
                    // literal
                    destination[w++] = (byte)(source.GetBits(state.literalBits) + state.literalOffset);
                }
                else
                {
                    if (source.GetBit() != 0)
                    {
                        // normal phrase
                        int highIndex = source.GetInteger();

                        if (highIndex == 2)
                        {
                            int phraseLength = source.GetInteger();
                            TransferMatch(destination, ref w, state.lastIndex, phraseLength);
                        }
                        else
                        {
                            state.lastIndex = ((highIndex - 3) << state.indexBase) + source.GetBits(state.indexBase);

                            int phraseLength = source.GetInteger();

                            if (state.lastIndex >= 0x10000) phraseLength += 3;
                            else if (state.lastIndex >= 0x37FF) phraseLength += 2;
                            else if (state.lastIndex >= 0x27F) phraseLength++;
                            else if (state.lastIndex <= 127) phraseLength += 4;

                            TransferMatch(destination, ref w, state.lastIndex, phraseLength);
                        }
                    }
                    else if (source.GetBit() != 0)
                    {
                        // one-byte phrase or literal size change
                        int value = source.GetBits(4) - 1;
                        if (value == 0)
                        {
                            destination[w++] = 0;
                        }
                        else if (value > 0)
                        {
                            destination[w] = destination[w - value];
                            w++;
                        }
                        else
                        {
                            if (source.GetBit() != 0)
                            {
                                do
                                {
                                    for (int i = 0; i < 256; i++)
                                    {
                                        destination[w++] = (byte)source.GetBits(8);
                                    }
                                } while (source.GetBit() != 0);
                            }
                            else
                            {
                                state.literalBits = 7 + source.GetBit();
                                state.literalOffset = 0;
                                if (state.literalBits != 8)
                                {
                                    state.literalOffset = source.GetBits(8);
                                }
                            }
                        }
                    }
                    else
                    {
                        // short phrase
                        int index = source.GetBits(7);
                        int length = source.GetBits(2) + 2;
                        if (index == 0)
                        {
                            // extended short
                            if (length == 2) break; // DecompDone
                            state.indexBase = source.GetBits(length + 1);
                        }
                        else
                        {
                            state.lastIndex = index;
                            TransferMatch(destination, ref w, state.lastIndex, length);
                        }
                    }
                }
            }

            return (destination, w);
        }

        private static void TransferMatch(byte[] destination, ref int w, int offset, int length)
        {
            do
            {
                destination[w] = destination[w++ - offset];
            }
            while (--length > 0);
        }

        private class CompressionState
        {
            public int lastIndex;
            public int indexBase;
            public int literalBits;
            public int literalOffset;

            public CompressionState()
            {
                lastIndex = 1; indexBase = 8;
            }
        };

        private class CompressionSource(ROM _rom)
        {
            private ROM rom = _rom;
            private uint bitBuffer;
            private int bitsRemaining;

            public int GetBit()
            {
                if (bitsRemaining == 0)
                {
                    bitsRemaining = 32;
                    AdvanceBuffer();
                }

                uint result = bitBuffer >> 31;
                bitBuffer <<= 1;
                bitsRemaining--;
                return (int)result;
            }

            public int GetBits(int Count)
            {
                if (bitsRemaining >= Count)
                {
                    uint result = bitBuffer >> (32 - Count);
                    bitBuffer <<= Count;
                    bitsRemaining -= Count;
                    return (int)result;
                }
                else
                {
                    int remainder = Count - bitsRemaining;

                    uint result = bitBuffer >> (32 - bitsRemaining) << remainder;
                    AdvanceBuffer();

                    result |= bitBuffer >> (32 - remainder);
                    bitsRemaining = 32 - remainder;
                    bitBuffer <<= remainder;

                    return (int)result;
                }
            }

            public int GetInteger()
            {
                int result = 1;
                do
                {
                    result = (result << 1) + GetBit();
                }
                while (GetBit() != 0);
                return result;
            }

            private void AdvanceBuffer()
            {
                if (rom.Remaining >= 4)
                {
                    bitBuffer = (uint)rom.ReadInt();
                }
                else
                {
                    bitBuffer = 0;
                }
            }
        }

    }
}
