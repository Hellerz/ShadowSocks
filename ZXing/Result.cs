namespace ZXing
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public sealed class Result
    {
        
        private ZXing.BarcodeFormat BarcodeFormatk__BackingField;
        
        private byte[] RawBytesk__BackingField;
        
        private IDictionary<ResultMetadataType, object> ResultMetadatak__BackingField;
        
        private ResultPoint[] ResultPointsk__BackingField;
        
        private string Textk__BackingField;
        
        private long Timestampk__BackingField;

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
            
            get
            {
                return this.BarcodeFormatk__BackingField;
            }
            private 
            set
            {
                this.BarcodeFormatk__BackingField = value;
            }
        }

        public byte[] RawBytes
        {
            
            get
            {
                return this.RawBytesk__BackingField;
            }
            private 
            set
            {
                this.RawBytesk__BackingField = value;
            }
        }

        public IDictionary<ResultMetadataType, object> ResultMetadata
        {
            
            get
            {
                return this.ResultMetadatak__BackingField;
            }
            private 
            set
            {
                this.ResultMetadatak__BackingField = value;
            }
        }

        public ResultPoint[] ResultPoints
        {
            
            get
            {
                return this.ResultPointsk__BackingField;
            }
            private 
            set
            {
                this.ResultPointsk__BackingField = value;
            }
        }

        public string Text
        {
            
            get
            {
                return this.Textk__BackingField;
            }
            private 
            set
            {
                this.Textk__BackingField = value;
            }
        }

        public long Timestamp
        {
            
            get
            {
                return this.Timestampk__BackingField;
            }
            private 
            set
            {
                this.Timestampk__BackingField = value;
            }
        }
    }
}

