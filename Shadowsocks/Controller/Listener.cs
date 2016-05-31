namespace Shadowsocks.Controller
{
    using Shadowsocks.Model;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    public class Listener
    {
        private Configuration _config;
        private IList<Service> _services;
        private bool _shareOverLAN;
        private Socket _tcpSocket;
        private Socket _udpSocket;

        public Listener(IList<Service> services)
        {
            this._services = services;
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            Socket asyncState = (Socket) ar.AsyncState;
            try
            {
                Socket socket2 = asyncState.EndAccept(ar);
                byte[] buffer = new byte[0x1000];
                object[] state = new object[] { socket2, buffer };
                socket2.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), state);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                try
                {
                    asyncState.BeginAccept(new AsyncCallback(this.AcceptCallback), asyncState);
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception exception2)
                {
                    Logging.LogUsefulException(exception2);
                }
            }
        }

        private bool CheckIfPortInUse(int port)
        {
            foreach (IPEndPoint point in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners())
            {
                if (point.Port == port)
                {
                    return true;
                }
            }
            return false;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            object[] asyncState = (object[]) ar.AsyncState;
            Socket socket = (Socket) asyncState[0];
            byte[] firstPacket = (byte[]) asyncState[1];
            try
            {
                int length = socket.EndReceive(ar);
                foreach (Service service in this._services)
                {
                    if (service.Handle(firstPacket, length, socket, null))
                    {
                        return;
                    }
                }
                if (socket.ProtocolType == ProtocolType.Tcp)
                {
                    socket.Close();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                socket.Close();
            }
        }

        public void RecvFromCallback(IAsyncResult ar)
        {
            UDPState asyncState = (UDPState) ar.AsyncState;
            try
            {
                int length = this._udpSocket.EndReceiveFrom(ar, ref asyncState.remoteEndPoint);
                foreach (Service service in this._services)
                {
                    if (service.Handle(asyncState.buffer, length, this._udpSocket, asyncState))
                    {
                        return;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception)
            {
            }
            finally
            {
                try
                {
                    this._udpSocket.BeginReceiveFrom(asyncState.buffer, 0, asyncState.buffer.Length, SocketFlags.None, ref asyncState.remoteEndPoint, new AsyncCallback(this.RecvFromCallback), asyncState);
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception)
                {
                }
            }
        }

        public void Start(Configuration config)
        {
            this._config = config;
            this._shareOverLAN = config.shareOverLan;
            if (this.CheckIfPortInUse(this._config.localPort))
            {
                throw new Exception(I18N.GetString("Port already in use"));
            }
            try
            {
                this._tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this._tcpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this._udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                IPEndPoint localEP = null;
                if (this._shareOverLAN)
                {
                    localEP = new IPEndPoint(IPAddress.Any, this._config.localPort);
                }
                else
                {
                    localEP = new IPEndPoint(IPAddress.Loopback, this._config.localPort);
                }
                this._tcpSocket.Bind(localEP);
                this._udpSocket.Bind(localEP);
                this._tcpSocket.Listen(0x400);
                Console.WriteLine("Shadowsocks started");
                this._tcpSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), this._tcpSocket);
                UDPState state = new UDPState();
                this._udpSocket.BeginReceiveFrom(state.buffer, 0, state.buffer.Length, SocketFlags.None, ref state.remoteEndPoint, new AsyncCallback(this.RecvFromCallback), state);
            }
            catch (SocketException)
            {
                this._tcpSocket.Close();
                throw;
            }
        }

        public void Stop()
        {
            if (this._tcpSocket != null)
            {
                this._tcpSocket.Close();
                this._tcpSocket = null;
            }
            if (this._udpSocket != null)
            {
                this._udpSocket.Close();
                this._udpSocket = null;
            }
        }

        public interface Service
        {
            bool Handle(byte[] firstPacket, int length, Socket socket, object state);
        }

        public class UDPState
        {
            public byte[] buffer = new byte[0x1000];
            public EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }
    }
}

