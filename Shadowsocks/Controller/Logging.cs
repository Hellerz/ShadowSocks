namespace Shadowsocks.Controller
{
    using System;
    using System.IO;
    using System.Net.Sockets;

    public class Logging
    {
        public static string LogFile;

        public static void Debug(object o)
        {
        }

        public static void LogUsefulException(Exception e)
        {
            if (e is SocketException)
            {
                SocketException exception = (SocketException) e;
                if (((exception.SocketErrorCode != SocketError.ConnectionAborted) && (exception.SocketErrorCode != SocketError.ConnectionReset)) && (exception.SocketErrorCode != SocketError.NotConnected))
                {
                    Console.WriteLine(e);
                }
            }
            else if (!(e is ObjectDisposedException))
            {
                Console.WriteLine(e);
            }
        }

        public static bool OpenLogFile()
        {
            try
            {
                LogFile = Path.Combine(Path.GetTempPath(), "shadowsocks.log");
                FileStream stream = new FileStream(LogFile, FileMode.Append);
                StreamWriterWithTimestamp newOut = new StreamWriterWithTimestamp(stream);
                newOut.AutoFlush = true;
                Console.SetOut(newOut);
                Console.SetError(newOut);
                return true;
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
                return false;
            }
        }
    }
}

