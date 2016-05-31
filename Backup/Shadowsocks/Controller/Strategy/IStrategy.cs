namespace Shadowsocks.Controller.Strategy
{
    using Shadowsocks.Model;
    using System;
    using System.Net;

    public interface IStrategy
    {
        Server GetAServer(IStrategyCallerType type, IPEndPoint localIPEndPoint);
        void ReloadServers();
        void SetFailure(Server server);
        void UpdateLastRead(Server server);
        void UpdateLastWrite(Server server);
        void UpdateLatency(Server server, TimeSpan latency);

        string ID { get; }

        string Name { get; }
    }
}

