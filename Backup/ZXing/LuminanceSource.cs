namespace ZXing
{
    using System;
    using System.Text;

    public abstract class LuminanceSource
    {
        private int height;
        private int width;

        protected LuminanceSource(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public virtual LuminanceSource crop(int left, int top, int width, int height)
        {
            throw new NotSupportedException("This luminance source does not support cropping.");
        }

        public abstract byte[] getRow(int y, byte[] row);
        public virtual LuminanceSource rotateCounterClockwise()
        {
            throw new NotSupportedException("This luminance source does not support rotation.");
        }

        public virtual LuminanceSource rotateCounterClockwise45()
        {
            throw new NotSupportedException("This luminance source does not support rotation by 45 degrees.");
        }

        public override string ToString()
        {
            byte[] row = new byte[this.width];
            StringBuilder builder = new StringBuilder(this.height * (this.width + 1));
            for (int i = 0; i < this.height; i++)
            {
                row = this.getRow(i, row);
                for (int j = 0; j < this.width; j++)
                {
                    char ch;
                    int num3 = row[j] & 0xff;
                    if (num3 < 0x40)
                    {
                        ch = '#';
                    }
                    else if (num3 < 0x80)
                    {
                        ch = '+';
                    }
                    else if (num3 < 0xc0)
                    {
                        ch = '.';
                    }
                    else
                    {
                        ch = ' ';
                    }
                    builder.Append(ch);
                }
                builder.Append('\n');
            }
            return builder.ToString();
        }

        public virtual bool CropSupported
        {
            get
            {
                return false;
            }
        }

        public virtual int Height
        {
            get
            {
                return this.height;
            }
            protected set
            {
                this.height = value;
            }
        }

        public virtual bool InversionSupported
        {
            get
            {
                return false;
            }
        }

        public abstract byte[] Matrix { get; }

        public virtual bool RotateSupported
        {
            get
            {
                return false;
            }
        }

        public virtual int Width
        {
            get
            {
                return this.width;
            }
            protected set
            {
                this.width = value;
            }
        }
    }
}

