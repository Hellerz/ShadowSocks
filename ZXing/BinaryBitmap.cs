namespace ZXing
{
    using System;
    using ZXing.Common;

    public sealed class BinaryBitmap
    {
        private Binarizer binarizer;
        private BitMatrix matrix;

        public BinaryBitmap(Binarizer binarizer)
        {
            if (binarizer == null)
            {
                throw new ArgumentException("Binarizer must be non-null.");
            }
            this.binarizer = binarizer;
        }

        public BinaryBitmap crop(int left, int top, int width, int height)
        {
            LuminanceSource source = this.binarizer.LuminanceSource.crop(left, top, width, height);
            return new BinaryBitmap(this.binarizer.createBinarizer(source));
        }

        public BitArray getBlackRow(int y, BitArray row)
        {
            return this.binarizer.getBlackRow(y, row);
        }

        public BinaryBitmap rotateCounterClockwise()
        {
            LuminanceSource source = this.binarizer.LuminanceSource.rotateCounterClockwise();
            return new BinaryBitmap(this.binarizer.createBinarizer(source));
        }

        public BinaryBitmap rotateCounterClockwise45()
        {
            LuminanceSource source = this.binarizer.LuminanceSource.rotateCounterClockwise45();
            return new BinaryBitmap(this.binarizer.createBinarizer(source));
        }

        public override string ToString()
        {
            BitMatrix blackMatrix = this.BlackMatrix;
            if (blackMatrix == null)
            {
                return string.Empty;
            }
            return blackMatrix.ToString();
        }

        public BitMatrix BlackMatrix
        {
            get
            {
                if (this.matrix == null)
                {
                    this.matrix = this.binarizer.BlackMatrix;
                }
                return this.matrix;
            }
        }

        public bool CropSupported
        {
            get
            {
                return this.binarizer.LuminanceSource.CropSupported;
            }
        }

        public int Height
        {
            get
            {
                return this.binarizer.Height;
            }
        }

        public bool RotateSupported
        {
            get
            {
                return this.binarizer.LuminanceSource.RotateSupported;
            }
        }

        public int Width
        {
            get
            {
                return this.binarizer.Width;
            }
        }
    }
}

