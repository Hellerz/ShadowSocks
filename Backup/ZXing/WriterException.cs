namespace ZXing
{
    using System;

    [Serializable]
    public sealed class WriterException : Exception
    {
        public WriterException()
        {
        }

        public WriterException(string message) : base(message)
        {
        }

        public WriterException(string message, Exception innerExc) : base(message, innerExc)
        {
        }
    }
}

