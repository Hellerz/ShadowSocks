namespace ZXing.QrCode.Internal
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    public sealed class QRCode
    {
        
        private ErrorCorrectionLevel ECLevelk__BackingField;
        
        private int MaskPatternk__BackingField;
        
        private ByteMatrix Matrixk__BackingField;
        
        private ZXing.QrCode.Internal.Mode Modek__BackingField;
        
        private ZXing.QrCode.Internal.Version Versionk__BackingField;
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
            
            get
            {
                return this.ECLevelk__BackingField;
            }
            
            set
            {
                this.ECLevelk__BackingField = value;
            }
        }

        public int MaskPattern
        {
            
            get
            {
                return this.MaskPatternk__BackingField;
            }
            
            set
            {
                this.MaskPatternk__BackingField = value;
            }
        }

        public ByteMatrix Matrix
        {
            
            get
            {
                return this.Matrixk__BackingField;
            }
            
            set
            {
                this.Matrixk__BackingField = value;
            }
        }

        public ZXing.QrCode.Internal.Mode Mode
        {
            
            get
            {
                return this.Modek__BackingField;
            }
            
            set
            {
                this.Modek__BackingField = value;
            }
        }

        public ZXing.QrCode.Internal.Version Version
        {
            
            get
            {
                return this.Versionk__BackingField;
            }
            
            set
            {
                this.Versionk__BackingField = value;
            }
        }
    }
}

