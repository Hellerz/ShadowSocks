namespace Shadowsocks.Controller.Strategy
{
    using Shadowsocks.Controller;
    using System;
    using System.Collections.Generic;

    internal class StrategyManager
    {
        private List<IStrategy> _strategies = new List<IStrategy>();

        public StrategyManager(ShadowsocksController controller)
        {
            this._strategies.Add(new BalancingStrategy(controller));
            this._strategies.Add(new HighAvailabilityStrategy(controller));
        }

        public IList<IStrategy> GetStrategies()
        {
            return this._strategies;
        }
    }
}

