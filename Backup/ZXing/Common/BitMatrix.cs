namespace ZXing.Common
{
    using System;
    using System.Reflection;

    public sealed class BitMatrix
    {
        private readonly int[] bits;
        private readonly int height;
        private readonly int rowSize;
        private readonly int width;

        public BitMatrix(int dimension) : this(dimension, dimension)
        {
        }

        public BitMatrix(int width, int height)
        {
            if ((width < 1) || (height < 1))
            {
                throw new ArgumentException("Both dimensions must be greater than 0");
            }
            this.width = width;
            this.height = height;
            this.rowSize = (width + 0x1f) >> 5;
            this.bits = new int[this.rowSize * height];
        }

        private BitMatrix(int width, int height, int rowSize, int[] bits)
        {
            this.width = width;
            this.height = height;
            this.rowSize = rowSize;
            this.bits = bits;
        }

        public void flip(int x, int y)
        {
            int index = (y * this.rowSize) + (x >> 5);
            this.bits[index] ^= ((int) 1) << x;
        }

        public int[] getBottomRightOnBit()
        {
            int index = this.bits.Length - 1;
            while ((index >= 0) && (this.bits[index] == 0))
            {
                index--;
            }
            if (index < 0)
            {
                return null;
            }
            int num2 = index / this.rowSize;
            int num3 = (index % this.rowSize) << 5;
            int num4 = this.bits[index];
            int num5 = 0x1f;
            while ((num4 >> num5) == 0)
            {
                num5--;
            }
            num3 += num5;
            return new int[] { num3, num2 };
        }

        public BitArray getRow(int y, BitArray row)
        {
            if ((row == null) || (row.Size < this.width))
            {
                row = new BitArray(this.width);
            }
            else
            {
                row.clear();
            }
            int num = y * this.rowSize;
            for (int i = 0; i < this.rowSize; i++)
            {
                row.setBulk(i << 5, this.bits[num + i]);
            }
            return row;
        }

        public int[] getTopLeftOnBit()
        {
            int index = 0;
            while ((index < this.bits.Length) && (this.bits[index] == 0))
            {
                index++;
            }
            if (index == this.bits.Length)
            {
                return null;
            }
            int num2 = index / this.rowSize;
            int num3 = (index % this.rowSize) << 5;
            int num4 = this.bits[index];
            int num5 = 0;
            while ((num4 << (0x1f - num5)) == 0)
            {
                num5++;
            }
            num3 += num5;
            return new int[] { num3, num2 };
        }

        public void setRegion(int left, int top, int width, int height)
        {
            if ((top < 0) || (left < 0))
            {
                throw new ArgumentException("Left and top must be nonnegative");
            }
            if ((height < 1) || (width < 1))
            {
                throw new ArgumentException("Height and width must be at least 1");
            }
            int num = left + width;
            int num2 = top + height;
            if ((num2 > this.height) || (num > this.width))
            {
                throw new ArgumentException("The region must fit inside the matrix");
            }
            for (int i = top; i < num2; i++)
            {
                int num4 = i * this.rowSize;
                for (int j = left; j < num; j++)
                {
                    this.bits[num4 + (j >> 5)] |= ((int) 1) << j;
                }
            }
        }

        public void setRow(int y, BitArray row)
        {
            Array.Copy(row.Array, 0, this.bits, y * this.rowSize, this.rowSize);
        }

        public int Height
        {
            get
            {
                return this.height;
            }
        }

        public bool this[int x, int y]
        {
            get
            {
                int index = (y * this.rowSize) + (x >> 5);
                return (((this.bits[index] >> x) & 1) != 0);
            }
            set
            {
                if (value)
                {
                    int index = (y * this.rowSize) + (x >> 5);
                    this.bits[index] |= ((int) 1) << x;
                }
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
        }
    }
}

