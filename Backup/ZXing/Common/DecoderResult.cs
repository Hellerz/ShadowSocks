namespace ZXing.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public sealed class DecoderResult
    {
        [CompilerGenerated]
        private IList<byte[]> <ByteSegments>k__BackingField;
        [CompilerGenerated]
        private string <ECLevel>k__BackingField;
        [CompilerGenerated]
        private int <Erasures>k__BackingField;
        [CompilerGenerated]
        private int <ErrorsCorrected>k__BackingField;
        [CompilerGenerated]
        private object <Other>k__BackingField;
        [CompilerGenerated]
        private byte[] <RawBytes>k__BackingField;
        [CompilerGenerated]
        private int <StructuredAppendParity>k__BackingField;
        [CompilerGenerated]
        private int <StructuredAppendSequenceNumber>k__BackingField;
        [CompilerGenerated]
        private string <Text>k__BackingField;

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
            [CompilerGenerated]
            get
            {
                return this.<ByteSegments>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<ByteSegments>k__BackingField = value;
            }
        }

        public string ECLevel
        {
            [CompilerGenerated]
            get
            {
                return this.<ECLevel>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<ECLevel>k__BackingField = value;
            }
        }

        public int Erasures
        {
            [CompilerGenerated]
            get
            {
                return this.<Erasures>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<Erasures>k__BackingField = value;
            }
        }

        public int ErrorsCorrected
        {
            [CompilerGenerated]
            get
            {
                return this.<ErrorsCorrected>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<ErrorsCorrected>k__BackingField = value;
            }
        }

        public object Other
        {
            [CompilerGenerated]
            get
            {
                return this.<Other>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<Other>k__BackingField = value;
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

        public bool StructuredAppend
        {
            get
            {
                return ((this.StructuredAppendParity >= 0) && (this.StructuredAppendSequenceNumber >= 0));
            }
        }

        public int StructuredAppendParity
        {
            [CompilerGenerated]
            get
            {
                return this.<StructuredAppendParity>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<StructuredAppendParity>k__BackingField = value;
            }
        }

        public int StructuredAppendSequenceNumber
        {
            [CompilerGenerated]
            get
            {
                return this.<StructuredAppendSequenceNumber>k__BackingField;
            }
            private [CompilerGenerated]
            set
            {
                this.<StructuredAppendSequenceNumber>k__BackingField = value;
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
    }
}

