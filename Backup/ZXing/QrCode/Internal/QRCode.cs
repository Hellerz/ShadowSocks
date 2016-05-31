namespace ZXing.QrCode.Internal
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    public sealed class QRCode
    {
        [CompilerGenerated]
        private ErrorCorrectionLevel <ECLevel>k__BackingField;
        [CompilerGenerated]
        private int <MaskPattern>k__BackingField;
        [CompilerGenerated]
        private ByteMatrix <Matrix>k__BackingField;
        [CompilerGenerated]
        private ZXing.QrCode.Internal.Mode <Mode>k__BackingField;
        [CompilerGenerated]
        private ZXing.QrCode.Internal.Version <Version>k__BackingField;
        public static int NUM_MASK_PATTERNS = 8;

        public QRCode()
        {
            this.MaskPattern = -1;
        }

        public static bool isValidMaskPattern(int maskPattern)
        {
            return ((maskPattern >= 0) && (maskPattern < NUM_MASK_PATTERNS));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(200);
            builder.Append("<<\n");
            builder.Append(" mode: ");
            builder.Append(this.Mode);
            builder.Append("\n ecLevel: ");
            builder.Append(this.ECLevel);
            builder.Append("\n version: ");
            if (this.Version == null)
            {
                builder.Append("null");
            }
            else
            {
                builder.Append(this.Version);
            }
            builder.Append("\n maskPattern: ");
            builder.Append(this.MaskPattern);
            if (this.Matrix == null)
            {
                builder.Append("\n matrix: null\n");
            }
            else
            {
                builder.Append("\n matrix:\n");
                builder.Append(this.Matrix.ToString());
            }
            builder.Append(">>\n");
            return builder.ToString();
        }

        public ErrorCorrectionLevel ECLevel
        {
            [CompilerGenerated]
            get
            {
                return this.<ECLevel>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<ECLevel>k__BackingField = value;
            }
        }

        public int MaskPattern
        {
            [CompilerGenerated]
            get
            {
                return this.<MaskPattern>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<MaskPattern>k__BackingField = value;
            }
        }

        public ByteMatrix Matrix
        {
            [CompilerGenerated]
            get
            {
                return this.<Matrix>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<Matrix>k__BackingField = value;
            }
        }

        public ZXing.QrCode.Internal.Mode Mode
        {
            [CompilerGenerated]
            get
            {
                return this.<Mode>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<Mode>k__BackingField = value;
            }
        }

        public ZXing.QrCode.Internal.Version Version
        {
            [CompilerGenerated]
            get
            {
                return this.<Version>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<Version>k__BackingField = value;
            }
        }
    }
}

