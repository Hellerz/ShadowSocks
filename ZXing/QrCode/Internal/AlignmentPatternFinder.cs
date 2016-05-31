namespace ZXing.QrCode.Internal
{
    using System;
    using System.Collections.Generic;
    using ZXing;
    using ZXing.Common;

    internal sealed class AlignmentPatternFinder
    {
        private readonly int[] crossCheckStateCount;
        private readonly int height;
        private readonly BitMatrix image;
        private readonly float moduleSize;
        private readonly IList<AlignmentPattern> possibleCenters;
        private readonly ResultPointCallback resultPointCallback;
        private readonly int startX;
        private readonly int startY;
        private readonly int width;

        internal AlignmentPatternFinder(BitMatrix image, int startX, int startY, int width, int height, float moduleSize, ResultPointCallback resultPointCallback)
        {
            this.image = image;
            this.possibleCenters = new List<AlignmentPattern>(5);
            this.startX = startX;
            this.startY = startY;
            this.width = width;
            this.height = height;
            this.moduleSize = moduleSize;
            this.crossCheckStateCount = new int[3];
            this.resultPointCallback = resultPointCallback;
        }

        private static float? centerFromEnd(int[] stateCount, int end)
        {
            float f = (end - stateCount[2]) - (((float) stateCount[1]) / 2f);
            if (float.IsNaN(f))
            {
                return null;
            }
            return new float?(f);
        }

        private float? crossCheckVertical(int startI, int centerJ, int maxCount, int originalStateCountTotal)
        {
            int height = this.image.Height;
            int[] crossCheckStateCount = this.crossCheckStateCount;
            crossCheckStateCount[0] = 0;
            crossCheckStateCount[1] = 0;
            crossCheckStateCount[2] = 0;
            int end = startI;
            while (((end >= 0) && this.image[centerJ, end]) && (crossCheckStateCount[1] <= maxCount))
            {
                crossCheckStateCount[1]++;
                end--;
            }
            if ((end >= 0) && (crossCheckStateCount[1] <= maxCount))
            {
                while (((end >= 0) && !this.image[centerJ, end]) && (crossCheckStateCount[0] <= maxCount))
                {
                    crossCheckStateCount[0]++;
                    end--;
                }
                if (crossCheckStateCount[0] <= maxCount)
                {
                    end = startI + 1;
                    while (((end < height) && this.image[centerJ, end]) && (crossCheckStateCount[1] <= maxCount))
                    {
                        crossCheckStateCount[1]++;
                        end++;
                    }
                    if ((end != height) && (crossCheckStateCount[1] <= maxCount))
                    {
                        while (((end < height) && !this.image[centerJ, end]) && (crossCheckStateCount[2] <= maxCount))
                        {
                            crossCheckStateCount[2]++;
                            end++;
                        }
                        if (crossCheckStateCount[2] > maxCount)
                        {
                            return null;
                        }
                        int num3 = (crossCheckStateCount[0] + crossCheckStateCount[1]) + crossCheckStateCount[2];
                        if ((5 * Math.Abs((int) (num3 - originalStateCountTotal))) >= (2 * originalStateCountTotal))
                        {
                            return null;
                        }
                        if (!this.foundPatternCross(crossCheckStateCount))
                        {
                            return null;
                        }
                        return centerFromEnd(crossCheckStateCount, end);
                    }
                }
                return null;
            }
            return null;
        }

        internal AlignmentPattern find()
        {
            int startX = this.startX;
            int height = this.height;
            int j = startX + this.width;
            int num4 = this.startY + (height >> 1);
            int[] stateCount = new int[3];
            for (int i = 0; i < height; i++)
            {
                int num6 = num4 + (((i & 1) == 0) ? ((i + 1) >> 1) : -((i + 1) >> 1));
                stateCount[0] = 0;
                stateCount[1] = 0;
                stateCount[2] = 0;
                int num7 = startX;
                while ((num7 < j) && !this.image[num7, num6])
                {
                    num7++;
                }
                int index = 0;
                while (num7 < j)
                {
                    if (this.image[num7, num6])
                    {
                        switch (index)
                        {
                            case 1:
                                stateCount[index]++;
                                goto Label_012A;

                            case 2:
                                if (this.foundPatternCross(stateCount))
                                {
                                    AlignmentPattern pattern = this.handlePossibleCenter(stateCount, num6, num7);
                                    if (pattern != null)
                                    {
                                        return pattern;
                                    }
                                }
                                stateCount[0] = stateCount[2];
                                stateCount[1] = 1;
                                stateCount[2] = 0;
                                index = 1;
                                goto Label_012A;
                        }
                        stateCount[++index]++;
                    }
                    else
                    {
                        if (index == 1)
                        {
                            index++;
                        }
                        stateCount[index]++;
                    }
                Label_012A:
                    num7++;
                }
                if (this.foundPatternCross(stateCount))
                {
                    AlignmentPattern pattern2 = this.handlePossibleCenter(stateCount, num6, j);
                    if (pattern2 != null)
                    {
                        return pattern2;
                    }
                }
            }
            if (this.possibleCenters.Count != 0)
            {
                return this.possibleCenters[0];
            }
            return null;
        }

        private bool foundPatternCross(int[] stateCount)
        {
            float num = this.moduleSize / 2f;
            for (int i = 0; i < 3; i++)
            {
                if (Math.Abs((float) (this.moduleSize - stateCount[i])) >= num)
                {
                    return false;
                }
            }
            return true;
        }

        private AlignmentPattern handlePossibleCenter(int[] stateCount, int i, int j)
        {
            int originalStateCountTotal = (stateCount[0] + stateCount[1]) + stateCount[2];
            float? nullable = centerFromEnd(stateCount, j);
            if (nullable.HasValue)
            {
                float? nullable2 = this.crossCheckVertical(i, (int) nullable.Value, 2 * stateCount[1], originalStateCountTotal);
                if (nullable2.HasValue)
                {
                    float moduleSize = ((float) ((stateCount[0] + stateCount[1]) + stateCount[2])) / 3f;
                    foreach (AlignmentPattern pattern in this.possibleCenters)
                    {
                        if (pattern.aboutEquals(moduleSize, nullable2.Value, nullable.Value))
                        {
                            return pattern.combineEstimate(nullable2.Value, nullable.Value, moduleSize);
                        }
                    }
                    AlignmentPattern item = new AlignmentPattern(nullable.Value, nullable2.Value, moduleSize);
                    this.possibleCenters.Add(item);
                    if (this.resultPointCallback != null)
                    {
                        this.resultPointCallback(item);
                    }
                }
            }
            return null;
        }
    }
}

