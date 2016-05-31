namespace ZXing.Common
{
    using System;
    using System.Runtime.InteropServices;
    using ZXing;

    public class GlobalHistogramBinarizer : Binarizer
    {
        private readonly int[] buckets;
        private static readonly byte[] EMPTY = new byte[0];
        private const int LUMINANCE_BITS = 5;
        private const int LUMINANCE_BUCKETS = 0x20;
        private const int LUMINANCE_SHIFT = 3;
        private byte[] luminances;

        public GlobalHistogramBinarizer(LuminanceSource source) : base(source)
        {
            this.luminances = EMPTY;
            this.buckets = new int[0x20];
        }

        public override Binarizer createBinarizer(LuminanceSource source)
        {
            return new GlobalHistogramBinarizer(source);
        }

        private static bool estimateBlackPoint(int[] buckets, out int blackPoint)
        {
            blackPoint = 0;
            int length = buckets.Length;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            for (int i = 0; i < length; i++)
            {
                if (buckets[i] > num4)
                {
                    num3 = i;
                    num4 = buckets[i];
                }
                if (buckets[i] > num2)
                {
                    num2 = buckets[i];
                }
            }
            int num6 = 0;
            int num7 = 0;
            for (int j = 0; j < length; j++)
            {
                int num9 = j - num3;
                int num10 = (buckets[j] * num9) * num9;
                if (num10 > num7)
                {
                    num6 = j;
                    num7 = num10;
                }
            }
            if (num3 > num6)
            {
                int num11 = num3;
                num3 = num6;
                num6 = num11;
            }
            if ((num6 - num3) <= (length >> 4))
            {
                return false;
            }
            int num12 = num6 - 1;
            int num13 = -1;
            for (int k = num6 - 1; k > num3; k--)
            {
                int num15 = k - num3;
                int num16 = ((num15 * num15) * (num6 - k)) * (num2 - buckets[k]);
                if (num16 > num13)
                {
                    num12 = k;
                    num13 = num16;
                }
            }
            blackPoint = num12 << 3;
            return true;
        }

        public override BitArray getBlackRow(int y, BitArray row)
        {
            int num4;
            LuminanceSource luminanceSource = this.LuminanceSource;
            int width = luminanceSource.Width;
            if ((row == null) || (row.Size < width))
            {
                row = new BitArray(width);
            }
            else
            {
                row.clear();
            }
            this.initArrays(width);
            byte[] buffer = luminanceSource.getRow(y, this.luminances);
            int[] buckets = this.buckets;
            for (int i = 0; i < width; i++)
            {
                int num3 = buffer[i] & 0xff;
                buckets[num3 >> 3]++;
            }
            if (!estimateBlackPoint(buckets, out num4))
            {
                return null;
            }
            int num5 = buffer[0] & 0xff;
            int num6 = buffer[1] & 0xff;
            for (int j = 1; j < (width - 1); j++)
            {
                int num8 = buffer[j + 1] & 0xff;
                int num9 = (((num6 << 2) - num5) - num8) >> 1;
                row[j] = num9 < num4;
                num5 = num6;
                num6 = num8;
            }
            return row;
        }

        private void initArrays(int luminanceSize)
        {
            if (this.luminances.Length < luminanceSize)
            {
                this.luminances = new byte[luminanceSize];
            }
            for (int i = 0; i < 0x20; i++)
            {
                this.buckets[i] = 0;
            }
        }

        public override BitMatrix BlackMatrix
        {
            get
            {
                byte[] buffer;
                int num8;
                LuminanceSource luminanceSource = this.LuminanceSource;
                int width = luminanceSource.Width;
                int height = luminanceSource.Height;
                BitMatrix matrix = new BitMatrix(width, height);
                this.initArrays(width);
                int[] buckets = this.buckets;
                for (int i = 1; i < 5; i++)
                {
                    int y = (height * i) / 5;
                    buffer = luminanceSource.getRow(y, this.luminances);
                    int num5 = (width << 2) / 5;
                    for (int k = width / 5; k < num5; k++)
                    {
                        int num7 = buffer[k] & 0xff;
                        buckets[num7 >> 3]++;
                    }
                }
                if (!estimateBlackPoint(buckets, out num8))
                {
                    return null;
                }
                buffer = luminanceSource.Matrix;
                for (int j = 0; j < height; j++)
                {
                    int num10 = j * width;
                    for (int m = 0; m < width; m++)
                    {
                        int num12 = buffer[num10 + m] & 0xff;
                        matrix[m, j] = num12 < num8;
                    }
                }
                return matrix;
            }
        }
    }
}

