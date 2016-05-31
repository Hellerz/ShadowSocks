namespace ZXing.Common
{
    using System;
    using System.Runtime.CompilerServices;
    using ZXing;

    public class DetectorResult
    {
        
        private BitMatrix Bitsk__BackingField;
        
        private ResultPoint[] Pointsk__BackingField;

        public DetectorResult(BitMatrix bits, ResultPoint[] points)
        {
            this.Bits = bits;
            this.Points = points;
        }

        public BitMatrix Bits
        {
            
            get
            {
                return this.Bitsk__BackingField;
            }
            private 
            set
            {
                this.Bitsk__BackingField = value;
            }
        }

        public ResultPoint[] Points
        {
            
            get
            {
                return this.Pointsk__BackingField;
            }
            private 
            set
            {
                this.Pointsk__BackingField = value;
            }
        }
    }
}

