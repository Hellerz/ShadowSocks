namespace ZXing.QrCode.Internal
{
    using System;
    using ZXing;

    public sealed class FinderPattern : ResultPoint
    {
        private int count;
        private readonly float estimatedModuleSize;

        internal FinderPattern(float posX, float posY, float estimatedModuleSize) : this(posX, posY, estimatedModuleSize, 1)
        {
            this.estimatedModuleSize = estimatedModuleSize;
            this.count = 1;
        }

        internal FinderPattern(float posX, float posY, float estimatedModuleSize, int count) : base(posX, posY)
        {
            this.estimatedModuleSize = estimatedModuleSize;
            this.count = count;
        }

        internal bool aboutEquals(float moduleSize, float i, float j)
        {
            if ((Math.Abs((float) (i - this.Y)) > moduleSize) || (Math.Abs((float) (j - this.X)) > moduleSize))
            {
                return false;
            }
            float num = Math.Abs((float) (moduleSize - this.estimatedModuleSize));
            if (num > 1f)
            {
                return (num <= this.estimatedModuleSize);
            }
            return true;
        }

        internal FinderPattern combineEstimate(float i, float j, float newModuleSize)
        {
            int count = this.count + 1;
            float posX = ((this.count * this.X) + j) / ((float) count);
            float posY = ((this.count * this.Y) + i) / ((float) count);
            return new FinderPattern(posX, posY, ((this.count * this.estimatedModuleSize) + newModuleSize) / ((float) count), count);
        }

        internal int Count
        {
            get
            {
                return this.count;
            }
        }

        public float EstimatedModuleSize
        {
            get
            {
                return this.estimatedModuleSize;
            }
        }
    }
}

