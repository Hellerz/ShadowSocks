namespace ZXing.QrCode.Internal
{
    using System;

    public sealed class FinderPatternInfo
    {
        private readonly FinderPattern bottomLeft;
        private readonly FinderPattern topLeft;
        private readonly FinderPattern topRight;

        public FinderPatternInfo(FinderPattern[] patternCenters)
        {
            this.bottomLeft = patternCenters[0];
            this.topLeft = patternCenters[1];
            this.topRight = patternCenters[2];
        }

        public FinderPattern BottomLeft
        {
            get
            {
                return this.bottomLeft;
            }
        }

        public FinderPattern TopLeft
        {
            get
            {
                return this.topLeft;
            }
        }

        public FinderPattern TopRight
        {
            get
            {
                return this.topRight;
            }
        }
    }
}

