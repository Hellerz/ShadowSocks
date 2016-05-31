namespace ZXing.QrCode.Internal
{
    using System;
    using ZXing;

    public sealed class QRCodeDecoderMetaData
    {
        private readonly bool mirrored;

        public QRCodeDecoderMetaData(bool mirrored)
        {
            this.mirrored = mirrored;
        }

        public void applyMirroredCorrection(ResultPoint[] points)
        {
            if ((this.mirrored && (points != null)) && (points.Length >= 3))
            {
                ResultPoint point = points[0];
                points[0] = points[2];
                points[2] = point;
            }
        }

        public bool IsMirrored
        {
            get
            {
                return this.mirrored;
            }
        }
    }
}

