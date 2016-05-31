namespace ZXing.QrCode.Internal
{
    using System;

    public sealed class ErrorCorrectionLevel
    {
        private readonly int bits;
        private static readonly ErrorCorrectionLevel[] FOR_BITS = new ErrorCorrectionLevel[] { M, L, H, Q };
        public static readonly ErrorCorrectionLevel H = new ErrorCorrectionLevel(3, 2, "H");
        public static readonly ErrorCorrectionLevel L = new ErrorCorrectionLevel(0, 1, "L");
        public static readonly ErrorCorrectionLevel M = new ErrorCorrectionLevel(1, 0, "M");
        private readonly string name;
        private readonly int ordinal_Renamed_Field;
        public static readonly ErrorCorrectionLevel Q = new ErrorCorrectionLevel(2, 3, "Q");

        private ErrorCorrectionLevel(int ordinal, int bits, string name)
        {
            this.ordinal_Renamed_Field = ordinal;
            this.bits = bits;
            this.name = name;
        }

        public static ErrorCorrectionLevel forBits(int bits)
        {
            if ((bits < 0) || (bits >= FOR_BITS.Length))
            {
                throw new ArgumentException();
            }
            return FOR_BITS[bits];
        }

        public int ordinal()
        {
            return this.ordinal_Renamed_Field;
        }

        public override string ToString()
        {
            return this.name;
        }

        public int Bits
        {
            get
            {
                return this.bits;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}

