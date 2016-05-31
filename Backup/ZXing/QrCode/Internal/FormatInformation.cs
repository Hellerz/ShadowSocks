namespace ZXing.QrCode.Internal
{
    using System;

    internal sealed class FormatInformation
    {
        private static readonly int[] BITS_SET_IN_HALF_BYTE;
        private readonly byte dataMask;
        private readonly ZXing.QrCode.Internal.ErrorCorrectionLevel errorCorrectionLevel;
        private static readonly int[][] FORMAT_INFO_DECODE_LOOKUP;
        private const int FORMAT_INFO_MASK_QR = 0x5412;

        static FormatInformation()
        {
            int[][] numArray = new int[0x20][];
            int[] numArray2 = new int[2];
            numArray2[0] = 0x5412;
            numArray[0] = numArray2;
            numArray[1] = new int[] { 0x5125, 1 };
            numArray[2] = new int[] { 0x5e7c, 2 };
            numArray[3] = new int[] { 0x5b4b, 3 };
            numArray[4] = new int[] { 0x45f9, 4 };
            numArray[5] = new int[] { 0x40ce, 5 };
            numArray[6] = new int[] { 0x4f97, 6 };
            numArray[7] = new int[] { 0x4aa0, 7 };
            numArray[8] = new int[] { 0x77c4, 8 };
            numArray[9] = new int[] { 0x72f3, 9 };
            numArray[10] = new int[] { 0x7daa, 10 };
            numArray[11] = new int[] { 0x789d, 11 };
            numArray[12] = new int[] { 0x662f, 12 };
            numArray[13] = new int[] { 0x6318, 13 };
            numArray[14] = new int[] { 0x6c41, 14 };
            numArray[15] = new int[] { 0x6976, 15 };
            numArray[0x10] = new int[] { 0x1689, 0x10 };
            numArray[0x11] = new int[] { 0x13be, 0x11 };
            numArray[0x12] = new int[] { 0x1ce7, 0x12 };
            numArray[0x13] = new int[] { 0x19d0, 0x13 };
            numArray[20] = new int[] { 0x762, 20 };
            numArray[0x15] = new int[] { 0x255, 0x15 };
            numArray[0x16] = new int[] { 0xd0c, 0x16 };
            numArray[0x17] = new int[] { 0x83b, 0x17 };
            numArray[0x18] = new int[] { 0x355f, 0x18 };
            numArray[0x19] = new int[] { 0x3068, 0x19 };
            numArray[0x1a] = new int[] { 0x3f31, 0x1a };
            numArray[0x1b] = new int[] { 0x3a06, 0x1b };
            numArray[0x1c] = new int[] { 0x24b4, 0x1c };
            numArray[0x1d] = new int[] { 0x2183, 0x1d };
            numArray[30] = new int[] { 0x2eda, 30 };
            numArray[0x1f] = new int[] { 0x2bed, 0x1f };
            FORMAT_INFO_DECODE_LOOKUP = numArray;
            BITS_SET_IN_HALF_BYTE = new int[] { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };
        }

        private FormatInformation(int formatInfo)
        {
            this.errorCorrectionLevel = ZXing.QrCode.Internal.ErrorCorrectionLevel.forBits((formatInfo >> 3) & 3);
            this.dataMask = (byte) (formatInfo & 7);
        }

        internal static FormatInformation decodeFormatInformation(int maskedFormatInfo1, int maskedFormatInfo2)
        {
            FormatInformation information = doDecodeFormatInformation(maskedFormatInfo1, maskedFormatInfo2);
            if (information != null)
            {
                return information;
            }
            return doDecodeFormatInformation(maskedFormatInfo1 ^ 0x5412, maskedFormatInfo2 ^ 0x5412);
        }

        private static FormatInformation doDecodeFormatInformation(int maskedFormatInfo1, int maskedFormatInfo2)
        {
            int num = 0x7fffffff;
            int formatInfo = 0;
            foreach (int[] numArray in FORMAT_INFO_DECODE_LOOKUP)
            {
                int b = numArray[0];
                if ((b == maskedFormatInfo1) || (b == maskedFormatInfo2))
                {
                    return new FormatInformation(numArray[1]);
                }
                int num4 = numBitsDiffering(maskedFormatInfo1, b);
                if (num4 < num)
                {
                    formatInfo = numArray[1];
                    num = num4;
                }
                if (maskedFormatInfo1 != maskedFormatInfo2)
                {
                    num4 = numBitsDiffering(maskedFormatInfo2, b);
                    if (num4 < num)
                    {
                        formatInfo = numArray[1];
                        num = num4;
                    }
                }
            }
            if (num <= 3)
            {
                return new FormatInformation(formatInfo);
            }
            return null;
        }

        public override bool Equals(object o)
        {
            if (!(o is FormatInformation))
            {
                return false;
            }
            FormatInformation information = (FormatInformation) o;
            return ((this.errorCorrectionLevel == information.errorCorrectionLevel) && (this.dataMask == information.dataMask));
        }

        public override int GetHashCode()
        {
            return ((this.errorCorrectionLevel.ordinal() << 3) | this.dataMask);
        }

        internal static int numBitsDiffering(int a, int b)
        {
            a ^= b;
            return (((((((BITS_SET_IN_HALF_BYTE[a & 15] + BITS_SET_IN_HALF_BYTE[(a >> 4) & 15]) + BITS_SET_IN_HALF_BYTE[(a >> 8) & 15]) + BITS_SET_IN_HALF_BYTE[(a >> 12) & 15]) + BITS_SET_IN_HALF_BYTE[(a >> 0x10) & 15]) + BITS_SET_IN_HALF_BYTE[(a >> 20) & 15]) + BITS_SET_IN_HALF_BYTE[(a >> 0x18) & 15]) + BITS_SET_IN_HALF_BYTE[(a >> 0x1c) & 15]);
        }

        internal byte DataMask
        {
            get
            {
                return this.dataMask;
            }
        }

        internal ZXing.QrCode.Internal.ErrorCorrectionLevel ErrorCorrectionLevel
        {
            get
            {
                return this.errorCorrectionLevel;
            }
        }
    }
}

