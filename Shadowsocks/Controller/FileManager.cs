namespace Shadowsocks.Controller
{
    using System;
    using System.IO;
    using System.IO.Compression;

    public class FileManager
    {
        public static bool ByteArrayToFile(string fileName, byte[] content)
        {
            try
            {
                FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                stream.Write(content, 0, content.Length);
                stream.Close();
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception caught in process: {0}", exception.ToString());
            }
            return false;
        }

        public static void UncompressFile(string fileName, byte[] content)
        {
            FileStream stream = File.Create(fileName);
            byte[] buffer = new byte[0x1000];
            using (GZipStream stream2 = new GZipStream(new MemoryStream(content), CompressionMode.Decompress, false))
            {
                while (true)
                {
                    int count = stream2.Read(buffer, 0, buffer.Length);
                    if (count == 0)
                    {
                        goto Label_0046;
                    }
                    stream.Write(buffer, 0, count);
                }
            }
        Label_0046:
            stream.Close();
        }
    }
}

