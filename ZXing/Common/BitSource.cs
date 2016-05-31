namespace ZXing.Common
{
    using System;

    public sealed class BitSource
    {
        private int bitOffset;
        private int byteOffset;
        private readonly byte[] bytes;

        public BitSource(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public int available()
        {
            return ((8 * (this.bytes.Length - this.byteOffset)) - this.bitOffset);
        }

        public int readBits(int numBits)
        {
            if (((numBits < 1) || (numBits > 0x20)) || (numBits > this.available()))
            {
                throw new ArgumentException(numBits.ToString(), "numBits");
            }
            int num = 0;
            if (this.bitOffset > 0)
            {
                int num2 = 8 - this.bitOffset;
                int num3 = (numBits < num2) ? numBits : num2;
                int num4 = num2 - num3;
                int num5 = (0xff >> ((8 - num3) & 0x1f)) << num4;
                num = (this.bytes[this.byteOffset] & num5) >> num4;
                numBits -= num3;
                this.bitOffset += num3;
                if (this.bitOffset == 8)
                {
                    this.bitOffset = 0;
                    this.byteOffset++;
                }
            }
            if (numBits > 0)
            {
                while (numBits >= 8)
                {
                    num = (num << 8) | (this.bytes[this.byteOffset] & 0xff);
                    this.byteOffset++;
                    numBits -= 8;
                }
                if (numBits > 0)
                {
                    int num6 = 8 - numBits;
                    int num7 = (0xff >> (num6 & 0x1f)) << num6;
                    num = (num << numBits) | ((this.bytes[this.byteOffset] & num7) >> num6);
                    this.bitOffset += numBits;
                }
            }
            return num;
        }

        public int BitOffset
        {
            get
            {
                return this.bitOffset;
            }
        }

        public int ByteOffset
        {
            get
            {
                return this.byteOffset;
            }
        }
    }
}

