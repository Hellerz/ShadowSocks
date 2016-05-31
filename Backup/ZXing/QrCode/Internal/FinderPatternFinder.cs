namespace ZXing.QrCode.Internal
{
    using System;
    using System.Collections.Generic;
    using ZXing;
    using ZXing.Common;

    public class FinderPatternFinder
    {
        private const int CENTER_QUORUM = 2;
        private readonly int[] crossCheckStateCount;
        private bool hasSkipped;
        private readonly BitMatrix image;
        private const int INTEGER_MATH_SHIFT = 8;
        protected internal const int MAX_MODULES = 0x39;
        protected internal const int MIN_SKIP = 3;
        private List<FinderPattern> possibleCenters;
        private readonly ResultPointCallback resultPointCallback;

        public FinderPatternFinder(BitMatrix image) : this(image, null)
        {
        }

        public FinderPatternFinder(BitMatrix image, ResultPointCallback resultPointCallback)
        {
            this.image = image;
            this.possibleCenters = new List<FinderPattern>();
            this.crossCheckStateCount = new int[5];
            this.resultPointCallback = resultPointCallback;
        }

        private static float? centerFromEnd(int[] stateCount, int end)
        {
            float f = ((end - stateCount[4]) - stateCount[3]) - (((float) stateCount[2]) / 2f);
            if (float.IsNaN(f))
            {
                return null;
            }
            return new float?(f);
        }

        private bool crossCheckDiagonal(int startI, int centerJ, int maxCount, int originalStateCountTotal)
        {
            int height = this.image.Height;
            int width = this.image.Width;
            int[] crossCheckStateCount = this.CrossCheckStateCount;
            int num3 = 0;
            while (((startI - num3) >= 0) && this.image[centerJ - num3, startI - num3])
            {
                crossCheckStateCount[2]++;
                num3++;
            }
            if (((startI - num3) >= 0) && ((centerJ - num3) >= 0))
            {
                while ((((startI - num3) >= 0) && ((centerJ - num3) >= 0)) && (!this.image[centerJ - num3, startI - num3] && (crossCheckStateCount[1] <= maxCount)))
                {
                    crossCheckStateCount[1]++;
                    num3++;
                }
                if ((((startI - num3) >= 0) && ((centerJ - num3) >= 0)) && (crossCheckStateCount[1] <= maxCount))
                {
                    while ((((startI - num3) >= 0) && ((centerJ - num3) >= 0)) && (this.image[centerJ - num3, startI - num3] && (crossCheckStateCount[0] <= maxCount)))
                    {
                        crossCheckStateCount[0]++;
                        num3++;
                    }
                    if (crossCheckStateCount[0] <= maxCount)
                    {
                        num3 = 1;
                        while ((((startI + num3) < height) && ((centerJ + num3) < width)) && this.image[centerJ + num3, startI + num3])
                        {
                            crossCheckStateCount[2]++;
                            num3++;
                        }
                        if (((startI + num3) < height) && ((centerJ + num3) < width))
                        {
                            while ((((startI + num3) < height) && ((centerJ + num3) < width)) && (!this.image[centerJ + num3, startI + num3] && (crossCheckStateCount[3] < maxCount)))
                            {
                                crossCheckStateCount[3]++;
                                num3++;
                            }
                            if ((((startI + num3) < height) && ((centerJ + num3) < width)) && (crossCheckStateCount[3] < maxCount))
                            {
                                while ((((startI + num3) < height) && ((centerJ + num3) < width)) && (this.image[centerJ + num3, startI + num3] && (crossCheckStateCount[4] < maxCount)))
                                {
                                    crossCheckStateCount[4]++;
                                    num3++;
                                }
                                if (crossCheckStateCount[4] >= maxCount)
                                {
                                    return false;
                                }
                                int num4 = (((crossCheckStateCount[0] + crossCheckStateCount[1]) + crossCheckStateCount[2]) + crossCheckStateCount[3]) + crossCheckStateCount[4];
                                return ((Math.Abs((int) (num4 - originalStateCountTotal)) < (2 * originalStateCountTotal)) && foundPatternCross(crossCheckStateCount));
                            }
                            return false;
                        }
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

        private float? crossCheckHorizontal(int startJ, int centerI, int maxCount, int originalStateCountTotal)
        {
            int width = this.image.Width;
            int[] crossCheckStateCount = this.CrossCheckStateCount;
            int end = startJ;
            while ((end >= 0) && this.image[end, centerI])
            {
                crossCheckStateCount[2]++;
                end--;
            }
            if (end >= 0)
            {
                while (((end >= 0) && !this.image[end, centerI]) && (crossCheckStateCount[1] <= maxCount))
                {
                    crossCheckStateCount[1]++;
                    end--;
                }
                if ((end >= 0) && (crossCheckStateCount[1] <= maxCount))
                {
                    while (((end >= 0) && this.image[end, centerI]) && (crossCheckStateCount[0] <= maxCount))
                    {
                        crossCheckStateCount[0]++;
                        end--;
                    }
                    if (crossCheckStateCount[0] <= maxCount)
                    {
                        end = startJ + 1;
                        while ((end < width) && this.image[end, centerI])
                        {
                            crossCheckStateCount[2]++;
                            end++;
                        }
                        if (end != width)
                        {
                            while (((end < width) && !this.image[end, centerI]) && (crossCheckStateCount[3] < maxCount))
                            {
                                crossCheckStateCount[3]++;
                                end++;
                            }
                            if ((end != width) && (crossCheckStateCount[3] < maxCount))
                            {
                                while (((end < width) && this.image[end, centerI]) && (crossCheckStateCount[4] < maxCount))
                                {
                                    crossCheckStateCount[4]++;
                                    end++;
                                }
                                if (crossCheckStateCount[4] >= maxCount)
                                {
                                    return null;
                                }
                                int num3 = (((crossCheckStateCount[0] + crossCheckStateCount[1]) + crossCheckStateCount[2]) + crossCheckStateCount[3]) + crossCheckStateCount[4];
                                if ((5 * Math.Abs((int) (num3 - originalStateCountTotal))) >= originalStateCountTotal)
                                {
                                    return null;
                                }
                                if (!foundPatternCross(crossCheckStateCount))
                                {
                                    return null;
                                }
                                return centerFromEnd(crossCheckStateCount, end);
                            }
                            return null;
                        }
                    }
                    return null;
                }
                return null;
            }
            return null;
        }

        private float? crossCheckVertical(int startI, int centerJ, int maxCount, int originalStateCountTotal)
        {
            int height = this.image.Height;
            int[] crossCheckStateCount = this.CrossCheckStateCount;
            int end = startI;
            while ((end >= 0) && this.image[centerJ, end])
            {
                crossCheckStateCount[2]++;
                end--;
            }
            if (end >= 0)
            {
                while (((end >= 0) && !this.image[centerJ, end]) && (crossCheckStateCount[1] <= maxCount))
                {
                    crossCheckStateCount[1]++;
                    end--;
                }
                if ((end >= 0) && (crossCheckStateCount[1] <= maxCount))
                {
                    while (((end >= 0) && this.image[centerJ, end]) && (crossCheckStateCount[0] <= maxCount))
                    {
                        crossCheckStateCount[0]++;
                        end--;
                    }
                    if (crossCheckStateCount[0] <= maxCount)
                    {
                        end = startI + 1;
                        while ((end < height) && this.image[centerJ, end])
                        {
                            crossCheckStateCount[2]++;
                            end++;
                        }
                        if (end != height)
                        {
                            while (((end < height) && !this.image[centerJ, end]) && (crossCheckStateCount[3] < maxCount))
                            {
                                crossCheckStateCount[3]++;
                                end++;
                            }
                            if ((end != height) && (crossCheckStateCount[3] < maxCount))
                            {
                                while (((end < height) && this.image[centerJ, end]) && (crossCheckStateCount[4] < maxCount))
                                {
                                    crossCheckStateCount[4]++;
                                    end++;
                                }
                                if (crossCheckStateCount[4] >= maxCount)
                                {
                                    return null;
                                }
                                int num3 = (((crossCheckStateCount[0] + crossCheckStateCount[1]) + crossCheckStateCount[2]) + crossCheckStateCount[3]) + crossCheckStateCount[4];
                                if ((5 * Math.Abs((int) (num3 - originalStateCountTotal))) >= (2 * originalStateCountTotal))
                                {
                                    return null;
                                }
                                if (!foundPatternCross(crossCheckStateCount))
                                {
                                    return null;
                                }
                                return centerFromEnd(crossCheckStateCount, end);
                            }
                            return null;
                        }
                    }
                    return null;
                }
                return null;
            }
            return null;
        }

        internal virtual FinderPatternInfo find(IDictionary<DecodeHintType, object> hints)
        {
            bool flag = (hints != null) && hints.ContainsKey(DecodeHintType.TRY_HARDER);
            bool pureBarcode = (hints != null) && hints.ContainsKey(DecodeHintType.PURE_BARCODE);
            int height = this.image.Height;
            int width = this.image.Width;
            int num3 = (3 * height) / 0xe4;
            if ((num3 < 3) || flag)
            {
                num3 = 3;
            }
            bool flag3 = false;
            int[] stateCount = new int[5];
            for (int i = num3 - 1; (i < height) && !flag3; i += num3)
            {
                stateCount[0] = 0;
                stateCount[1] = 0;
                stateCount[2] = 0;
                stateCount[3] = 0;
                stateCount[4] = 0;
                int index = 0;
                for (int j = 0; j < width; j++)
                {
                    if (this.image[j, i])
                    {
                        if ((index & 1) == 1)
                        {
                            index++;
                        }
                        stateCount[index]++;
                    }
                    else if ((index & 1) == 0)
                    {
                        if (index == 4)
                        {
                            if (foundPatternCross(stateCount))
                            {
                                if (this.handlePossibleCenter(stateCount, i, j, pureBarcode))
                                {
                                    num3 = 2;
                                    if (this.hasSkipped)
                                    {
                                        flag3 = this.haveMultiplyConfirmedCenters();
                                    }
                                    else
                                    {
                                        int num7 = this.findRowSkip();
                                        if (num7 > stateCount[2])
                                        {
                                            i += (num7 - stateCount[2]) - num3;
                                            j = width - 1;
                                        }
                                    }
                                }
                                else
                                {
                                    stateCount[0] = stateCount[2];
                                    stateCount[1] = stateCount[3];
                                    stateCount[2] = stateCount[4];
                                    stateCount[3] = 1;
                                    stateCount[4] = 0;
                                    index = 3;
                                    goto Label_01C7;
                                }
                                index = 0;
                                stateCount[0] = 0;
                                stateCount[1] = 0;
                                stateCount[2] = 0;
                                stateCount[3] = 0;
                                stateCount[4] = 0;
                            Label_01C7:;
                            }
                            else
                            {
                                stateCount[0] = stateCount[2];
                                stateCount[1] = stateCount[3];
                                stateCount[2] = stateCount[4];
                                stateCount[3] = 1;
                                stateCount[4] = 0;
                                index = 3;
                            }
                        }
                        else
                        {
                            stateCount[++index]++;
                        }
                    }
                    else
                    {
                        stateCount[index]++;
                    }
                }
                if (foundPatternCross(stateCount) && this.handlePossibleCenter(stateCount, i, width, pureBarcode))
                {
                    num3 = stateCount[0];
                    if (this.hasSkipped)
                    {
                        flag3 = this.haveMultiplyConfirmedCenters();
                    }
                }
            }
            FinderPattern[] patterns = this.selectBestPatterns();
            if (patterns == null)
            {
                return null;
            }
            ResultPoint.orderBestPatterns(patterns);
            return new FinderPatternInfo(patterns);
        }

        private int findRowSkip()
        {
            if (this.possibleCenters.Count > 1)
            {
                ResultPoint point = null;
                foreach (FinderPattern pattern in this.possibleCenters)
                {
                    if (pattern.Count >= 2)
                    {
                        if (point == null)
                        {
                            point = pattern;
                        }
                        else
                        {
                            this.hasSkipped = true;
                            return (((int) (Math.Abs((float) (point.X - pattern.X)) - Math.Abs((float) (point.Y - pattern.Y)))) / 2);
                        }
                    }
                }
            }
            return 0;
        }

        protected internal static bool foundPatternCross(int[] stateCount)
        {
            int num = 0;
            for (int i = 0; i < 5; i++)
            {
                int num3 = stateCount[i];
                if (num3 == 0)
                {
                    return false;
                }
                num += num3;
            }
            if (num < 7)
            {
                return false;
            }
            int num4 = (num << 8) / 7;
            int num5 = num4 / 2;
            return ((((Math.Abs((int) (num4 - (stateCount[0] << 8))) < num5) && (Math.Abs((int) (num4 - (stateCount[1] << 8))) < num5)) && ((Math.Abs((int) ((3 * num4) - (stateCount[2] << 8))) < (3 * num5)) && (Math.Abs((int) (num4 - (stateCount[3] << 8))) < num5))) && (Math.Abs((int) (num4 - (stateCount[4] << 8))) < num5));
        }

        protected bool handlePossibleCenter(int[] stateCount, int i, int j, bool pureBarcode)
        {
            int originalStateCountTotal = (((stateCount[0] + stateCount[1]) + stateCount[2]) + stateCount[3]) + stateCount[4];
            float? nullable = centerFromEnd(stateCount, j);
            if (nullable.HasValue)
            {
                float? nullable2 = this.crossCheckVertical(i, (int) nullable.Value, stateCount[2], originalStateCountTotal);
                if (nullable2.HasValue)
                {
                    nullable = this.crossCheckHorizontal((int) nullable.Value, (int) nullable2.Value, stateCount[2], originalStateCountTotal);
                    if (nullable.HasValue && (!pureBarcode || this.crossCheckDiagonal((int) nullable2.Value, (int) nullable.Value, stateCount[2], originalStateCountTotal)))
                    {
                        float moduleSize = ((float) originalStateCountTotal) / 7f;
                        bool flag = false;
                        for (int k = 0; k < this.possibleCenters.Count; k++)
                        {
                            FinderPattern pattern = this.possibleCenters[k];
                            if (pattern.aboutEquals(moduleSize, nullable2.Value, nullable.Value))
                            {
                                this.possibleCenters.RemoveAt(k);
                                this.possibleCenters.Insert(k, pattern.combineEstimate(nullable2.Value, nullable.Value, moduleSize));
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            FinderPattern item = new FinderPattern(nullable.Value, nullable2.Value, moduleSize);
                            this.possibleCenters.Add(item);
                            if (this.resultPointCallback != null)
                            {
                                this.resultPointCallback(item);
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private bool haveMultiplyConfirmedCenters()
        {
            int num = 0;
            float num2 = 0f;
            int count = this.possibleCenters.Count;
            foreach (FinderPattern pattern in this.possibleCenters)
            {
                if (pattern.Count >= 2)
                {
                    num++;
                    num2 += pattern.EstimatedModuleSize;
                }
            }
            if (num < 3)
            {
                return false;
            }
            float num4 = num2 / ((float) count);
            float num5 = 0f;
            for (int i = 0; i < count; i++)
            {
                FinderPattern pattern2 = this.possibleCenters[i];
                num5 += Math.Abs((float) (pattern2.EstimatedModuleSize - num4));
            }
            return (num5 <= (0.05f * num2));
        }

        private FinderPattern[] selectBestPatterns()
        {
            int count = this.possibleCenters.Count;
            if (count < 3)
            {
                return null;
            }
            if (count > 3)
            {
                float num2 = 0f;
                float num3 = 0f;
                foreach (FinderPattern pattern in this.possibleCenters)
                {
                    float estimatedModuleSize = pattern.EstimatedModuleSize;
                    num2 += estimatedModuleSize;
                    num3 += estimatedModuleSize * estimatedModuleSize;
                }
                float f = num2 / ((float) count);
                float num6 = (float) Math.Sqrt((double) ((num3 / ((float) count)) - (f * f)));
                this.possibleCenters.Sort(new FurthestFromAverageComparator(f));
                float num7 = Math.Max(0.2f * f, num6);
                for (int i = 0; (i < this.possibleCenters.Count) && (this.possibleCenters.Count > 3); i++)
                {
                    FinderPattern pattern2 = this.possibleCenters[i];
                    if (Math.Abs((float) (pattern2.EstimatedModuleSize - f)) > num7)
                    {
                        this.possibleCenters.RemoveAt(i);
                        i--;
                    }
                }
            }
            if (this.possibleCenters.Count > 3)
            {
                float num9 = 0f;
                foreach (FinderPattern pattern3 in this.possibleCenters)
                {
                    num9 += pattern3.EstimatedModuleSize;
                }
                float num10 = num9 / ((float) this.possibleCenters.Count);
                this.possibleCenters.Sort(new CenterComparator(num10));
                this.possibleCenters = this.possibleCenters.GetRange(0, 3);
            }
            return new FinderPattern[] { this.possibleCenters[0], this.possibleCenters[1], this.possibleCenters[2] };
        }

        private int[] CrossCheckStateCount
        {
            get
            {
                this.crossCheckStateCount[0] = 0;
                this.crossCheckStateCount[1] = 0;
                this.crossCheckStateCount[2] = 0;
                this.crossCheckStateCount[3] = 0;
                this.crossCheckStateCount[4] = 0;
                return this.crossCheckStateCount;
            }
        }

        protected internal virtual BitMatrix Image
        {
            get
            {
                return this.image;
            }
        }

        protected internal virtual List<FinderPattern> PossibleCenters
        {
            get
            {
                return this.possibleCenters;
            }
        }

        private sealed class CenterComparator : IComparer<FinderPattern>
        {
            private readonly float average;

            public CenterComparator(float f)
            {
                this.average = f;
            }

            public int Compare(FinderPattern x, FinderPattern y)
            {
                if (y.Count != x.Count)
                {
                    return (y.Count - x.Count);
                }
                float num = Math.Abs((float) (y.EstimatedModuleSize - this.average));
                float num2 = Math.Abs((float) (x.EstimatedModuleSize - this.average));
                if (num < num2)
                {
                    return 1;
                }
                if (num != num2)
                {
                    return -1;
                }
                return 0;
            }
        }

        private sealed class FurthestFromAverageComparator : IComparer<FinderPattern>
        {
            private readonly float average;

            public FurthestFromAverageComparator(float f)
            {
                this.average = f;
            }

            public int Compare(FinderPattern x, FinderPattern y)
            {
                float num = Math.Abs((float) (y.EstimatedModuleSize - this.average));
                float num2 = Math.Abs((float) (x.EstimatedModuleSize - this.average));
                if (num < num2)
                {
                    return -1;
                }
                if (num != num2)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}

