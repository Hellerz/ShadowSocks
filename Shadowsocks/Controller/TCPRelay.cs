namespace Shadowsocks.Controller
{
    using System;
    using System.Net.Sockets;

    internal class TCPRelay : Listener.Service
    {
        private ShadowsocksController _controller;

        public TCPRelay(ShadowsocksController controller)
        {
            this._controller = controller;
        }

        public bool Handle(byte[] firstPacket, int length, Socket socket, object state)
        {
            if (socket.ProtocolType != ProtocolType.Tcp)
            {
                return false;
            }
            if ((length < 2) || (firstPacket[0] != 5))
            {
                return false;
            }
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
            Handler handler = new Handler();
            handler.connection = socket;
            handler.controller = this._controller;
            handler.Start(firstPacket, length);
            return true;
        }
    }
}

