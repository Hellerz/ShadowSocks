namespace ZXing.Common
{
    using System;

    public sealed class PerspectiveTransform
    {
        private float a11;
        private float a12;
        private float a13;
        private float a21;
        private float a22;
        private float a23;
        private float a31;
        private float a32;
        private float a33;

        private PerspectiveTransform(float a11, float a21, float a31, float a12, float a22, float a32, float a13, float a23, float a33)
        {
            this.a11 = a11;
            this.a12 = a12;
            this.a13 = a13;
            this.a21 = a21;
            this.a22 = a22;
            this.a23 = a23;
            this.a31 = a31;
            this.a32 = a32;
            this.a33 = a33;
        }

        internal PerspectiveTransform buildAdjoint()
        {
            return new PerspectiveTransform((this.a22 * this.a33) - (this.a23 * this.a32), (this.a23 * this.a31) - (this.a21 * this.a33), (this.a21 * this.a32) - (this.a22 * this.a31), (this.a13 * this.a32) - (this.a12 * this.a33), (this.a11 * this.a33) - (this.a13 * this.a31), (this.a12 * this.a31) - (this.a11 * this.a32), (this.a12 * this.a23) - (this.a13 * this.a22), (this.a13 * this.a21) - (this.a11 * this.a23), (this.a11 * this.a22) - (this.a12 * this.a21));
        }

        public static PerspectiveTransform quadrilateralToQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, float x0p, float y0p, float x1p, float y1p, float x2p, float y2p, float x3p, float y3p)
        {
            PerspectiveTransform other = quadrilateralToSquare(x0, y0, x1, y1, x2, y2, x3, y3);
            return squareToQuadrilateral(x0p, y0p, x1p, y1p, x2p, y2p, x3p, y3p).times(other);
        }

        public static PerspectiveTransform quadrilateralToSquare(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            return squareToQuadrilateral(x0, y0, x1, y1, x2, y2, x3, y3).buildAdjoint();
        }

        public static PerspectiveTransform squareToQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float num = ((x0 - x1) + x2) - x3;
            float num2 = ((y0 - y1) + y2) - y3;
            if ((num == 0f) && (num2 == 0f))
            {
                return new PerspectiveTransform(x1 - x0, x2 - x1, x0, y1 - y0, y2 - y1, y0, 0f, 0f, 1f);
            }
            float num3 = x1 - x2;
            float num4 = x3 - x2;
            float num5 = y1 - y2;
            float num6 = y3 - y2;
            float num7 = (num3 * num6) - (num4 * num5);
            float num8 = ((num * num6) - (num4 * num2)) / num7;
            float num9 = ((num3 * num2) - (num * num5)) / num7;
            return new PerspectiveTransform((x1 - x0) + (num8 * x1), (x3 - x0) + (num9 * x3), x0, (y1 - y0) + (num8 * y1), (y3 - y0) + (num9 * y3), y0, num8, num9, 1f);
        }

        internal PerspectiveTransform times(PerspectiveTransform other)
        {
            return new PerspectiveTransform(((this.a11 * other.a11) + (this.a21 * other.a12)) + (this.a31 * other.a13), ((this.a11 * other.a21) + (this.a21 * other.a22)) + (this.a31 * other.a23), ((this.a11 * other.a31) + (this.a21 * other.a32)) + (this.a31 * other.a33), ((this.a12 * other.a11) + (this.a22 * other.a12)) + (this.a32 * other.a13), ((this.a12 * other.a21) + (this.a22 * other.a22)) + (this.a32 * other.a23), ((this.a12 * other.a31) + (this.a22 * other.a32)) + (this.a32 * other.a33), ((this.a13 * other.a11) + (this.a23 * other.a12)) + (this.a33 * other.a13), ((this.a13 * other.a21) + (this.a23 * other.a22)) + (this.a33 * other.a23), ((this.a13 * other.a31) + (this.a23 * other.a32)) + (this.a33 * other.a33));
        }

        public void transformPoints(float[] points)
        {
            int length = points.Length;
            float num2 = this.a11;
            float num3 = this.a12;
            float num4 = this.a13;
            float num5 = this.a21;
            float num6 = this.a22;
            float num7 = this.a23;
            float num8 = this.a31;
            float num9 = this.a32;
            float num10 = this.a33;
            for (int i = 0; i < length; i += 2)
            {
                float num12 = points[i];
                float num13 = points[i + 1];
                float num14 = ((num4 * num12) + (num7 * num13)) + num10;
                points[i] = (((num2 * num12) + (num5 * num13)) + num8) / num14;
                points[i + 1] = (((num3 * num12) + (num6 * num13)) + num9) / num14;
            }
        }

        public void transformPoints(float[] xValues, float[] yValues)
        {
            int length = xValues.Length;
            for (int i = 0; i < length; i++)
            {
                float num3 = xValues[i];
                float num4 = yValues[i];
                float num5 = ((this.a13 * num3) + (this.a23 * num4)) + this.a33;
                xValues[i] = (((this.a11 * num3) + (this.a21 * num4)) + this.a31) / num5;
                yValues[i] = (((this.a12 * num3) + (this.a22 * num4)) + this.a32) / num5;
            }
        }
    }
}

