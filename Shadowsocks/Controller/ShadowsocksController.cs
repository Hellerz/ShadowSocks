using Shadowsocks.View;
using Shadowsocks.TaskMannager;
namespace Shadowsocks.Controller
{
    using Shadowsocks.Controller.Strategy;
    using Shadowsocks.Model;
    using Shadowsocks.Util;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class ShadowsocksController
    {
        private Configuration _config = Configuration.Load();
        private Listener _listener;
        private PACServer _pacServer;
        private Thread _ramThread;
        private StrategyManager _strategyManager;
        private bool _systemProxyIsDirty;
        public EventHandler ConfigChanged;
        public EventHandler EnableGlobalChanged;
        public EventHandler EnableStatusChanged;
        public ErrorEventHandler Errored;
        public GFWListUpdater gfwListUpdater;
        public EventHandler<PathEventArgs> PACFileReadyToOpen;
        private PolipoRunner polipoRunner;
        public EventHandler ShareOverLANStatusChanged;
        public bool stopped;
        public EventHandler<GFWListUpdater.ResultEventArgs> UpdatePACFromGFWListCompleted;
        public ErrorEventHandler UpdatePACFromGFWListError;
        public EventHandler<PathEventArgs> UserRuleFileReadyToOpen;

        public ShadowsocksController()
        {
            this._strategyManager = new StrategyManager(this);
            TaskMannager.TaskMannager.AutoTask.Start(Common.Common.DefaultAutoIntervel);
        }

        public bool AddServerBySSURL(string ssURL)
        {
            try
            {
                Server item = new Server(ssURL);
                this._config.configs.Add(item);
                this._config.index = this._config.configs.Count - 1;
                this.SaveConfig(this._config);
                return true;
            }
            catch (Exception exception)
            {
                Logging.LogUsefulException(exception);
                return false;
            }
        }

        public Server GetAServer(IStrategyCallerType type, IPEndPoint localIPEndPoint)
        {
            IStrategy currentStrategy = this.GetCurrentStrategy();
            if (currentStrategy != null)
            {
                return currentStrategy.GetAServer(type, localIPEndPoint);
            }
            if (this._config.index < 0)
            {
                this._config.index = 0;
            }
            return this.GetCurrentServer();
        }

        public Configuration GetConfigurationCopy()
        {
            return Configuration.Load();
        }

        public Configuration GetCurrentConfiguration()
        {
            return this._config;
        }

        public Server GetCurrentServer()
        {
            return this._config.GetCurrentServer();
        }

        public IStrategy GetCurrentStrategy()
        {
            foreach (IStrategy strategy in this._strategyManager.GetStrategies())
            {
                if (strategy.ID == this._config.strategy)
                {
                    return strategy;
                }
            }
            return null;
        }

        public string GetQRCodeForCurrentServer()
        {
            Server currentServer = this.GetCurrentServer();
            string s = string.Concat(new object[] { currentServer.method, ":", currentServer.password, "@", currentServer.server, ":", currentServer.server_port });
            string str2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
            return ("ss://" + str2);
        }

        public IList<IStrategy> GetStrategies()
        {
            return this._strategyManager.GetStrategies();
        }

        private void pacServer_PACFileChanged(object sender, EventArgs e)
        {
            this.UpdateSystemProxy();
        }

        private void pacServer_PACUpdateCompleted(object sender, GFWListUpdater.ResultEventArgs e)
        {
            if (this.UpdatePACFromGFWListCompleted != null)
            {
                this.UpdatePACFromGFWListCompleted(this, e);
            }
        }

        private void pacServer_PACUpdateError(object sender, ErrorEventArgs e)
        {
            if (this.UpdatePACFromGFWListError != null)
            {
                this.UpdatePACFromGFWListError(this, e);
            }
        }

        private void ReleaseMemory()
        {
            while (true)
            {
                Utils.ReleaseMemory();
                Thread.Sleep(0x7530);
            }
        }

        protected void Reload()
        {
            this._config = Configuration.Load();
            if (this.polipoRunner == null)
            {
                this.polipoRunner = new PolipoRunner();
            }
            if (this._pacServer == null)
            {
                this._pacServer = new PACServer();
                this._pacServer.PACFileChanged += new EventHandler(this.pacServer_PACFileChanged);
            }
            this._pacServer.UpdateConfiguration(this._config);
            if (this.gfwListUpdater == null)
            {
                this.gfwListUpdater = new GFWListUpdater();
                this.gfwListUpdater.UpdateCompleted += new EventHandler<GFWListUpdater.ResultEventArgs>(this.pacServer_PACUpdateCompleted);
                this.gfwListUpdater.Error += new ErrorEventHandler(this.pacServer_PACUpdateError);
            }
            if (this._listener != null)
            {
                this._listener.Stop();
            }
            this.polipoRunner.Stop();
            try
            {
                IStrategy currentStrategy = this.GetCurrentStrategy();
                if (currentStrategy != null)
                {
                    currentStrategy.ReloadServers();
                }
                this.polipoRunner.Start(this._config);
                TCPRelay item = new TCPRelay(this);
                UDPRelay relay2 = new UDPRelay(this);
                List<Listener.Service> services = new List<Listener.Service>();
                services.Add(item);
                services.Add(relay2);
                services.Add(this._pacServer);
                services.Add(new PortForwarder(this.polipoRunner.RunningPort));
                this._listener = new Listener(services);
                this._listener.Start(this._config);
            }
            catch (Exception exception)
            {
                if (exception is SocketException)
                {
                    SocketException exception2 = (SocketException) exception;
                    if (exception2.SocketErrorCode == SocketError.AccessDenied)
                    {
                        exception = new Exception(I18N.GetString("Port already in use"), exception);
                    }
                }
                Logging.LogUsefulException(exception);
                this.ReportError(exception);
            }
            if (this.ConfigChanged != null)
            {
                this.ConfigChanged(this, new EventArgs());
            }
            this.UpdateSystemProxy();
            Utils.ReleaseMemory();
        }

        protected void ReportError(Exception e)
        {
            if (this.Errored != null)
            {
                this.Errored(this, new ErrorEventArgs(e));
            }
        }

        public void SaveConfig(Configuration newConfig)
        {
            Configuration.Save(newConfig);
            this.Reload();
        }

        public void SavePACUrl(string pacUrl)
        {
            this._config.pacUrl = pacUrl;
            this.UpdateSystemProxy();
            this.SaveConfig(this._config);
            if (this.ConfigChanged != null)
            {
                this.ConfigChanged(this, new EventArgs());
            }
        }

        public void SaveServers(List<Server> servers, int localPort = 0)
        {
            this._config.configs = servers;
            if (localPort > 0)
            {
                this._config.localPort = localPort;
            }
            this.SaveConfig(this._config);
        }

        public void SelectServerIndex(int index)
        {
            this._config.index = index;
            this._config.strategy = null;
            this.SaveConfig(this._config);
        }

        public void SelectStrategy(string strategyID)
        {
            this._config.index = -1;
            this._config.strategy = strategyID;
            this.SaveConfig(this._config);
        }

        public void Start()
        {
            this.Reload();
        }

        private void StartReleasingMemory()
        {
            this._ramThread = new Thread(new ThreadStart(this.ReleaseMemory));
            this._ramThread.IsBackground = true;
            this._ramThread.Start();
        }

        public void Stop()
        {
            if (!this.stopped)
            {
                this.stopped = true;
                if (this._listener != null)
                {
                    this._listener.Stop();
                }
                if (this.polipoRunner != null)
                {
                    this.polipoRunner.Stop();
                }
                if (this._config.enabled)
                {
                    SystemProxy.Update(this._config, true);
                }
            }
        }

        public void ToggleEnable(bool enabled)
        {
            this._config.enabled = enabled;
            this.UpdateSystemProxy();
            this.SaveConfig(this._config);
            if (this.EnableStatusChanged != null)
            {
                this.EnableStatusChanged(this, new EventArgs());
            }
        }

        public void ToggleGlobal(bool global)
        {
            this._config.global = global;
            this.UpdateSystemProxy();
            this.SaveConfig(this._config);
            if (this.EnableGlobalChanged != null)
            {
                this.EnableGlobalChanged(this, new EventArgs());
            }
        }

        public void ToggleShareOverLAN(bool enabled)
        {
            this._config.shareOverLan = enabled;
            this.SaveConfig(this._config);
            if (this.ShareOverLANStatusChanged != null)
            {
                this.ShareOverLANStatusChanged(this, new EventArgs());
            }
        }

        public void TouchPACFile()
        {
            string str = this._pacServer.TouchPACFile();
            if (this.PACFileReadyToOpen != null)
            {
                PathEventArgs e = new PathEventArgs();
                e.Path = str;
                this.PACFileReadyToOpen(this, e);
            }
        }

        public void TouchUserRuleFile()
        {
            string str = this._pacServer.TouchUserRuleFile();
            if (this.UserRuleFileReadyToOpen != null)
            {
                PathEventArgs e = new PathEventArgs();
                e.Path = str;
                this.UserRuleFileReadyToOpen(this, e);
            }
        }

        public void UpdatePACFromGFWList()
        {
            if (this.gfwListUpdater != null)
            {
                this.gfwListUpdater.UpdatePACFromGFWList(this._config);
            }
        }

        private void UpdateSystemProxy()
        {
            if (this._config.enabled)
            {
                SystemProxy.Update(this._config, false);
                this._systemProxyIsDirty = true;
            }
            else if (this._systemProxyIsDirty)
            {
                SystemProxy.Update(this._config, false);
                this._systemProxyIsDirty = false;
            }
        }

        public void UseOnlinePAC(bool useOnlinePac)
        {
            this._config.useOnlinePac = useOnlinePac;
            this.UpdateSystemProxy();
            this.SaveConfig(this._config);
            if (this.ConfigChanged != null)
            {
                this.ConfigChanged(this, new EventArgs());
            }
        }

        public class PathEventArgs : EventArgs
        {
            public string Path;
        }
    }
}

