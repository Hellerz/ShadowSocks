namespace Shadowsocks.Controller.Strategy
{
    using Shadowsocks.Controller;
    using Shadowsocks.Model;
    using System;
    using System.Collections.Generic;
    using System.Net;

    internal class HighAvailabilityStrategy : IStrategy
    {
        private ShadowsocksController _controller;
        protected ServerStatus _currentServer;
        private Random _random;
        protected Dictionary<Server, ServerStatus> _serverStatus;

        public HighAvailabilityStrategy(ShadowsocksController controller)
        {
            this._controller = controller;
            this._random = new Random();
            this._serverStatus = new Dictionary<Server, ServerStatus>();
        }

        public void ChooseNewServer()
        {
            List<ServerStatus> list = new List<ServerStatus>(this._serverStatus.Values);
            DateTime now = DateTime.Now;
            foreach (ServerStatus status in list)
            {
                TimeSpan span = (TimeSpan) (now - status.lastFailure);
                TimeSpan span2 = (TimeSpan) (now - status.lastTimeDetectLatency);
                TimeSpan span3 = (TimeSpan) (status.lastRead - status.lastWrite);
                status.score = (100000.0 * Math.Min(300.0, span.TotalSeconds)) - (10.0 * ((Math.Min(2000.0, status.latency.TotalMilliseconds) / (1.0 + ((span2.TotalSeconds / 30.0) / 10.0))) + (-100.0 * Math.Min(5.0, span3.TotalSeconds))));
                Logging.Debug(string.Format("server: {0} latency:{1} score: {2}", status.server.FriendlyName(), status.latency, status.score));
            }
            ServerStatus status2 = null;
            foreach (ServerStatus status3 in list)
            {
                if (status2 == null)
                {
                    status2 = status3;
                }
                else if (status3.score >= status2.score)
                {
                    status2 = status3;
                }
            }
            if ((status2 != null) && ((this._currentServer == null) || ((status2.score - this._currentServer.score) > 200.0)))
            {
                this._currentServer = status2;
                Console.WriteLine("HA switching to server: {0}", this._currentServer.server.FriendlyName());
            }
        }

        public Server GetAServer(IStrategyCallerType type, IPEndPoint localIPEndPoint)
        {
            if (type == IStrategyCallerType.TCP)
            {
                this.ChooseNewServer();
            }
            if (this._currentServer == null)
            {
                return null;
            }
            return this._currentServer.server;
        }

        public void ReloadServers()
        {
            Dictionary<Server, ServerStatus> dictionary = new Dictionary<Server, ServerStatus>(this._serverStatus);
            foreach (Server server in this._controller.GetCurrentConfiguration().configs)
            {
                if (!dictionary.ContainsKey(server))
                {
                    ServerStatus status = new ServerStatus();
                    status.server = server;
                    status.lastFailure = DateTime.MinValue;
                    status.lastRead = DateTime.Now;
                    status.lastWrite = DateTime.Now;
                    status.latency = new TimeSpan(0, 0, 0, 0, 10);
                    status.lastTimeDetectLatency = DateTime.Now;
                    dictionary[server] = status;
                }
                else
                {
                    dictionary[server].server = server;
                }
            }
            this._serverStatus = dictionary;
            this.ChooseNewServer();
        }

        public void SetFailure(Server server)
        {
            ServerStatus status;
            Logging.Debug(string.Format("failure: {0}", server.FriendlyName()));
            if (this._serverStatus.TryGetValue(server, out status))
            {
                status.lastFailure = DateTime.Now;
            }
        }

        public void UpdateLastRead(Server server)
        {
            ServerStatus status;
            Logging.Debug(string.Format("last read: {0}", server.FriendlyName()));
            if (this._serverStatus.TryGetValue(server, out status))
            {
                status.lastRead = DateTime.Now;
            }
        }

        public void UpdateLastWrite(Server server)
        {
            ServerStatus status;
            Logging.Debug(string.Format("last write: {0}", server.FriendlyName()));
            if (this._serverStatus.TryGetValue(server, out status))
            {
                status.lastWrite = DateTime.Now;
            }
        }

        public void UpdateLatency(Server server, TimeSpan latency)
        {
            ServerStatus status;
            Logging.Debug(string.Format("latency: {0} {1}", server.FriendlyName(), latency));
            if (this._serverStatus.TryGetValue(server, out status))
            {
                status.latency = latency;
                status.lastTimeDetectLatency = DateTime.Now;
            }
        }

        public string ID
        {
            get
            {
                return "com.shadowsocks.strategy.ha";
            }
        }

        public string Name
        {
            get
            {
                return I18N.GetString("High Availability");
            }
        }

        public class ServerStatus
        {
            public DateTime lastFailure;
            public DateTime lastRead;
            public DateTime lastTimeDetectLatency;
            public DateTime lastWrite;
            public TimeSpan latency;
            public double score;
            public Server server;
        }
    }
}

