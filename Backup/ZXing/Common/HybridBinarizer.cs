namespace ZXing.Common
{
    using System;
    using ZXing;

    public sealed class HybridBinarizer : GlobalHistogramBinarizer
    {
        private const int BLOCK_SIZE = 8;
        private const int BLOCK_SIZE_MASK = 7;
        private const int BLOCK_SIZE_POWER = 3;
        private BitMatrix matrix;
        private const int MIN_DYNAMIC_RANGE = 0x18;
        private const int MINIMUM_DIMENSION = 40;

        public HybridBinarizer(LuminanceSource source) : base(source)
        {
        }

        private void binarizeEntireImage()
        {
            if (this.matrix == null)
            {
                LuminanceSource luminanceSource = this.LuminanceSource;
                int width = luminanceSource.Width;
                int height = luminanceSource.Height;
                if ((width >= 40) && (height >= 40))
                {
                    byte[] luminances = luminanceSource.Matrix;
                    int subWidth = width >> 3;
                    if ((width & 7) != 0)
                    {
                        subWidth++;
                    }
                    int subHeight = height >> 3;
                    if ((height & 7) != 0)
                    {
                        subHeight++;
                    }
                    int[][] blackPoints = calculateBlackPoints(luminances, subWidth, subHeight, width, height);
                    BitMatrix matrix = new BitMatrix(width, height);
                    calculateThresholdForBlock(luminances, subWidth, subHeight, width, height, blackPoints, matrix);
                    this.matrix = matrix;
                }
                else
                {
                    this.matrix = base.BlackMatrix;
                }
            }
        }

        private static int[][] calculateBlackPoints(byte[] luminances, int subWidth, int subHeight, int width, int height)
        {
            int[][] numArray = new int[subHeight][];
            for (int i = 0; i < subHeight; i++)
            {
                numArray[i] = new int[subWidth];
            }
            for (int j = 0; j < subHeight; j++)
            {
                int num3 = j << 3;
                int num4 = height - 8;
                if (num3 > num4)
                {
                    num3 = num4;
                }
                for (int k = 0; k < subWidth; k++)
                {
                    int num6 = k << 3;
                    int num7 = width - 8;
                    if (num6 > num7)
                    {
                        num6 = num7;
                    }
                    int num8 = 0;
                    int num9 = 0xff;
                    int num10 = 0;
                    int num11 = 0;
                    for (int m = (num3 * width) + num6; num11 < 8; m += width)
                    {
                        for (int n = 0; n < 8; n++)
                        {
                            int num14 = luminances[m + n] & 0xff;
                            num8 += num14;
                            if (num14 < num9)
                            {
                                num9 = num14;
                            }
                            if (num14 > num10)
                            {
                                num10 = num14;
                            }
                        }
                        if ((num10 - num9) > 0x18)
                        {
                            num11++;
                            for (m += width; num11 < 8; m += width)
                            {
                                for (int num15 = 0; num15 < 8; num15++)
                                {
                                    num8 += luminances[m + num15] & 0xff;
                                }
                                num11++;
                            }
                        }
                        num11++;
                    }
                    int num16 = num8 >> 6;
                    if ((num10 - num9) <= 0x18)
                    {
                        num16 = num9 >> 1;
                        if ((j > 0) && (k > 0))
                        {
                            int num17 = ((numArray[j - 1][k] + (2 * numArray[j][k - 1])) + numArray[j - 1][k - 1]) >> 2;
                            if (num9 < num17)
                            {
                                num16 = num17;
                            }
                        }
                    }
                    numArray[j][k] = num16;
                }
            }
            return numArray;
        }

        private static void calculateThresholdForBlock(byte[] luminances, int subWidth, int subHeight, int width, int height, int[][] blackPoints, BitMatrix matrix)
        {
            for (int i = 0; i < subHeight; i++)
            {
                int yoffset = i << 3;
                int num3 = height - 8;
                if (yoffset > num3)
                {
                    yoffset = num3;
                }
                for (int j = 0; j < subWidth; j++)
                {
                    int xoffset = j << 3;
                    int num6 = width - 8;
                    if (xoffset > num6)
                    {
                        xoffset = num6;
                    }
                    int index = cap(j, 2, subWidth - 3);
                    int num8 = cap(i, 2, subHeight - 3);
                    int num9 = 0;
                    for (int k = -2; k <= 2; k++)
                    {
                        int[] numArray = blackPoints[num8 + k];
                        num9 += numArray[index - 2];
                        num9 += numArray[index - 1];
                        num9 += numArray[index];
                        num9 += numArray[index + 1];
                        num9 += numArray[index + 2];
                    }
                    int threshold = num9 / 0x19;
                    thresholdBlock(luminances, xoffset, yoffset, threshold, width, matrix);
                }
            }
        }

        private static int cap(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            if (value <= max)
            {
                return value;
            }
            return max;
        }

        public override Binarizer createBinarizer(LuminanceSource source)
        {
            return new HybridBinarizer(source);
        }

        private static void thresholdBlock(byte[] luminances, int xoffset, int yoffset, int threshold, int stride, BitMatrix matrix)
        {
            int num = (yoffset * stride) + xoffset;
            int num2 = 0;
            while (num2 < 8)
            {
                for (int i = 0; i < 8; i++)
                {
                    int num4 = luminances[num + i] & 0xff;
                    matrix[xoffset + i, yoffset + num2] = num4 <= threshold;
                }
                num2++;
                num += stride;
            }
        }

        public override BitMatrix BlackMatrix
        {
            get
            {
                this.binarizeEntireImage();
                return this.matrix;
            }
        }
    }
}

