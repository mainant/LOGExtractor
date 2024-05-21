using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOGExtractor.Gba
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    internal class ROM
    {
        private readonly byte[] mem;
        private int _position;
        private Stack<int> positionStack = new();

        public int Position => _position;
        public int Length => mem.Length;
        public int Remaining => Length - _position;

        private string _DebuggerDisplay => $"0x08{_position:X6}";

        private ROM(byte[] _mem)
        {
            mem = _mem;
        }

        public static ROM FromFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            byte[] bytes = File.ReadAllBytes(filePath);
            return new ROM(bytes);
        }

        public void Seek(int newPosition)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(newPosition, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(newPosition, Length);

            _position = newPosition;
        }

        public void Skip(int n)
        {
            CheckLength(n);
            _position += n;
        }

        public void PushPosition(int newPosition)
        {
            int current = _position;
            Seek(newPosition);
            positionStack.Push(current);
        }

        public void PopPosition()
        {
            if (positionStack.Count == 0)
                throw new InvalidOperationException();

            int pos = positionStack.Pop();
            Seek(pos);
        }

        public int ReadByte()
        {
            CheckLength(1);
            return Read();
        }

        public int ReadShort()
        {
            CheckLength(2);
            return Read() | (Read() << 8);
        }

        public int ReadShortBigEndian()
        {
            CheckLength(2);
            return (Read() << 8) | Read();
        }

        public int ReadInt()
        {
            CheckLength(4);
            return Read() | (Read() << 8) | (Read() << 16) | (Read() << 24);
        }

        public int ReadPointer()
        {
            int value = ReadInt();
            if ((value & 0x08000000) != 0)
            {
                return value & 0xFFFFFF;
            }
            return value;
        }

        public string ReadUnicodeString()
        {
            int startPos = _position;
            int length = 0;
            while (ReadShort() != 0)
            {
                length += 2;
            }
            return Encoding.Unicode.GetString(mem, startPos, length);
        }

        private byte Read()
        {
            return mem[_position++];
        }

        private void CheckLength(int n)
        {
            if (n > Remaining)
                throw new InvalidOperationException();
        }

    }
}
