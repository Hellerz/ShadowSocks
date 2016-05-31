namespace ZXing.QrCode.Internal
{
    using System;

    internal sealed class BlockPair
    {
        private readonly byte[] dataBytes;
        private readonly byte[] errorCorrectionBytes;

        public BlockPair(byte[] data, byte[] errorCorrection)
        {
            this.dataBytes = data;
            this.errorCorrectionBytes = errorCorrection;
        }

        public byte[] DataBytes
        {
            get
            {
                return this.dataBytes;
            }
        }

        public byte[] ErrorCorrectionBytes
        {
            get
            {
                return this.errorCorrectionBytes;
            }
        }
    }
}

