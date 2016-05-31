namespace ZXing.QrCode.Internal
{
    using System;
    using ZXing;
    using ZXing.Common;

    public static class MatrixUtil
    {
        private static readonly int[][] POSITION_ADJUSTMENT_PATTERN;
        private static readonly int[][] POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE;
        private static readonly int[][] POSITION_DETECTION_PATTERN;
        private static readonly int[][] TYPE_INFO_COORDINATES;
        private const int TYPE_INFO_MASK_PATTERN = 0x5412;
        private const int TYPE_INFO_POLY = 0x537;
        private const int VERSION_INFO_POLY = 0x1f25;

        static MatrixUtil()
        {
            int[][] numArray = new int[7][];
            numArray[0] = new int[] { 1, 1, 1, 1, 1, 1, 1 };
            int[] numArray2 = new int[7];
            numArray2[0] = 1;
            numArray2[6] = 1;
            numArray[1] = numArray2;
            numArray[2] = new int[] { 1, 0, 1, 1, 1, 0, 1 };
            numArray[3] = new int[] { 1, 0, 1, 1, 1, 0, 1 };
            numArray[4] = new int[] { 1, 0, 1, 1, 1, 0, 1 };
            int[] numArray3 = new int[7];
            numArray3[0] = 1;
            numArray3[6] = 1;
            numArray[5] = numArray3;
            numArray[6] = new int[] { 1, 1, 1, 1, 1, 1, 1 };
            POSITION_DETECTION_PATTERN = numArray;
            int[][] numArray4 = new int[5][];
            numArray4[0] = new int[] { 1, 1, 1, 1, 1 };
            int[] numArray5 = new int[5];
            numArray5[0] = 1;
            numArray5[4] = 1;
            numArray4[1] = numArray5;
            numArray4[2] = new int[] { 1, 0, 1, 0, 1 };
            int[] numArray6 = new int[5];
            numArray6[0] = 1;
            numArray6[4] = 1;
            numArray4[3] = numArray6;
            numArray4[4] = new int[] { 1, 1, 1, 1, 1 };
            POSITION_ADJUSTMENT_PATTERN = numArray4;
            POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE = new int[][] { 
                new int[] { -1, -1, -1, -1, -1, -1, -1 }, new int[] { 6, 0x12, -1, -1, -1, -1, -1 }, new int[] { 6, 0x16, -1, -1, -1, -1, -1 }, new int[] { 6, 0x1a, -1, -1, -1, -1, -1 }, new int[] { 6, 30, -1, -1, -1, -1, -1 }, new int[] { 6, 0x22, -1, -1, -1, -1, -1 }, new int[] { 6, 0x16, 0x26, -1, -1, -1, -1 }, new int[] { 6, 0x18, 0x2a, -1, -1, -1, -1 }, new int[] { 6, 0x1a, 0x2e, -1, -1, -1, -1 }, new int[] { 6, 0x1c, 50, -1, -1, -1, -1 }, new int[] { 6, 30, 0x36, -1, -1, -1, -1 }, new int[] { 6, 0x20, 0x3a, -1, -1, -1, -1 }, new int[] { 6, 0x22, 0x3e, -1, -1, -1, -1 }, new int[] { 6, 0x1a, 0x2e, 0x42, -1, -1, -1 }, new int[] { 6, 0x1a, 0x30, 70, -1, -1, -1 }, new int[] { 6, 0x1a, 50, 0x4a, -1, -1, -1 }, 
                new int[] { 6, 30, 0x36, 0x4e, -1, -1, -1 }, new int[] { 6, 30, 0x38, 0x52, -1, -1, -1 }, new int[] { 6, 30, 0x3a, 0x56, -1, -1, -1 }, new int[] { 6, 0x22, 0x3e, 90, -1, -1, -1 }, new int[] { 6, 0x1c, 50, 0x48, 0x5e, -1, -1 }, new int[] { 6, 0x1a, 50, 0x4a, 0x62, -1, -1 }, new int[] { 6, 30, 0x36, 0x4e, 0x66, -1, -1 }, new int[] { 6, 0x1c, 0x36, 80, 0x6a, -1, -1 }, new int[] { 6, 0x20, 0x3a, 0x54, 110, -1, -1 }, new int[] { 6, 30, 0x3a, 0x56, 0x72, -1, -1 }, new int[] { 6, 0x22, 0x3e, 90, 0x76, -1, -1 }, new int[] { 6, 0x1a, 50, 0x4a, 0x62, 0x7a, -1 }, new int[] { 6, 30, 0x36, 0x4e, 0x66, 0x7e, -1 }, new int[] { 6, 0x1a, 0x34, 0x4e, 0x68, 130, -1 }, new int[] { 6, 30, 0x38, 0x52, 0x6c, 0x86, -1 }, new int[] { 6, 0x22, 60, 0x56, 0x70, 0x8a, -1 }, 
                new int[] { 6, 30, 0x3a, 0x56, 0x72, 0x8e, -1 }, new int[] { 6, 0x22, 0x3e, 90, 0x76, 0x92, -1 }, new int[] { 6, 30, 0x36, 0x4e, 0x66, 0x7e, 150 }, new int[] { 6, 0x18, 50, 0x4c, 0x66, 0x80, 0x9a }, new int[] { 6, 0x1c, 0x36, 80, 0x6a, 0x84, 0x9e }, new int[] { 6, 0x20, 0x3a, 0x54, 110, 0x88, 0xa2 }, new int[] { 6, 0x1a, 0x36, 0x52, 110, 0x8a, 0xa6 }, new int[] { 6, 30, 0x3a, 0x56, 0x72, 0x8e, 170 }
             };
            int[][] numArray8 = new int[15][];
            int[] numArray9 = new int[2];
            numArray9[0] = 8;
            numArray8[0] = numArray9;
            numArray8[1] = new int[] { 8, 1 };
            numArray8[2] = new int[] { 8, 2 };
            numArray8[3] = new int[] { 8, 3 };
            numArray8[4] = new int[] { 8, 4 };
            numArray8[5] = new int[] { 8, 5 };
            numArray8[6] = new int[] { 8, 7 };
            numArray8[7] = new int[] { 8, 8 };
            numArray8[8] = new int[] { 7, 8 };
            numArray8[9] = new int[] { 5, 8 };
            numArray8[10] = new int[] { 4, 8 };
            numArray8[11] = new int[] { 3, 8 };
            numArray8[12] = new int[] { 2, 8 };
            numArray8[13] = new int[] { 1, 8 };
            int[] numArray23 = new int[2];
            numArray23[1] = 8;
            numArray8[14] = numArray23;
            TYPE_INFO_COORDINATES = numArray8;
        }

        public static void buildMatrix(BitArray dataBits, ErrorCorrectionLevel ecLevel, Version version, int maskPattern, ByteMatrix matrix)
        {
            clearMatrix(matrix);
            embedBasicPatterns(version, matrix);
            embedTypeInfo(ecLevel, maskPattern, matrix);
            maybeEmbedVersionInfo(version, matrix);
            embedDataBits(dataBits, maskPattern, matrix);
        }

        public static int calculateBCHCode(int value, int poly)
        {
            int num = findMSBSet(poly);
            value = value << (num - 1);
            while (findMSBSet(value) >= num)
            {
                value ^= poly << (findMSBSet(value) - num);
            }
            return value;
        }

        public static void clearMatrix(ByteMatrix matrix)
        {
            matrix.clear(2);
        }

        public static void embedBasicPatterns(Version version, ByteMatrix matrix)
        {
            embedPositionDetectionPatternsAndSeparators(matrix);
            embedDarkDotAtLeftBottomCorner(matrix);
            maybeEmbedPositionAdjustmentPatterns(version, matrix);
            embedTimingPatterns(matrix);
        }

        private static void embedDarkDotAtLeftBottomCorner(ByteMatrix matrix)
        {
            if (matrix[8, matrix.Height - 8] == 0)
            {
                throw new WriterException();
            }
            matrix[8, matrix.Height - 8] = 1;
        }

        public static void embedDataBits(BitArray dataBits, int maskPattern, ByteMatrix matrix)
        {
            int num = 0;
            int num2 = -1;
            int num3 = matrix.Width - 1;
            int y = matrix.Height - 1;
            while (num3 > 0)
            {
                if (num3 == 6)
                {
                    num3--;
                }
                while ((y >= 0) && (y < matrix.Height))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int x = num3 - i;
                        if (isEmpty(matrix[x, y]))
                        {
                            int num7;
                            if (num < dataBits.Size)
                            {
                                num7 = dataBits[num] ? 1 : 0;
                                num++;
                            }
                            else
                            {
                                num7 = 0;
                            }
                            if ((maskPattern != -1) && MaskUtil.getDataMaskBit(maskPattern, x, y))
                            {
                                num7 ^= 1;
                            }
                            matrix[x, y] = num7;
                        }
                    }
                    y += num2;
                }
                num2 = -num2;
                y += num2;
                num3 -= 2;
            }
            if (num != dataBits.Size)
            {
                throw new WriterException(string.Concat(new object[] { "Not all bits consumed: ", num, '/', dataBits.Size }));
            }
        }

        private static void embedHorizontalSeparationPattern(int xStart, int yStart, ByteMatrix matrix)
        {
            for (int i = 0; i < 8; i++)
            {
                if (!isEmpty(matrix[xStart + i, yStart]))
                {
                    throw new WriterException();
                }
                matrix[xStart + i, yStart] = 0;
            }
        }

        private static void embedPositionAdjustmentPattern(int xStart, int yStart, ByteMatrix matrix)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    matrix[xStart + j, yStart + i] = POSITION_ADJUSTMENT_PATTERN[i][j];
                }
            }
        }

        private static void embedPositionDetectionPattern(int xStart, int yStart, ByteMatrix matrix)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    matrix[xStart + j, yStart + i] = POSITION_DETECTION_PATTERN[i][j];
                }
            }
        }

        private static void embedPositionDetectionPatternsAndSeparators(ByteMatrix matrix)
        {
            int length = POSITION_DETECTION_PATTERN[0].Length;
            embedPositionDetectionPattern(0, 0, matrix);
            embedPositionDetectionPattern(matrix.Width - length, 0, matrix);
            embedPositionDetectionPattern(0, matrix.Width - length, matrix);
            embedHorizontalSeparationPattern(0, 7, matrix);
            embedHorizontalSeparationPattern(matrix.Width - 8, 7, matrix);
            embedHorizontalSeparationPattern(0, matrix.Width - 8, matrix);
            embedVerticalSeparationPattern(7, 0, matrix);
            embedVerticalSeparationPattern((matrix.Height - 7) - 1, 0, matrix);
            embedVerticalSeparationPattern(7, matrix.Height - 7, matrix);
        }

        private static void embedTimingPatterns(ByteMatrix matrix)
        {
            for (int i = 8; i < (matrix.Width - 8); i++)
            {
                int num2 = (i + 1) % 2;
                if (isEmpty(matrix[i, 6]))
                {
                    matrix[i, 6] = num2;
                }
                if (isEmpty(matrix[6, i]))
                {
                    matrix[6, i] = num2;
                }
            }
        }

        public static void embedTypeInfo(ErrorCorrectionLevel ecLevel, int maskPattern, ByteMatrix matrix)
        {
            BitArray bits = new BitArray();
            makeTypeInfoBits(ecLevel, maskPattern, bits);
            for (int i = 0; i < bits.Size; i++)
            {
                int num2 = bits[(bits.Size - 1) - i] ? 1 : 0;
                int num3 = TYPE_INFO_COORDINATES[i][0];
                int num4 = TYPE_INFO_COORDINATES[i][1];
                matrix[num3, num4] = num2;
                if (i < 8)
                {
                    int num5 = (matrix.Width - i) - 1;
                    int num6 = 8;
                    matrix[num5, num6] = num2;
                }
                else
                {
                    int num7 = 8;
                    int num8 = (matrix.Height - 7) + (i - 8);
                    matrix[num7, num8] = num2;
                }
            }
        }

        private static void embedVerticalSeparationPattern(int xStart, int yStart, ByteMatrix matrix)
        {
            for (int i = 0; i < 7; i++)
            {
                if (!isEmpty(matrix[xStart, yStart + i]))
                {
                    throw new WriterException();
                }
                matrix[xStart, yStart + i] = 0;
            }
        }

        public static int findMSBSet(int value_Renamed)
        {
            int num = 0;
            while (value_Renamed != 0)
            {
                value_Renamed = value_Renamed >> 1;
                num++;
            }
            return num;
        }

        private static bool isEmpty(int value)
        {
            return (value == 2);
        }

        public static void makeTypeInfoBits(ErrorCorrectionLevel ecLevel, int maskPattern, BitArray bits)
        {
            if (!QRCode.isValidMaskPattern(maskPattern))
            {
                throw new WriterException("Invalid mask pattern");
            }
            int num = (ecLevel.Bits << 3) | maskPattern;
            bits.appendBits(num, 5);
            int num2 = calculateBCHCode(num, 0x537);
            bits.appendBits(num2, 10);
            BitArray other = new BitArray();
            other.appendBits(0x5412, 15);
            bits.xor(other);
            if (bits.Size != 15)
            {
                throw new WriterException("should not happen but we got: " + bits.Size);
            }
        }

        public static void makeVersionInfoBits(Version version, BitArray bits)
        {
            bits.appendBits(version.VersionNumber, 6);
            int num = calculateBCHCode(version.VersionNumber, 0x1f25);
            bits.appendBits(num, 12);
            if (bits.Size != 0x12)
            {
                throw new WriterException("should not happen but we got: " + bits.Size);
            }
        }

        private static void maybeEmbedPositionAdjustmentPatterns(Version version, ByteMatrix matrix)
        {
            if (version.VersionNumber >= 2)
            {
                int index = version.VersionNumber - 1;
                int[] numArray = POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE[index];
                int length = POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE[index].Length;
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        int num5 = numArray[i];
                        int num6 = numArray[j];
                        if (((num6 != -1) && (num5 != -1)) && isEmpty(matrix[num6, num5]))
                        {
                            embedPositionAdjustmentPattern(num6 - 2, num5 - 2, matrix);
                        }
                    }
                }
            }
        }

        public static void maybeEmbedVersionInfo(Version version, ByteMatrix matrix)
        {
            if (version.VersionNumber >= 7)
            {
                BitArray bits = new BitArray();
                makeVersionInfoBits(version, bits);
                int num = 0x11;
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int num4 = bits[num] ? 1 : 0;
                        num--;
                        matrix[i, (matrix.Height - 11) + j] = num4;
                        matrix[(matrix.Height - 11) + j, i] = num4;
                    }
                }
            }
        }
    }
}

