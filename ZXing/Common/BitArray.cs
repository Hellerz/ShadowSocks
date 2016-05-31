namespace ZXing.Common
{
    using System;
    using System.Reflection;

    public sealed class BitArray
    {
        private static readonly int[] _lookup = new int[] { 
            0x20, 0, 1, 0x1a, 2, 0x17, 0x1b, 0, 3, 0x10, 0x18, 30, 0x1c, 11, 0, 13, 
            4, 7, 0x11, 0, 0x19, 0x16, 0x1f, 15, 0x1d, 10, 12, 6, 0, 0x15, 14, 9, 
            5, 20, 8, 0x13, 0x12
         };
        private int[] bits;
        private int size;

        public BitArray()
        {
            this.size = 0;
            this.bits = new int[1];
        }

        public BitArray(int size)
        {
            if (size < 1)
            {
                throw new ArgumentException("size must be at least 1");
            }
            this.size = size;
            this.bits = makeArray(size);
        }

        private BitArray(int[] bits, int size)
        {
            this.bits = bits;
            this.size = size;
        }

        public void appendBit(bool bit)
        {
            this.ensureCapacity(this.size + 1);
            if (bit)
            {
                this.bits[this.size >> 5] |= ((int) 1) << this.size;
            }
            this.size++;
        }

        public void appendBitArray(BitArray other)
        {
            int size = other.size;
            this.ensureCapacity(this.size + size);
            for (int i = 0; i < size; i++)
            {
                this.appendBit(other[i]);
            }
        }

        public void appendBits(int value, int numBits)
        {
            if ((numBits < 0) || (numBits > 0x20))
            {
                throw new ArgumentException("Num bits must be between 0 and 32");
            }
            this.ensureCapacity(this.size + numBits);
            for (int i = numBits; i > 0; i--)
            {
                this.appendBit(((value >> (i - 1)) & 1) == 1);
            }
        }

        public void clear()
        {
            int length = this.bits.Length;
            for (int i = 0; i < length; i++)
            {
                this.bits[i] = 0;
            }
        }

        public object Clone()
        {
            return new BitArray((int[]) this.bits.Clone(), this.size);
        }

        private void ensureCapacity(int size)
        {
            if (size > (this.bits.Length << 5))
            {
                int[] destinationArray = makeArray(size);
                System.Array.Copy(this.bits, 0, destinationArray, 0, this.bits.Length);
                this.bits = destinationArray;
            }
        }

        public override bool Equals(object o)
        {
            BitArray array = o as BitArray;
            if (array == null)
            {
                return false;
            }
            if (this.size != array.size)
            {
                return false;
            }
            for (int i = 0; i < this.size; i++)
            {
                if (this.bits[i] != array.bits[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int size = this.size;
            foreach (int num2 in this.bits)
            {
                size = (0x1f * size) + num2.GetHashCode();
            }
            return size;
        }

        private static int[] makeArray(int size)
        {
            return new int[(size + 0x1f) >> 5];
        }

        private static int numberOfTrailingZeros(int num)
        {
            int index = (-num & num) % 0x25;
            if (index < 0)
            {
                index *= -1;
            }
            return _lookup[index];
        }

        public void setBulk(int i, int newBits)
        {
            this.bits[i >> 5] = newBits;
        }

        public void toBytes(int bitOffset, byte[] array, int offset, int numBytes)
        {
            for (int i = 0; i < numBytes; i++)
            {
                int num2 = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (this[bitOffset])
                    {
                        num2 |= ((int) 1) << (7 - j);
                    }
                    bitOffset++;
                }
                array[offset + i] = (byte) num2;
            }
        }

        public void xor(BitArray other)
        {
            if (this.bits.Length != other.bits.Length)
            {
                throw new ArgumentException("Sizes don't match");
            }
            for (int i = 0; i < this.bits.Length; i++)
            {
                this.bits[i] ^= other.bits[i];
            }
        }

        public int[] Array
        {
            get
            {
                return this.bits;
            }
        }

        public bool this[int i]
        {
            get
            {
                return ((this.bits[i >> 5] & (((int) 1) << i)) != 0);
            }
            set
            {
                if (value)
                {
                    this.bits[i >> 5] |= ((int) 1) << i;
                }
            }
        }

        public int Size
        {
            get
            {
                return this.size;
            }
        }

        public int SizeInBytes
        {
            get
            {
                return ((this.size + 7) >> 3);
            }
        }
    }
}

