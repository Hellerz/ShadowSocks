namespace ZXing.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public sealed class DecoderResult
    {
        
        private IList<byte[]> ByteSegmentsk__BackingField;
        
        private string ECLevelk__BackingField;
        
        private int Erasuresk__BackingField;
        
        private int ErrorsCorrectedk__BackingField;
        
        private object Otherk__BackingField;
        
        private byte[] RawBytesk__BackingField;
        
        private int StructuredAppendParityk__BackingField;
        
        private int StructuredAppendSequenceNumberk__BackingField;
        
        private string Textk__BackingField;

        public DecoderResult(byte[] rawBytes, string text, IList<byte[]> byteSegments, string ecLevel) : this(rawBytes, text, byteSegments, ecLevel, -1, -1)
        {
        }

        public DecoderResult(byte[] rawBytes, string text, IList<byte[]> byteSegments, string ecLevel, int saSequence, int saParity)
        {
            if ((rawBytes == null) && (text == null))
            {
                throw new ArgumentException();
            }
            this.RawBytes = rawBytes;
            this.Text = text;
            this.ByteSegments = byteSegments;
            this.ECLevel = ecLevel;
            this.StructuredAppendParity = saParity;
            this.StructuredAppendSequenceNumber = saSequence;
        }

        public IList<byte[]> ByteSegments
        {
           
            get
            {
                return this.ByteSegmentsk__BackingField;
            }
            private 
            set
            {
                this.ByteSegmentsk__BackingField = value;
            }
        }

        public string ECLevel
        {
            get
            {
                return this.ECLevelk__BackingField;
            }
            private
            set
            {
                this.ECLevelk__BackingField = value;
            }
        }

        public int Erasures
        {
            
            get
            {
                return this.Erasuresk__BackingField;
            }
            
            set
            {
                this.Erasuresk__BackingField = value;
            }
        }

        public int ErrorsCorrected
        {
            
            get
            {
                return this.ErrorsCorrectedk__BackingField;
            }
            
            set
            {
                this.ErrorsCorrectedk__BackingField = value;
            }
        }

        public object Other
        {
            
            get
            {
                return this.Otherk__BackingField;
            }
            
            set
            {
                this.Otherk__BackingField = value;
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

        public bool StructuredAppend
        {
            get
            {
                return ((this.StructuredAppendParity >= 0) && (this.StructuredAppendSequenceNumber >= 0));
            }
        }

        public int StructuredAppendParity
        {
            
            get
            {
                return this.StructuredAppendParityk__BackingField;
            }
            private
            set
            {
                this.StructuredAppendParityk__BackingField = value;
            }
        }

        public int StructuredAppendSequenceNumber
        {
            
            get
            {
                return this.StructuredAppendSequenceNumberk__BackingField;
            }
            private
            set
            {
                this.StructuredAppendSequenceNumberk__BackingField = value;
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
    }
}

