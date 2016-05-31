namespace Shadowsocks.Controller
{
    using Shadowsocks.Controller.Strategy;
    using Shadowsocks.Encryption;
    using Shadowsocks.Model;
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal class UDPRelay : Listener.Service
    {
        private LRUCache<IPEndPoint, UDPHandler> _cache;
        private ShadowsocksController _controller;

        public UDPRelay(ShadowsocksController controller)
        {
            this._controller = controller;
            this._cache = new LRUCache<IPEndPoint, UDPHandler>(0x200);
        }

        public bool Handle(byte[] firstPacket, int length, Socket socket, object state)
        {
            if (socket.ProtocolType != ProtocolType.Udp)
            {
                return false;
            }
            if (length < 4)
            {
                return false;
            }
            Listener.UDPState state2 = (Listener.UDPState) state;
            IPEndPoint remoteEndPoint = (IPEndPoint) state2.remoteEndPoint;
            UDPHandler val = this._cache.get(remoteEndPoint);
            if (val == null)
            {
                val = new UDPHandler(socket, this._controller.GetAServer(IStrategyCallerType.UDP, remoteEndPoint), remoteEndPoint);
                this._cache.add(remoteEndPoint, val);
            }
            val.Send(firstPacket, length);
            val.Receive();
            return true;
        }

        public class UDPHandler
        {
            private byte[] _buffer;
            private Socket _local;
            private IPEndPoint _localEndPoint;
            private Socket _remote;
            private IPEndPoint _remoteEndPoint;
            private Server _server;

            public UDPHandler(Socket local, Server server, IPEndPoint localEndPoint)
            {
                IPAddress address;
                this._buffer = new byte[0x5dc];
                this._local = local;
                this._server = server;
                this._localEndPoint = localEndPoint;
                if (!IPAddress.TryParse(server.server, out address))
                {
                    address = Dns.GetHostEntry(server.server).AddressList[0];
                }
                this._remoteEndPoint = new IPEndPoint(address, server.server_port);
                this._remote = new Socket(this._remoteEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            }

            public void Close()
            {
                try
                {
                    this._remote.Close();
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception)
                {
                }
            }

            public void Receive()
            {
                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                this._remote.BeginReceiveFrom(this._buffer, 0, this._buffer.Length, SocketFlags.None, ref remoteEP, new AsyncCallback(this.RecvFromCallback), null);
            }

            public void RecvFromCallback(IAsyncResult ar)
            {
                try
                {
                    int num2;
                    EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    int length = this._remote.EndReceiveFrom(ar, ref endPoint);
                    byte[] outbuf = new byte[length];
                    EncryptorFactory.GetEncryptor(this._server.method, this._server.password).Decrypt(this._buffer, length, outbuf, out num2);
                    byte[] destinationArray = new byte[num2 + 3];
                    Array.Copy(outbuf, 0, destinationArray, 3, num2);
                    this._local.SendTo(destinationArray, num2 + 3, SocketFlags.None, this._localEndPoint);
                    this.Receive();
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception)
                {
                }
            }

            public void Send(byte[] data, int length)
            {
                int num;
                IEncryptor encryptor = EncryptorFactory.GetEncryptor(this._server.method, this._server.password);
                byte[] destinationArray = new byte[length - 3];
                Array.Copy(data, 3, destinationArray, 0, length - 3);
                byte[] outbuf = new byte[(length - 3) + 0x10];
                encryptor.Encrypt(destinationArray, destinationArray.Length, outbuf, out num);
                this._remote.SendTo(outbuf, this._remoteEndPoint);
            }
        }
    }
}

