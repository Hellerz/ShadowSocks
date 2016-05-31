namespace Shadowsocks.Controller
{
    using System;
    using System.IO;

    public class StreamWriterWithTimestamp : StreamWriter
    {
        public StreamWriterWithTimestamp(Stream stream) : base(stream)
        {
        }

        private string GetTimestamp()
        {
            return ("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ");
        }

        public override void Write(string value)
        {
            base.Write(this.GetTimestamp() + value);
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(this.GetTimestamp() + value);
        }
    }
}

