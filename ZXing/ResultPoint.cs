namespace ZXing
{
    using System;
    using System.Globalization;
    using System.Text;
    using ZXing.Common.Detector;

    public class ResultPoint
    {
        private readonly byte[] bytesX;
        private readonly byte[] bytesY;
        private string toString;
        private readonly float x;
        private readonly float y;

        public ResultPoint()
        {
        }

        public ResultPoint(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.bytesX = BitConverter.GetBytes(x);
            this.bytesY = BitConverter.GetBytes(y);
        }

        private static float crossProductZ(ResultPoint pointA, ResultPoint pointB, ResultPoint pointC)
        {
            float x = pointB.x;
            float y = pointB.y;
            return (((pointC.x - x) * (pointA.y - y)) - ((pointC.y - y) * (pointA.x - x)));
        }

        public static float distance(ResultPoint pattern1, ResultPoint pattern2)
        {
            return MathUtils.distance(pattern1.x, pattern1.y, pattern2.x, pattern2.y);
        }

        public override bool Equals(object other)
        {
            ResultPoint point = other as ResultPoint;
            if (point == null)
            {
                return false;
            }
            return ((this.x == point.x) && (this.y == point.y));
        }

        public override int GetHashCode()
        {
            return (((((0x1f * ((((this.bytesX[0] << 0x18) + (this.bytesX[1] << 0x10)) + (this.bytesX[2] << 8)) + this.bytesX[3])) + (this.bytesY[0] << 0x18)) + (this.bytesY[1] << 0x10)) + (this.bytesY[2] << 8)) + this.bytesY[3]);
        }

        public static void orderBestPatterns(ResultPoint[] patterns)
        {
            ResultPoint point;
            ResultPoint point2;
            ResultPoint point3;
            float num = distance(patterns[0], patterns[1]);
            float num2 = distance(patterns[1], patterns[2]);
            float num3 = distance(patterns[0], patterns[2]);
            if ((num2 >= num) && (num2 >= num3))
            {
                point2 = patterns[0];
                point = patterns[1];
                point3 = patterns[2];
            }
            else if ((num3 >= num2) && (num3 >= num))
            {
                point2 = patterns[1];
                point = patterns[0];
                point3 = patterns[2];
            }
            else
            {
                point2 = patterns[2];
                point = patterns[0];
                point3 = patterns[1];
            }
            if (crossProductZ(point, point2, point3) < 0f)
            {
                ResultPoint point4 = point;
                point = point3;
                point3 = point4;
            }
            patterns[0] = point;
            patterns[1] = point2;
            patterns[2] = point3;
        }

        public override string ToString()
        {
            if (this.toString == null)
            {
                StringBuilder builder = new StringBuilder(0x19);
                builder.AppendFormat(CultureInfo.CurrentUICulture, "({0}, {1})", new object[] { this.x, this.y });
                this.toString = builder.ToString();
            }
            return this.toString;
        }

        public virtual float X
        {
            get
            {
                return this.x;
            }
        }

        public virtual float Y
        {
            get
            {
                return this.y;
            }
        }
    }
}

