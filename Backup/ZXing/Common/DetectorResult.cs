namespace ZXing.Common
{
    using System;
    using System.Runtime.CompilerServices;
    using ZXing;

    public class DetectorResult
    {
        [CompilerGenerated]
        private BitMatrix <Bits>k__BackingField;
        [CompilerGenerated]
        private ResultPoint[] <Points>k__BackingField;

        public DetectorResult(BitMatrix bits, ResultPoint[] points)
        {
            this.Bits = bits;
            this.Points = points;
        }

        public BitMatrix Bits
        {
            [CompilerGenerated]
            get
            {
                return this.<Bits>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<Bits>k__BackingField = value;
            }
        }

        public ResultPoint[] Points
        {
            [CompilerGenerated]
            get
            {
                return this.<Points>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<Points>k__BackingField = value;
            }
        }
    }
}

