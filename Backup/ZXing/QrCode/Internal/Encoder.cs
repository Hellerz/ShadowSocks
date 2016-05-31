namespace ZXing.QrCode.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ZXing;
    using ZXing.Common;
    using ZXing.Common.ReedSolomon;

    public static class Encoder
    {
        private static readonly int[] ALPHANUMERIC_TABLE = new int[] { 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            0x24, -1, -1, -1, 0x25, 0x26, -1, -1, -1, -1, 0x27, 40, -1, 0x29, 0x2a, 0x2b, 
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0x2c, -1, -1, -1, -1, -1, 
            -1, 10, 11, 12, 13, 14, 15, 0x10, 0x11, 0x12, 0x13, 20, 0x15, 0x16, 0x17, 0x18, 
            0x19, 0x1a, 0x1b, 0x1c, 0x1d, 30, 0x1f, 0x20, 0x21, 0x22, 0x23, -1, -1, -1, -1, -1
         };
        internal static string DEFAULT_BYTE_MODE_ENCODING = "ISO-8859-1";

        internal static void append8BitBytes(string content, BitArray bits, string encoding)
        {
            byte[] bytes;
            try
            {
                bytes = Encoding.GetEncoding(encoding).GetBytes(content);
            }
            catch (Exception exception)
            {
                throw new WriterException(exception.Message, exception);
            }
            foreach (byte num in bytes)
            {
                bits.appendBits(num, 8);
            }
        }

        internal static void appendBytes(string content, Mode mode, BitArray bits, string encoding)
        {
            if (!mode.Equals(Mode.BYTE))
            {
                throw new WriterException("Invalid mode: " + mode);
            }
            append8BitBytes(content, bits, encoding);
        }

        internal static void appendLengthInfo(int numLetters, Version version, Mode mode, BitArray bits)
        {
            int numBits = mode.getCharacterCountBits(version);
            if (numLetters >= (((int) 1) << numBits))
            {
                throw new WriterException(numLetters + " is bigger than " + ((((int) 1) << numBits) - 1));
            }
            bits.appendBits(numLetters, numBits);
        }

        internal static void appendModeInfo(Mode mode, BitArray bits)
        {
            bits.appendBits(mode.Bits, 4);
        }

        internal static void appendNumericBytes(string content, BitArray bits)
        {
            int length = content.Length;
            int num2 = 0;
            while (num2 < length)
            {
                int num3 = content[num2] - '0';
                if ((num2 + 2) < length)
                {
                    int num4 = content[num2 + 1] - '0';
                    int num5 = content[num2 + 2] - '0';
                    bits.appendBits(((num3 * 100) + (num4 * 10)) + num5, 10);
                    num2 += 3;
                }
                else
                {
                    if ((num2 + 1) < length)
                    {
                        int num6 = content[num2 + 1] - '0';
                        bits.appendBits((num3 * 10) + num6, 7);
                        num2 += 2;
                        continue;
                    }
                    bits.appendBits(num3, 4);
                    num2++;
                }
            }
        }

        private static int calculateMaskPenalty(ByteMatrix matrix)
        {
            return (((MaskUtil.applyMaskPenaltyRule1(matrix) + MaskUtil.applyMaskPenaltyRule2(matrix)) + MaskUtil.applyMaskPenaltyRule3(matrix)) + MaskUtil.applyMaskPenaltyRule4(matrix));
        }

        private static int chooseMaskPattern(BitArray bits, ErrorCorrectionLevel ecLevel, Version version, ByteMatrix matrix)
        {
            int num = 0x7fffffff;
            int num2 = -1;
            for (int i = 0; i < QRCode.NUM_MASK_PATTERNS; i++)
            {
                MatrixUtil.buildMatrix(bits, ecLevel, version, i, matrix);
                int num4 = calculateMaskPenalty(matrix);
                if (num4 < num)
                {
                    num = num4;
                    num2 = i;
                }
            }
            return num2;
        }

        public static Mode chooseMode(string content)
        {
            return chooseMode(content, null);
        }

        private static Mode chooseMode(string content, string encoding)
        {
            return Mode.BYTE;
        }

        private static Version chooseVersion(int numInputBits, ErrorCorrectionLevel ecLevel)
        {
            for (int i = 1; i <= 40; i++)
            {
                Version version = Version.getVersionForNumber(i);
                int totalCodewords = version.TotalCodewords;
                int totalECCodewords = version.getECBlocksForLevel(ecLevel).TotalECCodewords;
                int num4 = totalCodewords - totalECCodewords;
                int num5 = (numInputBits + 7) / 8;
                if (num4 >= num5)
                {
                    return version;
                }
            }
            throw new WriterException("Data too big");
        }

        public static QRCode encode(string content, ErrorCorrectionLevel ecLevel)
        {
            return encode(content, ecLevel, null);
        }

        public static QRCode encode(string content, ErrorCorrectionLevel ecLevel, IDictionary<EncodeHintType, object> hints)
        {
            string str = ((hints == null) || !hints.ContainsKey(EncodeHintType.CHARACTER_SET)) ? null : ((string) hints[EncodeHintType.CHARACTER_SET]);
            if (str == null)
            {
                str = DEFAULT_BYTE_MODE_ENCODING;
            }
            DEFAULT_BYTE_MODE_ENCODING.Equals(str);
            Mode mode = chooseMode(content, str);
            BitArray bits = new BitArray();
            appendModeInfo(mode, bits);
            BitArray array2 = new BitArray();
            appendBytes(content, mode, array2, str);
            int numInputBits = (bits.Size + mode.getCharacterCountBits(Version.getVersionForNumber(1))) + array2.Size;
            Version version = chooseVersion(numInputBits, ecLevel);
            int num2 = (bits.Size + mode.getCharacterCountBits(version)) + array2.Size;
            Version version2 = chooseVersion(num2, ecLevel);
            BitArray array3 = new BitArray();
            array3.appendBitArray(bits);
            int numLetters = (mode == Mode.BYTE) ? array2.SizeInBytes : content.Length;
            appendLengthInfo(numLetters, version2, mode, array3);
            array3.appendBitArray(array2);
            Version.ECBlocks blocks = version2.getECBlocksForLevel(ecLevel);
            int numDataBytes = version2.TotalCodewords - blocks.TotalECCodewords;
            terminateBits(numDataBytes, array3);
            BitArray array4 = interleaveWithECBytes(array3, version2.TotalCodewords, numDataBytes, blocks.NumBlocks);
            QRCode code2 = new QRCode();
            code2.ECLevel = ecLevel;
            code2.Mode = mode;
            code2.Version = version2;
            QRCode code = code2;
            int dimensionForVersion = version2.DimensionForVersion;
            ByteMatrix matrix = new ByteMatrix(dimensionForVersion, dimensionForVersion);
            int maskPattern = chooseMaskPattern(array4, ecLevel, version2, matrix);
            code.MaskPattern = maskPattern;
            MatrixUtil.buildMatrix(array4, ecLevel, version2, maskPattern, matrix);
            code.Matrix = matrix;
            return code;
        }

        internal static byte[] generateECBytes(byte[] dataBytes, int numEcBytesInBlock)
        {
            int length = dataBytes.Length;
            int[] toEncode = new int[length + numEcBytesInBlock];
            for (int i = 0; i < length; i++)
            {
                toEncode[i] = dataBytes[i] & 0xff;
            }
            new ReedSolomonEncoder(GenericGF.QR_CODE_FIELD_256).encode(toEncode, numEcBytesInBlock);
            byte[] buffer = new byte[numEcBytesInBlock];
            for (int j = 0; j < numEcBytesInBlock; j++)
            {
                buffer[j] = (byte) toEncode[length + j];
            }
            return buffer;
        }

        internal static int getAlphanumericCode(int code)
        {
            if (code < ALPHANUMERIC_TABLE.Length)
            {
                return ALPHANUMERIC_TABLE[code];
            }
            return -1;
        }

        internal static void getNumDataBytesAndNumECBytesForBlockID(int numTotalBytes, int numDataBytes, int numRSBlocks, int blockID, int[] numDataBytesInBlock, int[] numECBytesInBlock)
        {
            if (blockID >= numRSBlocks)
            {
                throw new WriterException("Block ID too large");
            }
            int num = numTotalBytes % numRSBlocks;
            int num2 = numRSBlocks - num;
            int num3 = numTotalBytes / numRSBlocks;
            int num4 = num3 + 1;
            int num5 = numDataBytes / numRSBlocks;
            int num6 = num5 + 1;
            int num7 = num3 - num5;
            int num8 = num4 - num6;
            if (num7 != num8)
            {
                throw new WriterException("EC bytes mismatch");
            }
            if (numRSBlocks != (num2 + num))
            {
                throw new WriterException("RS blocks mismatch");
            }
            if (numTotalBytes != (((num5 + num7) * num2) + ((num6 + num8) * num)))
            {
                throw new WriterException("Total bytes mismatch");
            }
            if (blockID < num2)
            {
                numDataBytesInBlock[0] = num5;
                numECBytesInBlock[0] = num7;
            }
            else
            {
                numDataBytesInBlock[0] = num6;
                numECBytesInBlock[0] = num8;
            }
        }

        internal static BitArray interleaveWithECBytes(BitArray bits, int numTotalBytes, int numDataBytes, int numRSBlocks)
        {
            if (bits.SizeInBytes != numDataBytes)
            {
                throw new WriterException("Number of bits and data bytes does not match");
            }
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            List<BlockPair> list = new List<BlockPair>(numRSBlocks);
            for (int i = 0; i < numRSBlocks; i++)
            {
                int[] numDataBytesInBlock = new int[1];
                int[] numECBytesInBlock = new int[1];
                getNumDataBytesAndNumECBytesForBlockID(numTotalBytes, numDataBytes, numRSBlocks, i, numDataBytesInBlock, numECBytesInBlock);
                int numBytes = numDataBytesInBlock[0];
                byte[] buffer = new byte[numBytes];
                bits.toBytes(8 * num, buffer, 0, numBytes);
                byte[] errorCorrection = generateECBytes(buffer, numECBytesInBlock[0]);
                list.Add(new BlockPair(buffer, errorCorrection));
                num2 = Math.Max(num2, numBytes);
                num3 = Math.Max(num3, errorCorrection.Length);
                num += numDataBytesInBlock[0];
            }
            if (numDataBytes != num)
            {
                throw new WriterException("Data bytes does not match offset");
            }
            BitArray array = new BitArray();
            for (int j = 0; j < num2; j++)
            {
                foreach (BlockPair pair in list)
                {
                    byte[] dataBytes = pair.DataBytes;
                    if (j < dataBytes.Length)
                    {
                        array.appendBits(dataBytes[j], 8);
                    }
                }
            }
            for (int k = 0; k < num3; k++)
            {
                foreach (BlockPair pair2 in list)
                {
                    byte[] errorCorrectionBytes = pair2.ErrorCorrectionBytes;
                    if (k < errorCorrectionBytes.Length)
                    {
                        array.appendBits(errorCorrectionBytes[k], 8);
                    }
                }
            }
            if (numTotalBytes != array.SizeInBytes)
            {
                throw new WriterException(string.Concat(new object[] { "Interleaving error: ", numTotalBytes, " and ", array.SizeInBytes, " differ." }));
            }
            return array;
        }

        internal static void terminateBits(int numDataBytes, BitArray bits)
        {
            int num = numDataBytes << 3;
            if (bits.Size > num)
            {
                throw new WriterException(string.Concat(new object[] { "data bits cannot fit in the QR Code", bits.Size, " > ", num }));
            }
            for (int i = 0; (i < 4) && (bits.Size < num); i++)
            {
                bits.appendBit(false);
            }
            int num3 = bits.Size & 7;
            if (num3 > 0)
            {
                for (int k = num3; k < 8; k++)
                {
                    bits.appendBit(false);
                }
            }
            int num5 = numDataBytes - bits.SizeInBytes;
            for (int j = 0; j < num5; j++)
            {
                bits.appendBits(((j & 1) == 0) ? 0xec : 0x11, 8);
            }
            if (bits.Size != num)
            {
                throw new WriterException("Bits size does not equal capacity");
            }
        }
    }
}

