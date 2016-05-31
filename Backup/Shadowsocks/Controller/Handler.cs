namespace Shadowsocks.Controller
{
    using Shadowsocks.Controller.Strategy;
    using Shadowsocks.Encryption;
    using Shadowsocks.Model;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Timers;

    internal class Handler
    {
        private byte[] _firstPacket;
        private int _firstPacketLength;
        private DateTime _startConnectTime;
        public const int BufferSize = 0x4020;
        private bool closed;
        private byte command;
        private bool connected;
        public Socket connection;
        private bool connectionShutdown;
        private byte[] connetionRecvBuffer = new byte[0x4000];
        private byte[] connetionSendBuffer = new byte[0x4020];
        public ShadowsocksController controller;
        private object decryptionLock = new object();
        private object encryptionLock = new object();
        public IEncryptor encryptor;
        public const int RecvSize = 0x4000;
        public Socket remote;
        private byte[] remoteRecvBuffer = new byte[0x4000];
        private byte[] remoteSendBuffer = new byte[0x4020];
        private bool remoteShutdown;
        private int retryCount;
        public Server server;
        private int totalRead;
        private int totalWrite;

        private void CheckClose()
        {
            if (this.connectionShutdown && this.remoteShutdown)
            {
                this.Close();
            }
        }

        public void Close()
        {
            lock (this)
            {
                if (this.closed)
                {
                    return;
                }
                this.closed = true;
            }
            if (this.connection != null)
            {
                try
                {
                    this.connection.Shutdown(SocketShutdown.Both);
                    this.connection.Close();
                }
                catch (Exception exception)
                {
                    Logging.LogUsefulException(exception);
                }
            }
            if (this.remote != null)
            {
                try
                {
                    this.remote.Shutdown(SocketShutdown.Both);
                    this.remote.Close();
                }
                catch (Exception exception2)
                {
                    Logging.LogUsefulException(exception2);
                }
            }
            lock (this.encryptionLock)
            {
                lock (this.decryptionLock)
                {
                    if (this.encryptor != null)
                    {
                        this.encryptor.Dispose();
                    }
                }
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Server server = null;
            if (!this.closed)
            {
                try
                {
                    ServerTimer asyncState = (ServerTimer) ar.AsyncState;
                    server = asyncState.Server;
                    asyncState.Elapsed -= new ElapsedEventHandler(this.connectTimer_Elapsed);
                    asyncState.Enabled = false;
                    asyncState.Dispose();
                    this.remote.EndConnect(ar);
                    this.connected = true;
                    TimeSpan latency = (TimeSpan) (DateTime.Now - this._startConnectTime);
                    IStrategy currentStrategy = this.controller.GetCurrentStrategy();
                    if (currentStrategy != null)
                    {
                        currentStrategy.UpdateLatency(server, latency);
                    }
                    this.StartPipe();
                }
                catch (ArgumentException)
                {
                }
                catch (Exception exception)
                {
                    if (server != null)
                    {
                        IStrategy strategy2 = this.controller.GetCurrentStrategy();
                        if (strategy2 != null)
                        {
                            strategy2.SetFailure(server);
                        }
                    }
                    Logging.LogUsefulException(exception);
                    this.RetryConnect();
                }
            }
        }

        private void connectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!this.connected)
            {
                Server server = ((ServerTimer) sender).Server;
                IStrategy currentStrategy = this.controller.GetCurrentStrategy();
                if (currentStrategy != null)
                {
                    currentStrategy.SetFailure(server);
                }
                Console.WriteLine(string.Format("{0} timed out", server.FriendlyName()));
                this.remote.Close();
                this.RetryConnect();
            }
        }

        public void CreateRemote()
        {
            Server aServer = this.controller.GetAServer(IStrategyCallerType.TCP, (IPEndPoint) this.connection.RemoteEndPoint);
            if ((aServer == null) || (aServer.server == ""))
            {
                throw new ArgumentException("No server configured");
            }
            this.encryptor = EncryptorFactory.GetEncryptor(aServer.method, aServer.password);
            this.server = aServer;
        }

        private void HandleUDPAssociate()
        {
            IPEndPoint localEndPoint = (IPEndPoint) this.connection.LocalEndPoint;
            byte[] addressBytes = localEndPoint.Address.GetAddressBytes();
            int port = localEndPoint.Port;
            byte[] array = new byte[(4 + addressBytes.Length) + 2];
            array[0] = 5;
            if (localEndPoint.AddressFamily == AddressFamily.InterNetwork)
            {
                array[3] = 1;
            }
            else if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                array[3] = 4;
            }
            addressBytes.CopyTo(array, 4);
            array[array.Length - 1] = (byte) (port & 0xff);
            array[array.Length - 2] = (byte) ((port >> 8) & 0xff);
            this.connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.ReadAll), true);
        }

        private void HandshakeReceive()
        {
            if (!this.closed)
            {
                try
                {
                    if (this._firstPacketLength > 1)
                    {
                        byte[] buffer2 = new byte[2];
                        buffer2[0] = 5;
                        byte[] buffer = buffer2;
                        if (this._firstPacket[0] != 5)
                        {
                            byte[] buffer3 = new byte[2];
                            buffer3[1] = 0x5b;
                            buffer = buffer3;
                            Console.WriteLine("socks 5 protocol error");
                        }
                        this.connection.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(this.HandshakeSendCallback), null);
                    }
                    else
                    {
                        this.Close();
                    }
                }
                catch (Exception exception)
                {
                    Logging.LogUsefulException(exception);
                    this.Close();
                }
            }
        }

        private void handshakeReceive2Callback(IAsyncResult ar)
        {
            if (!this.closed)
            {
                try
                {
                    if (this.connection.EndReceive(ar) >= 3)
                    {
                        this.command = this.connetionRecvBuffer[1];
                        if (this.command == 1)
                        {
                            byte[] buffer2 = new byte[10];
                            buffer2[0] = 5;
                            buffer2[3] = 1;
                            byte[] buffer = buffer2;
                            this.connection.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(this.ResponseCallback), null);
                        }
                        else if (this.command == 3)
                        {
                            this.HandleUDPAssociate();
                        }
                    }
                    else
                    {
                        Console.WriteLine("failed to recv data in handshakeReceive2Callback");
                        this.Close();
                    }
                }
                catch (Exception exception)
                {
                    Logging.LogUsefulException(exception);
                    this.Close();
                }
            }
        }

        private void HandshakeSendCallback(IAsyncResult ar)
        {
            if (!this.closed)
            {
                try
                {
                    this.connection.EndSend(ar);
                    this.connection.BeginReceive(this.connetionRecvBuffer, 0, 3, SocketFlags.None, new AsyncCallback(this.handshakeReceive2Callback), null);
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
            if (!this.closed)
            {
                try
                {
                    int length = this.connection.EndReceive(ar);
                    this.totalWrite += length;
                    if (length > 0)
                    {
                        int num2;
                        lock (this.encryptionLock)
                        {
                            if (this.closed)
                            {
                                return;
                            }
                            this.encryptor.Encrypt(this.connetionRecvBuffer, length, this.connetionSendBuffer, out num2);
                        }
                        this.remote.BeginSend(this.connetionSendBuffer, 0, num2, SocketFlags.None, new AsyncCallback(this.PipeRemoteSendCallback), null);
                        IStrategy currentStrategy = this.controller.GetCurrentStrategy();
                        if (currentStrategy != null)
                        {
                            currentStrategy.UpdateLastWrite(this.server);
                        }
                    }
                    else
                    {
                        this.remote.Shutdown(SocketShutdown.Send);
                        this.remoteShutdown = true;
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
            if (!this.closed)
            {
                try
                {
                    this.connection.EndSend(ar);
                    this.remote.BeginReceive(this.remoteRecvBuffer, 0, 0x4000, SocketFlags.None, new AsyncCallback(this.PipeRemoteReceiveCallback), null);
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
            if (!this.closed)
            {
                try
                {
                    int length = this.remote.EndReceive(ar);
                    this.totalRead += length;
                    if (length > 0)
                    {
                        int num2;
                        lock (this.decryptionLock)
                        {
                            if (this.closed)
                            {
                                return;
                            }
                            this.encryptor.Decrypt(this.remoteRecvBuffer, length, this.remoteSendBuffer, out num2);
                        }
                        this.connection.BeginSend(this.remoteSendBuffer, 0, num2, SocketFlags.None, new AsyncCallback(this.PipeConnectionSendCallback), null);
                        IStrategy currentStrategy = this.controller.GetCurrentStrategy();
                        if (currentStrategy != null)
                        {
                            currentStrategy.UpdateLastRead(this.server);
                        }
                    }
                    else
                    {
                        this.connection.Shutdown(SocketShutdown.Send);
                        this.connectionShutdown = true;
                        this.CheckClose();
                        int totalRead = this.totalRead;
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
            if (!this.closed)
            {
                try
                {
                    this.remote.EndSend(ar);
                    this.connection.BeginReceive(this.connetionRecvBuffer, 0, 0x4000, SocketFlags.None, new AsyncCallback(this.PipeConnectionReceiveCallback), null);
                }
                catch (Exception exception)
                {
                    Logging.LogUsefulException(exception);
                    this.Close();
                }
            }
        }

        private void ReadAll(IAsyncResult ar)
        {
            if (!this.closed)
            {
                try
                {
                    if (ar.AsyncState != null)
                    {
                        this.connection.EndSend(ar);
                        this.connection.BeginReceive(this.connetionRecvBuffer, 0, 0x4000, SocketFlags.None, new AsyncCallback(this.ReadAll), null);
                    }
                    else if (this.connection.EndReceive(ar) > 0)
                    {
                        this.connection.BeginReceive(this.connetionRecvBuffer, 0, 0x4000, SocketFlags.None, new AsyncCallback(this.ReadAll), null);
                    }
                    else
                    {
                        this.Close();
                    }
                }
                catch (Exception exception)
                {
                    Logging.LogUsefulException(exception);
                    this.Close();
                }
            }
        }

        private void ResponseCallback(IAsyncResult ar)
        {
            try
            {
                this.connection.EndSend(ar);
                this.StartConnect();
            }
            catch (Exception exception)
            {
                Logging.LogUsefulException(exception);
                this.Close();
            }
        }

        private void RetryConnect()
        {
            if (this.retryCount < 4)
            {
                Logging.Debug("Connection failed, retrying");
                this.StartConnect();
                this.retryCount++;
            }
            else
            {
                this.Close();
            }
        }

        public void Start(byte[] firstPacket, int length)
        {
            this._firstPacket = firstPacket;
            this._firstPacketLength = length;
            this.HandshakeReceive();
        }

        private void StartConnect()
        {
            try
            {
                IPAddress address;
                this.CreateRemote();
                if (!IPAddress.TryParse(this.server.server, out address))
                {
                    address = Dns.GetHostEntry(this.server.server).AddressList[0];
                }
                IPEndPoint remoteEP = new IPEndPoint(address, this.server.server_port);
                this.remote = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                this.remote.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
                this._startConnectTime = DateTime.Now;
                ServerTimer state = new ServerTimer(0xbb8);
                state.AutoReset = false;
                state.Elapsed += new ElapsedEventHandler(this.connectTimer_Elapsed);
                state.Enabled = true;
                state.Server = this.server;
                this.connected = false;
                this.remote.BeginConnect(remoteEP, new AsyncCallback(this.ConnectCallback), state);
            }
            catch (Exception exception)
            {
                Logging.LogUsefulException(exception);
                this.Close();
            }
        }

        private void StartPipe()
        {
            if (!this.closed)
            {
                try
                {
                    this.remote.BeginReceive(this.remoteRecvBuffer, 0, 0x4000, SocketFlags.None, new AsyncCallback(this.PipeRemoteReceiveCallback), null);
                    this.connection.BeginReceive(this.connetionRecvBuffer, 0, 0x4000, SocketFlags.None, new AsyncCallback(this.PipeConnectionReceiveCallback), null);
                }
                catch (Exception exception)
                {
                    Logging.LogUsefulException(exception);
                    this.Close();
                }
            }
        }

        private class ServerTimer : Timer
        {
            public Shadowsocks.Model.Server Server;

            public ServerTimer(int p) : base((double) p)
            {
            }
        }
    }
}

