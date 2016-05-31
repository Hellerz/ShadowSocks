namespace ZXing
{
    using System;

    public abstract class BaseLuminanceSource : LuminanceSource
    {
        protected const int BChannelWeight = 0x1d00;
        protected const int ChannelWeight = 0x10;
        protected const int GChannelWeight = 0x9696;
        protected byte[] luminances;
        protected const int RChannelWeight = 0x4c6a;

        protected BaseLuminanceSource(int width, int height) : base(width, height)
        {
            this.luminances = new byte[width * height];
        }

        protected BaseLuminanceSource(byte[] luminanceArray, int width, int height) : base(width, height)
        {
            this.luminances = new byte[width * height];
            Buffer.BlockCopy(luminanceArray, 0, this.luminances, 0, width * height);
        }

        protected abstract LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height);
        public override LuminanceSource crop(int left, int top, int width, int height)
        {
            if (((left + width) > this.Width) || ((top + height) > this.Height))
            {
                throw new ArgumentException("Crop rectangle does not fit within image data.");
            }
            byte[] newLuminances = new byte[width * height];
            byte[] matrix = this.Matrix;
            int num = this.Width;
            int num2 = left + width;
            int num3 = top + height;
            int num4 = top;
            for (int i = 0; num4 < num3; i++)
            {
                int num6 = left;
                for (int j = 0; num6 < num2; j++)
                {
                    newLuminances[(i * width) + j] = matrix[(num4 * num) + num6];
                    num6++;
                }
                num4++;
            }
            return this.CreateLuminanceSource(newLuminances, width, height);
        }

        public override byte[] getRow(int y, byte[] row)
        {
            int width = this.Width;
            if ((row == null) || (row.Length < width))
            {
                row = new byte[width];
            }
            for (int i = 0; i < width; i++)
            {
                row[i] = this.luminances[(y * width) + i];
            }
            return row;
        }

        public override LuminanceSource rotateCounterClockwise()
        {
            byte[] newLuminances = new byte[this.Width * this.Height];
            int height = this.Height;
            int width = this.Width;
            byte[] matrix = this.Matrix;
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    int num5 = (width - j) - 1;
                    int num6 = i;
                    newLuminances[(num5 * height) + num6] = matrix[(i * this.Width) + j];
                }
            }
            return this.CreateLuminanceSource(newLuminances, height, width);
        }

        public override LuminanceSource rotateCounterClockwise45()
        {
            return base.rotateCounterClockwise45();
        }

        public override bool CropSupported
        {
            get
            {
                return true;
            }
        }

        public override bool InversionSupported
        {
            get
            {
                return true;
            }
        }

        public override byte[] Matrix
        {
            get
            {
                return this.luminances;
            }
        }

        public override bool RotateSupported
        {
            get
            {
                return true;
            }
        }
    }
}

