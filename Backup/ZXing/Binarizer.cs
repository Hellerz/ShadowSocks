namespace ZXing
{
    using System;
    using ZXing.Common;

    public abstract class Binarizer
    {
        private readonly ZXing.LuminanceSource source;

        protected internal Binarizer(ZXing.LuminanceSource source)
        {
            if (source == null)
            {
                throw new ArgumentException("Source must be non-null.");
            }
            this.source = source;
        }

        public abstract Binarizer createBinarizer(ZXing.LuminanceSource source);
        public abstract BitArray getBlackRow(int y, BitArray row);

        public abstract BitMatrix BlackMatrix { get; }

        public int Height
        {
            get
            {
                return this.source.Height;
            }
        }

        public virtual ZXing.LuminanceSource LuminanceSource
        {
            get
            {
                return this.source;
            }
        }

        public int Width
        {
            get
            {
                return this.source.Width;
            }
        }
    }
}

