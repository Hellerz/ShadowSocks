namespace ZXing
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public sealed class Result
    {
        [CompilerGenerated]
        private ZXing.BarcodeFormat <BarcodeFormat>k__BackingField;
        [CompilerGenerated]
        private byte[] <RawBytes>k__BackingField;
        [CompilerGenerated]
        private IDictionary<ResultMetadataType, object> <ResultMetadata>k__BackingField;
        [CompilerGenerated]
        private ResultPoint[] <ResultPoints>k__BackingField;
        [CompilerGenerated]
        private string <Text>k__BackingField;
        [CompilerGenerated]
        private long <Timestamp>k__BackingField;

        public Result(string text, byte[] rawBytes, ResultPoint[] resultPoints, ZXing.BarcodeFormat format) : this(text, rawBytes, resultPoints, format, DateTime.Now.Ticks)
        {
        }

        public Result(string text, byte[] rawBytes, ResultPoint[] resultPoints, ZXing.BarcodeFormat format, long timestamp)
        {
            if ((text == null) && (rawBytes == null))
            {
                throw new ArgumentException("Text and bytes are null");
            }
            this.Text = text;
            this.RawBytes = rawBytes;
            this.ResultPoints = resultPoints;
            this.BarcodeFormat = format;
            this.ResultMetadata = null;
            this.Timestamp = timestamp;
        }

        public void addResultPoints(ResultPoint[] newPoints)
        {
            ResultPoint[] resultPoints = this.ResultPoints;
            if (resultPoints == null)
            {
                this.ResultPoints = newPoints;
            }
            else if ((newPoints != null) && (newPoints.Length > 0))
            {
                ResultPoint[] destinationArray = new ResultPoint[resultPoints.Length + newPoints.Length];
                Array.Copy(resultPoints, 0, destinationArray, 0, resultPoints.Length);
                Array.Copy(newPoints, 0, destinationArray, resultPoints.Length, newPoints.Length);
                this.ResultPoints = destinationArray;
            }
        }

        public void putAllMetadata(IDictionary<ResultMetadataType, object> metadata)
        {
            if (metadata != null)
            {
                if (this.ResultMetadata == null)
                {
                    this.ResultMetadata = metadata;
                }
                else
                {
                    foreach (KeyValuePair<ResultMetadataType, object> pair in metadata)
                    {
                        this.ResultMetadata[pair.Key] = pair.Value;
                    }
                }
            }
        }

        public void putMetadata(ResultMetadataType type, object value)
        {
            if (this.ResultMetadata == null)
            {
                this.ResultMetadata = new Dictionary<ResultMetadataType, object>();
            }
            this.ResultMetadata[type] = value;
        }

        public override string ToString()
        {
            if (this.Text == null)
            {
                return ("[" + this.RawBytes.Length + " bytes]");
            }
            return this.Text;
        }

        public ZXing.BarcodeFormat BarcodeFormat
        {
            [CompilerGenerated]
            get
            {
                return this.<BarcodeFormat>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<BarcodeFormat>k__BackingField = value;
            }
        }

        public byte[] RawBytes
        {
            [CompilerGenerated]
            get
            {
                return this.<RawBytes>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<RawBytes>k__BackingField = value;
            }
        }

        public IDictionary<ResultMetadataType, object> ResultMetadata
        {
            [CompilerGenerated]
            get
            {
                return this.<ResultMetadata>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<ResultMetadata>k__BackingField = value;
            }
        }

        public ResultPoint[] ResultPoints
        {
            [CompilerGenerated]
            get
            {
                return this.<ResultPoints>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<ResultPoints>k__BackingField = value;
            }
        }

        public string Text
        {
            [CompilerGenerated]
            get
            {
                return this.<Text>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<Text>k__BackingField = value;
            }
        }

        public long Timestamp
        {
            [CompilerGenerated]
            get
            {
                return this.<Timestamp>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<Timestamp>k__BackingField = value;
            }
        }
    }
}

