namespace ZXing.QrCode.Internal
{
    using System;
    using System.Reflection;
    using System.Text;

    public sealed class ByteMatrix
    {
        private readonly byte[][] bytes;
        private readonly int height;
        private readonly int width;

        public ByteMatrix(int width, int height)
        {
            this.bytes = new byte[height][];
            for (int i = 0; i < height; i++)
            {
                this.bytes[i] = new byte[width];
            }
            this.width = width;
            this.height = height;
        }

        public void clear(byte value)
        {
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    this.bytes[i][j] = value;
                }
            }
        }

        public void set(int x, int y, bool value)
        {
            this.bytes[y][x] = value ? ((byte) 1) : ((byte) 0);
        }

        public void set(int x, int y, byte value)
        {
            this.bytes[y][x] = value;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(((2 * this.width) * this.height) + 2);
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    switch (this.bytes[i][j])
                    {
                        case 0:
                            builder.Append(" 0");
                            break;

                        case 1:
                            builder.Append(" 1");
                            break;

                        default:
                            builder.Append("  ");
                            break;
                    }
                }
                builder.Append('\n');
            }
            return builder.ToString();
        }

        public byte[][] Array
        {
            get
            {
                return this.bytes;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
        }

        public int this[int x, int y]
        {
            get
            {
                return this.bytes[y][x];
            }
            set
            {
                this.bytes[y][x] = (byte) value;
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

