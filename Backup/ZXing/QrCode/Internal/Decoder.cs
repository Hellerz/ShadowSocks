namespace ZXing.QrCode.Internal
{
    using System;
    using System.Collections.Generic;
    using ZXing.Common;
    using ZXing.Common.ReedSolomon;

    public sealed class Decoder
    {
        private readonly ReedSolomonDecoder rsDecoder = new ReedSolomonDecoder(GenericGF.QR_CODE_FIELD_256);

        private bool correctErrors(byte[] codewordBytes, int numDataCodewords)
        {
            int length = codewordBytes.Length;
            int[] received = new int[length];
            for (int i = 0; i < length; i++)
            {
                received[i] = codewordBytes[i] & 0xff;
            }
            int twoS = codewordBytes.Length - numDataCodewords;
            if (!this.rsDecoder.decode(received, twoS))
            {
                return false;
            }
            for (int j = 0; j < numDataCodewords; j++)
            {
                codewordBytes[j] = (byte) received[j];
            }
            return true;
        }

        public DecoderResult decode(BitMatrix bits, IDictionary<DecodeHintType, object> hints)
        {
            BitMatrixParser parser = BitMatrixParser.createBitMatrixParser(bits);
            if (parser == null)
            {
                return null;
            }
            DecoderResult result = this.decode(parser, hints);
            if (result == null)
            {
                parser.remask();
                parser.setMirror(true);
                if (parser.readVersion() == null)
                {
                    return null;
                }
                if (parser.readFormatInformation() == null)
                {
                    return null;
                }
                parser.mirror();
                result = this.decode(parser, hints);
                if (result != null)
                {
                    result.Other = new QRCodeDecoderMetaData(true);
                }
            }
            return result;
        }

        private DecoderResult decode(BitMatrixParser parser, IDictionary<DecodeHintType, object> hints)
        {
            Version version = parser.readVersion();
            if (version == null)
            {
                return null;
            }
            FormatInformation information = parser.readFormatInformation();
            if (information == null)
            {
                return null;
            }
            ErrorCorrectionLevel errorCorrectionLevel = information.ErrorCorrectionLevel;
            byte[] rawCodewords = parser.readCodewords();
            if (rawCodewords == null)
            {
                return null;
            }
            DataBlock[] blockArray = DataBlock.getDataBlocks(rawCodewords, version, errorCorrectionLevel);
            int num = 0;
            foreach (DataBlock block in blockArray)
            {
                num += block.NumDataCodewords;
            }
            byte[] bytes = new byte[num];
            int num2 = 0;
            foreach (DataBlock block2 in blockArray)
            {
                byte[] codewords = block2.Codewords;
                int numDataCodewords = block2.NumDataCodewords;
                if (!this.correctErrors(codewords, numDataCodewords))
                {
                    return null;
                }
                for (int i = 0; i < numDataCodewords; i++)
                {
                    bytes[num2++] = codewords[i];
                }
            }
            return DecodedBitStreamParser.decode(bytes, version, errorCorrectionLevel, hints);
        }

        public DecoderResult decode(bool[][] image, IDictionary<DecodeHintType, object> hints)
        {
            int length = image.Length;
            BitMatrix bits = new BitMatrix(length);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    bits[j, i] = image[i][j];
                }
            }
            return this.decode(bits, hints);
        }
    }
}

