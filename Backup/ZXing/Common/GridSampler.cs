namespace ZXing.Common
{
    using System;

    public abstract class GridSampler
    {
        private static GridSampler gridSampler = new DefaultGridSampler();

        protected GridSampler()
        {
        }

        protected internal static bool checkAndNudgePoints(BitMatrix image, float[] points)
        {
            int width = image.Width;
            int height = image.Height;
            bool flag = true;
            for (int i = 0; (i < points.Length) && flag; i += 2)
            {
                int num4 = (int) points[i];
                int num5 = (int) points[i + 1];
                if (((num4 < -1) || (num4 > width)) || ((num5 < -1) || (num5 > height)))
                {
                    return false;
                }
                flag = false;
                if (num4 == -1)
                {
                    points[i] = 0f;
                    flag = true;
                }
                else if (num4 == width)
                {
                    points[i] = width - 1;
                    flag = true;
                }
                if (num5 == -1)
                {
                    points[i + 1] = 0f;
                    flag = true;
                }
                else if (num5 == height)
                {
                    points[i + 1] = height - 1;
                    flag = true;
                }
            }
            flag = true;
            for (int j = points.Length - 2; (j >= 0) && flag; j -= 2)
            {
                int num7 = (int) points[j];
                int num8 = (int) points[j + 1];
                if (((num7 < -1) || (num7 > width)) || ((num8 < -1) || (num8 > height)))
                {
                    return false;
                }
                flag = false;
                if (num7 == -1)
                {
                    points[j] = 0f;
                    flag = true;
                }
                else if (num7 == width)
                {
                    points[j] = width - 1;
                    flag = true;
                }
                if (num8 == -1)
                {
                    points[j + 1] = 0f;
                    flag = true;
                }
                else if (num8 == height)
                {
                    points[j + 1] = height - 1;
                    flag = true;
                }
            }
            return true;
        }

        public virtual BitMatrix sampleGrid(BitMatrix image, int dimensionX, int dimensionY, PerspectiveTransform transform)
        {
            throw new NotSupportedException();
        }

        public abstract BitMatrix sampleGrid(BitMatrix image, int dimensionX, int dimensionY, float p1ToX, float p1ToY, float p2ToX, float p2ToY, float p3ToX, float p3ToY, float p4ToX, float p4ToY, float p1FromX, float p1FromY, float p2FromX, float p2FromY, float p3FromX, float p3FromY, float p4FromX, float p4FromY);
        public static void setGridSampler(GridSampler newGridSampler)
        {
            if (newGridSampler == null)
            {
                throw new ArgumentException();
            }
            gridSampler = newGridSampler;
        }

        public static GridSampler Instance
        {
            get
            {
                return gridSampler;
            }
        }
    }
}

