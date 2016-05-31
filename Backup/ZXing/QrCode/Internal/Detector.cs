namespace ZXing.QrCode.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using ZXing;
    using ZXing.Common;
    using ZXing.Common.Detector;

    public class Detector
    {
        private readonly BitMatrix image;
        private ZXing.ResultPointCallback resultPointCallback;

        public Detector(BitMatrix image)
        {
            this.image = image;
        }

        protected internal virtual float calculateModuleSize(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft)
        {
            return ((this.calculateModuleSizeOneWay(topLeft, topRight) + this.calculateModuleSizeOneWay(topLeft, bottomLeft)) / 2f);
        }

        private float calculateModuleSizeOneWay(ResultPoint pattern, ResultPoint otherPattern)
        {
            float f = this.sizeOfBlackWhiteBlackRunBothWays((int) pattern.X, (int) pattern.Y, (int) otherPattern.X, (int) otherPattern.Y);
            float num2 = this.sizeOfBlackWhiteBlackRunBothWays((int) otherPattern.X, (int) otherPattern.Y, (int) pattern.X, (int) pattern.Y);
            if (float.IsNaN(f))
            {
                return (num2 / 7f);
            }
            if (float.IsNaN(num2))
            {
                return (f / 7f);
            }
            return ((f + num2) / 14f);
        }

        private static bool computeDimension(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft, float moduleSize, out int dimension)
        {
            int num = MathUtils.round(ResultPoint.distance(topLeft, topRight) / moduleSize);
            int num2 = MathUtils.round(ResultPoint.distance(topLeft, bottomLeft) / moduleSize);
            dimension = ((num + num2) >> 1) + 7;
            switch ((dimension & 3))
            {
                case 0:
                    dimension++;
                    break;

                case 2:
                    dimension--;
                    break;

                case 3:
                    return true;
            }
            return true;
        }

        private static PerspectiveTransform createTransform(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft, ResultPoint alignmentPattern, int dimension)
        {
            float x;
            float y;
            float num4;
            float num5;
            float num = dimension - 3.5f;
            if (alignmentPattern != null)
            {
                x = alignmentPattern.X;
                y = alignmentPattern.Y;
                num4 = num5 = num - 3f;
            }
            else
            {
                x = (topRight.X - topLeft.X) + bottomLeft.X;
                y = (topRight.Y - topLeft.Y) + bottomLeft.Y;
                num4 = num5 = num;
            }
            return PerspectiveTransform.quadrilateralToQuadrilateral(3.5f, 3.5f, num, 3.5f, num4, num5, 3.5f, num, topLeft.X, topLeft.Y, topRight.X, topRight.Y, x, y, bottomLeft.X, bottomLeft.Y);
        }

        public virtual DetectorResult detect()
        {
            return this.detect(null);
        }

        public virtual DetectorResult detect(IDictionary<DecodeHintType, object> hints)
        {
            this.resultPointCallback = ((hints == null) || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK)) ? null : ((ZXing.ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK]);
            FinderPatternInfo info = new FinderPatternFinder(this.image, this.resultPointCallback).find(hints);
            if (info == null)
            {
                return null;
            }
            return this.processFinderPatternInfo(info);
        }

        protected AlignmentPattern findAlignmentInRegion(float overallEstModuleSize, int estAlignmentX, int estAlignmentY, float allowanceFactor)
        {
            int num = (int) (allowanceFactor * overallEstModuleSize);
            int startX = Math.Max(0, estAlignmentX - num);
            int num3 = Math.Min((int) (this.image.Width - 1), (int) (estAlignmentX + num));
            if ((num3 - startX) < (overallEstModuleSize * 3f))
            {
                return null;
            }
            int startY = Math.Max(0, estAlignmentY - num);
            int num5 = Math.Min((int) (this.image.Height - 1), (int) (estAlignmentY + num));
            AlignmentPatternFinder finder = new AlignmentPatternFinder(this.image, startX, startY, num3 - startX, num5 - startY, overallEstModuleSize, this.resultPointCallback);
            return finder.find();
        }

        protected internal virtual DetectorResult processFinderPatternInfo(FinderPatternInfo info)
        {
            int num2;
            ResultPoint[] pointArray;
            FinderPattern topLeft = info.TopLeft;
            FinderPattern topRight = info.TopRight;
            FinderPattern bottomLeft = info.BottomLeft;
            float moduleSize = this.calculateModuleSize(topLeft, topRight, bottomLeft);
            if (moduleSize < 1f)
            {
                return null;
            }
            if (!computeDimension(topLeft, topRight, bottomLeft, moduleSize, out num2))
            {
                return null;
            }
            Version version = Version.getProvisionalVersionForDimension(num2);
            if (version == null)
            {
                return null;
            }
            int num3 = version.DimensionForVersion - 7;
            AlignmentPattern alignmentPattern = null;
            if (version.AlignmentPatternCenters.Length > 0)
            {
                float num4 = (topRight.X - topLeft.X) + bottomLeft.X;
                float num5 = (topRight.Y - topLeft.Y) + bottomLeft.Y;
                float num6 = 1f - (3f / ((float) num3));
                int estAlignmentX = (int) (topLeft.X + (num6 * (num4 - topLeft.X)));
                int estAlignmentY = (int) (topLeft.Y + (num6 * (num5 - topLeft.Y)));
                for (int i = 4; i <= 0x10; i = i << 1)
                {
                    alignmentPattern = this.findAlignmentInRegion(moduleSize, estAlignmentX, estAlignmentY, (float) i);
                    if (alignmentPattern != null)
                    {
                        break;
                    }
                }
            }
            PerspectiveTransform transform = createTransform(topLeft, topRight, bottomLeft, alignmentPattern, num2);
            BitMatrix bits = sampleGrid(this.image, transform, num2);
            if (bits == null)
            {
                return null;
            }
            if (alignmentPattern == null)
            {
                pointArray = new ResultPoint[] { bottomLeft, topLeft, topRight };
            }
            else
            {
                pointArray = new ResultPoint[] { bottomLeft, topLeft, topRight, alignmentPattern };
            }
            return new DetectorResult(bits, pointArray);
        }

        private static BitMatrix sampleGrid(BitMatrix image, PerspectiveTransform transform, int dimension)
        {
            return GridSampler.Instance.sampleGrid(image, dimension, dimension, transform);
        }

        private float sizeOfBlackWhiteBlackRun(int fromX, int fromY, int toX, int toY)
        {
            bool flag = Math.Abs((int) (toY - fromY)) > Math.Abs((int) (toX - fromX));
            if (flag)
            {
                int num = fromX;
                fromX = fromY;
                fromY = num;
                num = toX;
                toX = toY;
                toY = num;
            }
            int num2 = Math.Abs((int) (toX - fromX));
            int num3 = Math.Abs((int) (toY - fromY));
            int num4 = -num2 >> 1;
            int num5 = (fromX < toX) ? 1 : -1;
            int num6 = (fromY < toY) ? 1 : -1;
            int num7 = 0;
            int num8 = toX + num5;
            int aX = fromX;
            int aY = fromY;
            while (aX != num8)
            {
                int num11 = flag ? aY : aX;
                int num12 = flag ? aX : aY;
                if ((num7 == 1) == this.image[num11, num12])
                {
                    if (num7 == 2)
                    {
                        return MathUtils.distance(aX, aY, fromX, fromY);
                    }
                    num7++;
                }
                num4 += num3;
                if (num4 > 0)
                {
                    if (aY == toY)
                    {
                        break;
                    }
                    aY += num6;
                    num4 -= num2;
                }
                aX += num5;
            }
            if (num7 == 2)
            {
                return MathUtils.distance(toX + num5, toY, fromX, fromY);
            }
            return float.NaN;
        }

        private float sizeOfBlackWhiteBlackRunBothWays(int fromX, int fromY, int toX, int toY)
        {
            float num = this.sizeOfBlackWhiteBlackRun(fromX, fromY, toX, toY);
            float num2 = 1f;
            int num3 = fromX - (toX - fromX);
            if (num3 < 0)
            {
                num2 = ((float) fromX) / ((float) (fromX - num3));
                num3 = 0;
            }
            else if (num3 >= this.image.Width)
            {
                num2 = ((float) ((this.image.Width - 1) - fromX)) / ((float) (num3 - fromX));
                num3 = this.image.Width - 1;
            }
            int num4 = fromY - ((int) ((toY - fromY) * num2));
            num2 = 1f;
            if (num4 < 0)
            {
                num2 = ((float) fromY) / ((float) (fromY - num4));
                num4 = 0;
            }
            else if (num4 >= this.image.Height)
            {
                num2 = ((float) ((this.image.Height - 1) - fromY)) / ((float) (num4 - fromY));
                num4 = this.image.Height - 1;
            }
            num3 = fromX + ((int) ((num3 - fromX) * num2));
            num += this.sizeOfBlackWhiteBlackRun(fromX, fromY, num3, num4);
            return (num - 1f);
        }

        protected internal virtual BitMatrix Image
        {
            get
            {
                return this.image;
            }
        }

        protected internal virtual ZXing.ResultPointCallback ResultPointCallback
        {
            get
            {
                return this.resultPointCallback;
            }
        }
    }
}

