namespace Shadowsocks.Controller
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal class PortForwarder : Listener.Service
    {
        private int _targetPort;

        public PortForwarder(int targetPort)
        {
            this._targetPort = targetPort;
        }

        public bool Handle(byte[] firstPacket, int length, Socket socket, object state)
        {
            if (socket.ProtocolType != ProtocolType.Tcp)
            {
                return false;
            }
            new Handler().Start(firstPacket, length, socket, this._targetPort);
            return true;
        }

        private class Handler
        {
            private bool _closed;
            private byte[] _firstPacket;
            private int _firstPacketLength;
            private Socket _local;
            private bool _localShutdown;
            private Socket _remote;
            private bool _remoteShutdown;
            private byte[] connetionRecvBuffer = new byte[0x4000];
            public const int RecvSize = 0x4000;
            private byte[] remoteRecvBuffer = new byte[0x4000];

            private void CheckClose()
            {
                if (this._localShutdown && this._remoteShutdown)
                {
                    this.Close();
                }
            }

            public void Close()
            {
                lock (this)
                {
                    if (this._closed)
                    {
                        return;
                    }
                    this._closed = true;
                }
                if (this._local != null)
                {
                    try
                    {
                        this._local.Shutdown(SocketShutdown.Both);
                        this._local.Close();
                    }
                    catch (Exception exception)
                    {
                        Logging.LogUsefulException(exception);
                    }
                }
                if (this._remote != null)
                {
                    try
                    {
                        this._remote.Shutdown(SocketShutdown.Both);
                        this._remote.Close();
                    }
                    catch (SocketException exception2)
                    {
                        Logging.LogUsefulException(exception2);
                    }
                }
            }

            private void ConnectCallback(IAsyncResult ar)
            {
                if (!this._closed)
                {
                    try
                    {
                        this._remote.EndConnect(ar);
                        this.HandshakeReceive();
                    }
                    catch (Exception exception)
                    {
                        Logging.LogUsefulException(exception);
                        this.Close();
                    }
                }
            }

            private void HandshakeReceive()
            {
                if (!this._closed)
                {
                    try
                    {
                        this._remote.BeginSend(this._firstPacket, 0, this._firstPacketLength, SocketFlags.None, new AsyncCallback(this.StartPipe), null);
                    }
                    catch (Exception exception)
                    {
                        Logging.LogUsefulException(exception);
                        this.Close();
                    }
                }
            }

            private void PipeConnectionReceiveCallback(IAsyncResult ar)
            {
                if (!this._closed)
                {
                    try
                    {
                        int size = this._local.EndReceive(ar);
                        if (size > 0)
                        {
                            this._remote.BeginSend(this.connetionRecvBuffer, 0, size, SocketFlags.None, new AsyncCallback(this.PipeRemoteSendCallback), null);
                        }
                        else
                        {
                            this._remote.Shutdown(SocketShutdown.Send);
                            this._remoteShutdown = true;
                            this.CheckClose();
                        }
                    }
                    catch (Exception exception)
                    {
                        Logging.LogUsefulException(exception);
                        this.Close();
                    }
                }
            }

            private void PipeConnectionSendCallback(IAsyncResult ar)
            {
                if (!this._closed)
                {
                    try
                    {
                        this._local.EndSend(ar);
                        this._remote.BeginReceive(this.remoteRecvBuffer, 0, 0x4000, SocketFlags.None, new AsyncCallback(this.PipeRemoteReceiveCallback), null);
                    }
                    catch (Exception exception)
                    {
                        Logging.LogUsefulException(exception);
                        this.Close();
                    }
                }
            }

            private void PipeRemoteReceiveCallback(IAsyncResult ar)
            {
                if (!this._closed)
                {
                    try
                    {
                        int size = this._remote.EndReceive(ar);
                        if (size > 0)
                        {
                            this._local.BeginSend(this.remoteRecvBuffer, 0, size, SocketFlags.None, new AsyncCallback(this.PipeConnectionSendCallback), null);
                        }
                        else
                        {
                            this._local.Shutdown(SocketShutdown.Send);
                            this._localShutdown = true;
                            this.CheckClose();
                        }
                    }
                    catch (Exception exception)
                    {
                        Logging.LogUsefulException(exception);
                        this.Close();
                    }
                }
            }

            private void PipeRemoteSendCallback(IAsyncResult ar)
            {
                if (!this._closed)
                {
                    try
                    {
                        this._remote.EndSend(ar);
                        this._local.BeginReceive(this.connetionRecvBuffer, 0, 0x4000, SocketFlags.None, new AsyncCallback(this.PipeConnectionReceiveCallback), null);
                    }
                    catch (Exception exception)
                    {
                        Logging.LogUsefulException(exception);
                        this.Close();
                    }
                }
            }

            public void Start(byte[] firstPacket, int length, Socket socket, int targetPort)
            {
                this._firstPacket = firstPacket;
                this._firstPacketLength = length;
                this._local = socket;
                try
                {
                    IPAddress address;
                    IPAddress.TryParse("127.0.0.1", out address);
                    IPEndPoint remoteEP = new IPEndPoint(address, targetPort);
                    this._remote = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    this._remote.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
                    this._remote.BeginConnect(remoteEP, new AsyncCallback(this.ConnectCallback), null);
                }
                catch (Exception exception)
                {
                    Logging.LogUsefulException(exception);
                    this.Close();
                }
            }

            private void StartPipe(IAsyncResult ar)
            {
                if (!this._closed)
                {
                    try
                    {
                        this._remote.EndSend(ar);
                        this._remote.BeginReceive(this.remoteRecvBuffer, 0, 0x4000, SocketFlags.None, new AsyncCallback(this.PipeRemoteReceiveCallback), null);
                        this._local.BeginReceive(this.connetionRecvBuffer, 0, 0x4000, SocketFlags.None, new AsyncCallback(this.PipeConnectionReceiveCallback), null);
                    }
                    catch (Exception exception)
                    {
                        Logging.LogUsefulException(exception);
                        this.Close();
                    }
                }
            }
        }
    }
}

