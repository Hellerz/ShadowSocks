namespace ZXing.Common.Detector
{
    using System;

    public static class MathUtils
    {
        public static float distance(int aX, int aY, int bX, int bY)
        {
            int num = aX - bX;
            int num2 = aY - bY;
            return (float) Math.Sqrt((double) ((num * num) + (num2 * num2)));
        }

        public static float distance(float aX, float aY, float bX, float bY)
        {
            float num = aX - bX;
            float num2 = aY - bY;
            return (float) Math.Sqrt((double) ((num * num) + (num2 * num2)));
        }

        public static int round(float d)
        {
            return (int) (d + 0.5f);
        }
    }
}

