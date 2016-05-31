namespace ZXing.QrCode.Internal
{
    using System;
    using ZXing;

    public sealed class AlignmentPattern : ResultPoint
    {
        private float estimatedModuleSize;

        internal AlignmentPattern(float posX, float posY, float estimatedModuleSize) : base(posX, posY)
        {
            this.estimatedModuleSize = estimatedModuleSize;
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

        internal AlignmentPattern combineEstimate(float i, float j, float newModuleSize)
        {
            float posX = (this.X + j) / 2f;
            float posY = (this.Y + i) / 2f;
            return new AlignmentPattern(posX, posY, (this.estimatedModuleSize + newModuleSize) / 2f);
        }
    }
}

