namespace Shadowsocks.Controller.Strategy
{
    using Shadowsocks.Controller;
    using Shadowsocks.Model;
    using System;
    using System.Collections.Generic;
    using System.Net;

    internal class BalancingStrategy : IStrategy
    {
        private ShadowsocksController _controller;
        private Random _random;

        public BalancingStrategy(ShadowsocksController controller)
        {
            this._controller = controller;
            this._random = new Random();
        }

        public Server GetAServer(IStrategyCallerType type, IPEndPoint localIPEndPoint)
        {
            int hashCode;
            List<Server> configs = this._controller.GetCurrentConfiguration().configs;
            if (type == IStrategyCallerType.TCP)
            {
                hashCode = this._random.Next();
            }
            else
            {
                hashCode = localIPEndPoint.GetHashCode();
            }
            return configs[hashCode % configs.Count];
        }

        public void ReloadServers()
        {
        }

        public void SetFailure(Server server)
        {
        }

        public void UpdateLastRead(Server server)
        {
        }

        public void UpdateLastWrite(Server server)
        {
        }

        public void UpdateLatency(Server server, TimeSpan latency)
        {
        }

        public string ID
        {
            get
            {
                return "com.shadowsocks.strategy.balancing";
            }
        }

        public string Name
        {
            get
            {
                return I18N.GetString("Load Balance");
            }
        }
    }
}

