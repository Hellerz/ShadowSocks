namespace ZXing.QrCode.Internal
{
    using System;

    public static class MaskUtil
    {
        private const int N1 = 3;
        private const int N2 = 3;
        private const int N3 = 40;
        private const int N4 = 10;

        public static int applyMaskPenaltyRule1(ByteMatrix matrix)
        {
            return (applyMaskPenaltyRule1Internal(matrix, true) + applyMaskPenaltyRule1Internal(matrix, false));
        }

        private static int applyMaskPenaltyRule1Internal(ByteMatrix matrix, bool isHorizontal)
        {
            int num = 0;
            int num2 = isHorizontal ? matrix.Height : matrix.Width;
            int num3 = isHorizontal ? matrix.Width : matrix.Height;
            byte[][] array = matrix.Array;
            for (int i = 0; i < num2; i++)
            {
                int num5 = 0;
                int num6 = -1;
                for (int j = 0; j < num3; j++)
                {
                    int num8 = isHorizontal ? array[i][j] : array[j][i];
                    if (num8 == num6)
                    {
                        num5++;
                    }
                    else
                    {
                        if (num5 >= 5)
                        {
                            num += 3 + (num5 - 5);
                        }
                        num5 = 1;
                        num6 = num8;
                    }
                }
                if (num5 >= 5)
                {
                    num += 3 + (num5 - 5);
                }
            }
            return num;
        }

        public static int applyMaskPenaltyRule2(ByteMatrix matrix)
        {
            int num = 0;
            byte[][] array = matrix.Array;
            int width = matrix.Width;
            int height = matrix.Height;
            for (int i = 0; i < (height - 1); i++)
            {
                for (int j = 0; j < (width - 1); j++)
                {
                    int num6 = array[i][j];
                    if (((num6 == array[i][j + 1]) && (num6 == array[i + 1][j])) && (num6 == array[i + 1][j + 1]))
                    {
                        num++;
                    }
                }
            }
            return (3 * num);
        }

        public static int applyMaskPenaltyRule3(ByteMatrix matrix)
        {
            int num = 0;
            byte[][] array = matrix.Array;
            int width = matrix.Width;
            int height = matrix.Height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte[] rowArray = array[i];
                    if ((((((j + 6) < width) && (rowArray[j] == 1)) && ((rowArray[j + 1] == 0) && (rowArray[j + 2] == 1))) && (((rowArray[j + 3] == 1) && (rowArray[j + 4] == 1)) && ((rowArray[j + 5] == 0) && (rowArray[j + 6] == 1)))) && (isWhiteHorizontal(rowArray, j - 4, j) || isWhiteHorizontal(rowArray, j + 7, j + 11)))
                    {
                        num++;
                    }
                    if ((((((i + 6) < height) && (array[i][j] == 1)) && ((array[i + 1][j] == 0) && (array[i + 2][j] == 1))) && (((array[i + 3][j] == 1) && (array[i + 4][j] == 1)) && ((array[i + 5][j] == 0) && (array[i + 6][j] == 1)))) && (isWhiteVertical(array, j, i - 4, i) || isWhiteVertical(array, j, i + 7, i + 11)))
                    {
                        num++;
                    }
                }
            }
            return (num * 40);
        }

        public static int applyMaskPenaltyRule4(ByteMatrix matrix)
        {
            int num = 0;
            byte[][] array = matrix.Array;
            int width = matrix.Width;
            int height = matrix.Height;
            for (int i = 0; i < height; i++)
            {
                byte[] buffer = array[i];
                for (int j = 0; j < width; j++)
                {
                    if (buffer[j] == 1)
                    {
                        num++;
                    }
                }
            }
            int num6 = matrix.Height * matrix.Width;
            double num7 = ((double) num) / ((double) num6);
            int num8 = (int) (Math.Abs((double) (num7 - 0.5)) * 20.0);
            return (num8 * 10);
        }

        public static bool getDataMaskBit(int maskPattern, int x, int y)
        {
            int num;
            int num2;
            switch (maskPattern)
            {
                case 0:
                    num = (y + x) & 1;
                    break;

                case 1:
                    num = y & 1;
                    break;

                case 2:
                    num = x % 3;
                    break;

                case 3:
                    num = (y + x) % 3;
                    break;

                case 4:
                    num = ((y >> 1) + (x / 3)) & 1;
                    break;

                case 5:
                    num2 = y * x;
                    num = (num2 & 1) + (num2 % 3);
                    break;

                case 6:
                    num2 = y * x;
                    num = ((num2 & 1) + (num2 % 3)) & 1;
                    break;

                case 7:
                    num2 = y * x;
                    num = ((num2 % 3) + ((y + x) & 1)) & 1;
                    break;

                default:
                    throw new ArgumentException("Invalid mask pattern: " + maskPattern);
            }
            return (num == 0);
        }

        private static bool isWhiteHorizontal(byte[] rowArray, int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                if (((i >= 0) && (i < rowArray.Length)) && (rowArray[i] == 1))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool isWhiteVertical(byte[][] array, int col, int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                if (((i >= 0) && (i < array.Length)) && (array[i][col] == 1))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

