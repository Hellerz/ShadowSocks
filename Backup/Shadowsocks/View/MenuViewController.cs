namespace Shadowsocks.View
{
    using Microsoft.VisualBasic;
    using Shadowsocks.Controller;
    using Shadowsocks.Controller.Strategy;
    using Shadowsocks.Model;
    using Shadowsocks.Properties;
    using Shadowsocks.Util;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using ZXing;
    using ZXing.Common;
    using ZXing.QrCode;

    public class MenuViewController
    {
        private bool _isFirstRun;
        private NotifyIcon _notifyIcon;
        private string _urlToOpen;
        private MenuItem AutoStartupItem;
        private ConfigForm configForm;
        private MenuItem ConfigItem;
        private ContextMenu contextMenu1;
        private ShadowsocksController controller;
        private MenuItem editGFWUserRuleItem;
        private MenuItem editLocalPACItem;
        private MenuItem editOnlinePACItem;
        private MenuItem enableItem;
        private MenuItem globalModeItem;
        private MenuItem localPACItem;
        private MenuItem modeItem;
        private MenuItem onlinePACItem;
        private MenuItem PACModeItem;
        private MenuItem SeperatorItem;
        private MenuItem ServersItem;
        private MenuItem ShareOverLANItem;
        private UpdateChecker updateChecker;
        private MenuItem updateFromGFWListItem;

        public MenuViewController(ShadowsocksController controller)
        {
            this.controller = controller;
            this.LoadMenu();
            controller.EnableStatusChanged += new EventHandler(this.controller_EnableStatusChanged);
            controller.ConfigChanged += new EventHandler(this.controller_ConfigChanged);
            controller.PACFileReadyToOpen += new EventHandler<ShadowsocksController.PathEventArgs>(this.controller_FileReadyToOpen);
            controller.UserRuleFileReadyToOpen += new EventHandler<ShadowsocksController.PathEventArgs>(this.controller_FileReadyToOpen);
            controller.ShareOverLANStatusChanged += new EventHandler(this.controller_ShareOverLANStatusChanged);
            controller.EnableGlobalChanged += new EventHandler(this.controller_EnableGlobalChanged);
            controller.Errored += new ErrorEventHandler(this.controller_Errored);
            controller.UpdatePACFromGFWListCompleted += new EventHandler<GFWListUpdater.ResultEventArgs>(this.controller_UpdatePACFromGFWListCompleted);
            controller.UpdatePACFromGFWListError += new ErrorEventHandler(this.controller_UpdatePACFromGFWListError);
            this._notifyIcon = new NotifyIcon();
            this.UpdateTrayIcon();
            this._notifyIcon.Visible = true;
            this._notifyIcon.ContextMenu = this.contextMenu1;
            this._notifyIcon.MouseDoubleClick += new MouseEventHandler(this.notifyIcon1_DoubleClick);
            this.updateChecker = new UpdateChecker();
            this.updateChecker.NewVersionFound += new EventHandler(this.updateChecker_NewVersionFound);
            this.LoadCurrentConfiguration();
            this.updateChecker.CheckUpdate(controller.GetConfigurationCopy());
            if (controller.GetConfigurationCopy().isDefault)
            {
                this._isFirstRun = true;
                this.ShowConfigForm();
            }
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/shadowsocks/shadowsocks-csharp");
        }

        private void AServerItem_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem) sender;
            this.controller.SelectServerIndex((int) item.Tag);
        }

        private void AStrategyItem_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem) sender;
            this.controller.SelectStrategy((string) item.Tag);
        }

        private void AutoStartupItem_Click(object sender, EventArgs e)
        {
            this.AutoStartupItem.Checked = !this.AutoStartupItem.Checked;
            if (!AutoStartup.Set(this.AutoStartupItem.Checked))
            {
                MessageBox.Show(I18N.GetString("Failed to update registry"));
            }
        }

        private void Config_Click(object sender, EventArgs e)
        {
            this.ShowConfigForm();
        }

        private void configForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.configForm = null;
            Utils.ReleaseMemory();
            this.ShowFirstTimeBalloon();
        }

        private void controller_ConfigChanged(object sender, EventArgs e)
        {
            this.LoadCurrentConfiguration();
            this.UpdateTrayIcon();
        }

        private void controller_EnableGlobalChanged(object sender, EventArgs e)
        {
            this.globalModeItem.Checked = this.controller.GetConfigurationCopy().global;
            this.PACModeItem.Checked = !this.globalModeItem.Checked;
        }

        private void controller_EnableStatusChanged(object sender, EventArgs e)
        {
            this.enableItem.Checked = this.controller.GetConfigurationCopy().enabled;
            this.modeItem.Enabled = this.enableItem.Checked;
        }

        private void controller_Errored(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.GetException().ToString(), string.Format(I18N.GetString("Shadowsocks Error: {0}"), e.GetException().Message));
        }

        private void controller_FileReadyToOpen(object sender, ShadowsocksController.PathEventArgs e)
        {
            string arguments = "/select, " + e.Path;
            Process.Start("explorer.exe", arguments);
        }

        private void controller_ShareOverLANStatusChanged(object sender, EventArgs e)
        {
            this.ShareOverLANItem.Checked = this.controller.GetConfigurationCopy().shareOverLan;
        }

        private void controller_UpdatePACFromGFWListCompleted(object sender, GFWListUpdater.ResultEventArgs e)
        {
            string content = e.Success ? I18N.GetString("PAC updated") : I18N.GetString("No updates found. Please report to GFWList if you have problems with it.");
            this.ShowBalloonTip(I18N.GetString("Shadowsocks"), content, ToolTipIcon.Info, 0x3e8);
        }

        private void controller_UpdatePACFromGFWListError(object sender, ErrorEventArgs e)
        {
            this.ShowBalloonTip(I18N.GetString("Failed to update PAC file"), e.GetException().Message, ToolTipIcon.Error, 0x1388);
            Logging.LogUsefulException(e.GetException());
        }

        private MenuItem CreateMenuGroup(string text, MenuItem[] items)
        {
            return new MenuItem(I18N.GetString(text), items);
        }

        private MenuItem CreateMenuItem(string text, EventHandler click)
        {
            return new MenuItem(I18N.GetString(text), click);
        }

        private void EditPACFileItem_Click(object sender, EventArgs e)
        {
            this.controller.TouchPACFile();
        }

        private void EditUserRuleFileForGFWListItem_Click(object sender, EventArgs e)
        {
            this.controller.TouchUserRuleFile();
        }

        private void EnableItem_Click(object sender, EventArgs e)
        {
            this.controller.ToggleEnable(!this.enableItem.Checked);
        }

        private void GlobalModeItem_Click(object sender, EventArgs e)
        {
            this.controller.ToggleGlobal(true);
        }

        private void LoadCurrentConfiguration()
        {
            Configuration configurationCopy = this.controller.GetConfigurationCopy();
            this.UpdateServersMenu();
            this.enableItem.Checked = configurationCopy.enabled;
            this.modeItem.Enabled = configurationCopy.enabled;
            this.globalModeItem.Checked = configurationCopy.global;
            this.PACModeItem.Checked = !configurationCopy.global;
            this.ShareOverLANItem.Checked = configurationCopy.shareOverLan;
            this.AutoStartupItem.Checked = AutoStartup.Check();
            this.onlinePACItem.Checked = this.onlinePACItem.Enabled && configurationCopy.useOnlinePac;
            this.localPACItem.Checked = !this.onlinePACItem.Checked;
            this.UpdatePACItemsEnabledStatus();
        }

        private void LoadMenu()
        {
            this.contextMenu1 = new ContextMenu(new MenuItem[] { this.enableItem = this.CreateMenuItem("Enable System Proxy", new EventHandler(this.EnableItem_Click)), this.modeItem = this.CreateMenuGroup("Mode", new MenuItem[] { this.PACModeItem = this.CreateMenuItem("PAC", new EventHandler(this.PACModeItem_Click)), this.globalModeItem = this.CreateMenuItem("Global", new EventHandler(this.GlobalModeItem_Click)) }), this.ServersItem = this.CreateMenuGroup("Servers", new MenuItem[] { this.SeperatorItem = new MenuItem("-"), this.ConfigItem = this.CreateMenuItem("Edit Servers...", new EventHandler(this.Config_Click)), this.CreateMenuItem("Show QRCode...", new EventHandler(this.QRCodeItem_Click)), this.CreateMenuItem("Scan QRCode from Screen...", new EventHandler(this.ScanQRCodeItem_Click)) }), this.CreateMenuGroup("PAC ", new MenuItem[] { this.localPACItem = this.CreateMenuItem("Local PAC", new EventHandler(this.LocalPACItem_Click)), this.onlinePACItem = this.CreateMenuItem("Online PAC", new EventHandler(this.OnlinePACItem_Click)), new MenuItem("-"), this.editLocalPACItem = this.CreateMenuItem("Edit Local PAC File...", new EventHandler(this.EditPACFileItem_Click)), this.updateFromGFWListItem = this.CreateMenuItem("Update Local PAC from GFWList", new EventHandler(this.UpdatePACFromGFWListItem_Click)), this.editGFWUserRuleItem = this.CreateMenuItem("Edit User Rule for GFWList...", new EventHandler(this.EditUserRuleFileForGFWListItem_Click)), this.editOnlinePACItem = this.CreateMenuItem("Edit Online PAC URL...", new EventHandler(this.UpdateOnlinePACURLItem_Click)) }), new MenuItem("-"), this.AutoStartupItem = this.CreateMenuItem("Start on Boot", new EventHandler(this.AutoStartupItem_Click)), this.ShareOverLANItem = this.CreateMenuItem("Allow Clients from LAN", new EventHandler(this.ShareOverLANItem_Click)), new MenuItem("-"), this.CreateMenuItem("Show Logs...", new EventHandler(this.ShowLogItem_Click)), this.CreateMenuItem("About...", new EventHandler(this.AboutItem_Click)), new MenuItem("-"), this.CreateMenuItem("Quit", new EventHandler(this.Quit_Click)) });
        }

        private void LocalPACItem_Click(object sender, EventArgs e)
        {
            if (!this.localPACItem.Checked)
            {
                this.localPACItem.Checked = true;
                this.onlinePACItem.Checked = false;
                this.controller.UseOnlinePAC(false);
                this.UpdatePACItemsEnabledStatus();
            }
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            Process.Start(this.updateChecker.LatestVersionURL);
            this._notifyIcon.BalloonTipClicked -= new EventHandler(this.notifyIcon1_BalloonTipClicked);
        }

        private void notifyIcon1_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ShowConfigForm();
            }
        }

        private void OnlinePACItem_Click(object sender, EventArgs e)
        {
            if (!this.onlinePACItem.Checked)
            {
                if (string.IsNullOrEmpty(this.controller.GetConfigurationCopy().pacUrl))
                {
                    this.UpdateOnlinePACURLItem_Click(sender, e);
                }
                if (!string.IsNullOrEmpty(this.controller.GetConfigurationCopy().pacUrl))
                {
                    this.localPACItem.Checked = false;
                    this.onlinePACItem.Checked = true;
                    this.controller.UseOnlinePAC(true);
                }
                this.UpdatePACItemsEnabledStatus();
            }
        }

        private void openURLFromQRCode(object sender, FormClosedEventArgs e)
        {
            Process.Start(this._urlToOpen);
        }

        private void PACModeItem_Click(object sender, EventArgs e)
        {
            this.controller.ToggleGlobal(false);
        }

        private void QRCodeItem_Click(object sender, EventArgs e)
        {
            new QRCodeForm(this.controller.GetQRCodeForCurrentServer()).Show();
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            this.controller.Stop();
            this._notifyIcon.Visible = false;
            Application.Exit();
        }

        private void ScanQRCodeItem_Click(object sender, EventArgs e)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                using (Bitmap bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                    }
                    int num = 10;
                    for (int i = 0; i < num; i++)
                    {
                        int x = (int) (((bitmap.Width * i) / 2.5) / ((double) num));
                        int y = (int) (((bitmap.Height * i) / 2.5) / ((double) num));
                        Rectangle srcRect = new Rectangle(x, y, bitmap.Width - (x * 2), bitmap.Height - (y * 2));
                        Bitmap image = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
                        double num5 = ((double) screen.Bounds.Width) / ((double) srcRect.Width);
                        using (Graphics graphics2 = Graphics.FromImage(image))
                        {
                            graphics2.DrawImage(bitmap, new Rectangle(0, 0, image.Width, image.Height), srcRect, GraphicsUnit.Pixel);
                        }
                        BitmapLuminanceSource source = new BitmapLuminanceSource(image);
                        BinaryBitmap bitmap3 = new BinaryBitmap(new HybridBinarizer(source));
                        Result result = new QRCodeReader().decode(bitmap3);
                        if (result != null)
                        {
                            bool flag = this.controller.AddServerBySSURL(result.Text);
                            QRCodeSplashForm form = new QRCodeSplashForm();
                            if (flag)
                            {
                                form.FormClosed += new FormClosedEventHandler(this.splash_FormClosed);
                            }
                            else if (result.Text.StartsWith("http://") || result.Text.StartsWith("https://"))
                            {
                                this._urlToOpen = result.Text;
                                form.FormClosed += new FormClosedEventHandler(this.openURLFromQRCode);
                            }
                            else
                            {
                                MessageBox.Show(I18N.GetString("Failed to decode QRCode"));
                                return;
                            }
                            double num6 = 2147483647.0;
                            double num7 = 2147483647.0;
                            double num8 = 0.0;
                            double num9 = 0.0;
                            foreach (ResultPoint point in result.ResultPoints)
                            {
                                num6 = Math.Min(num6, (double) point.X);
                                num7 = Math.Min(num7, (double) point.Y);
                                num8 = Math.Max(num8, (double) point.X);
                                num9 = Math.Max(num9, (double) point.Y);
                            }
                            num6 /= num5;
                            num7 /= num5;
                            num8 /= num5;
                            num9 /= num5;
                            double num10 = (num8 - num6) * 0.20000000298023224;
                            num6 += -num10 + x;
                            num8 += num10 + x;
                            num7 += -num10 + y;
                            num9 += num10 + y;
                            form.Location = new Point(screen.Bounds.X, screen.Bounds.Y);
                            form.TargetRect = new Rectangle(((int) num6) + screen.Bounds.X, ((int) num7) + screen.Bounds.Y, ((int) num8) - ((int) num6), ((int) num9) - ((int) num7));
                            form.Size = new Size(bitmap.Width, bitmap.Height);
                            form.Show();
                            return;
                        }
                    }
                }
            }
            MessageBox.Show(I18N.GetString("No QRCode found. Try to zoom in or move it to the center of the screen."));
        }

        private void ShareOverLANItem_Click(object sender, EventArgs e)
        {
            this.ShareOverLANItem.Checked = !this.ShareOverLANItem.Checked;
            this.controller.ToggleShareOverLAN(this.ShareOverLANItem.Checked);
        }

        private void ShowBalloonTip(string title, string content, ToolTipIcon icon, int timeout)
        {
            this._notifyIcon.BalloonTipTitle = title;
            this._notifyIcon.BalloonTipText = content;
            this._notifyIcon.BalloonTipIcon = icon;
            this._notifyIcon.ShowBalloonTip(timeout);
        }

        private void ShowConfigForm()
        {
            if (this.configForm != null)
            {
                this.configForm.Activate();
            }
            else
            {
                this.configForm = new ConfigForm(this.controller);
                this.configForm.Show();
                this.configForm.FormClosed += new FormClosedEventHandler(this.configForm_FormClosed);
            }
        }

        private void ShowFirstTimeBalloon()
        {
            if (this._isFirstRun)
            {
                this._notifyIcon.BalloonTipTitle = I18N.GetString("Shadowsocks is here");
                this._notifyIcon.BalloonTipText = I18N.GetString("You can turn on/off Shadowsocks in the context menu");
                this._notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                this._notifyIcon.ShowBalloonTip(0);
                this._isFirstRun = false;
            }
        }

        private void ShowLogItem_Click(object sender, EventArgs e)
        {
            string logFile = Logging.LogFile;
            Process.Start("notepad.exe", logFile);
        }

        private void splash_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.ShowConfigForm();
        }

        private void updateChecker_NewVersionFound(object sender, EventArgs e)
        {
            this.ShowBalloonTip(string.Format(I18N.GetString("Shadowsocks {0} Update Found"), this.updateChecker.LatestVersionNumber), I18N.GetString("Click here to download"), ToolTipIcon.Info, 0x1388);
            this._notifyIcon.BalloonTipClicked += new EventHandler(this.notifyIcon1_BalloonTipClicked);
            this._isFirstRun = false;
        }

        private void UpdateOnlinePACURLItem_Click(object sender, EventArgs e)
        {
            string pacUrl = this.controller.GetConfigurationCopy().pacUrl;
            string str2 = Interaction.InputBox(I18N.GetString("Please input PAC Url"), I18N.GetString("Edit Online PAC URL"), pacUrl, -1, -1);
            if (!string.IsNullOrEmpty(str2) && (str2 != pacUrl))
            {
                this.controller.SavePACUrl(str2);
            }
        }

        private void UpdatePACFromGFWListItem_Click(object sender, EventArgs e)
        {
            this.controller.UpdatePACFromGFWList();
        }

        private void UpdatePACItemsEnabledStatus()
        {
            if (this.localPACItem.Checked)
            {
                this.editLocalPACItem.Enabled = true;
                this.updateFromGFWListItem.Enabled = true;
                this.editGFWUserRuleItem.Enabled = true;
                this.editOnlinePACItem.Enabled = false;
            }
            else
            {
                this.editLocalPACItem.Enabled = false;
                this.updateFromGFWListItem.Enabled = false;
                this.editGFWUserRuleItem.Enabled = false;
                this.editOnlinePACItem.Enabled = true;
            }
        }

        private void UpdateServersMenu()
        {
            Menu.MenuItemCollection menuItems = this.ServersItem.MenuItems;
            while (menuItems[0] != this.SeperatorItem)
            {
                menuItems.RemoveAt(0);
            }
            int index = 0;
            foreach (IStrategy strategy in this.controller.GetStrategies())
            {
                MenuItem item = new MenuItem(strategy.Name);
                item.Tag = strategy.ID;
                item.Click += new EventHandler(this.AStrategyItem_Click);
                menuItems.Add(index, item);
                index++;
            }
            int num2 = index;
            Configuration configurationCopy = this.controller.GetConfigurationCopy();
            foreach (Server server in configurationCopy.configs)
            {
                MenuItem item2 = new MenuItem(server.FriendlyName());
                item2.Tag = index - num2;
                item2.Click += new EventHandler(this.AServerItem_Click);
                menuItems.Add(index, item2);
                index++;
            }
            foreach (MenuItem item3 in menuItems)
            {
                if ((item3.Tag != null) && ((item3.Tag.ToString() == configurationCopy.index.ToString()) || (item3.Tag.ToString() == configurationCopy.strategy)))
                {
                    item3.Checked = true;
                }
            }
        }

        private void UpdateTrayIcon()
        {
            Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
            int dpiX = (int) graphics.DpiX;
            graphics.Dispose();
            Bitmap original = null;
            if (dpiX < 0x61)
            {
                original = Resources.ss16;
            }
            else if (dpiX < 0x79)
            {
                original = Resources.ss20;
            }
            else
            {
                original = Resources.ss24;
            }
            Configuration configurationCopy = this.controller.GetConfigurationCopy();
            bool enabled = configurationCopy.enabled;
            bool global = configurationCopy.global;
            if (!enabled)
            {
                Bitmap bitmap2 = new Bitmap(original);
                for (int i = 0; i < bitmap2.Width; i++)
                {
                    for (int j = 0; j < bitmap2.Height; j++)
                    {
                        Color pixel = original.GetPixel(i, j);
                        bitmap2.SetPixel(i, j, Color.FromArgb((byte) (((double) pixel.A) / 1.25), pixel.R, pixel.G, pixel.B));
                    }
                }
                original = bitmap2;
            }
            this._notifyIcon.Icon = Icon.FromHandle(original.GetHicon());
            string name = null;
            if (this.controller.GetCurrentStrategy() != null)
            {
                name = this.controller.GetCurrentStrategy().Name;
            }
            else
            {
                name = configurationCopy.GetCurrentServer().FriendlyName();
            }
            string str2 = I18N.GetString("Shadowsocks") + " 2.5.1\n" + (enabled ? (I18N.GetString("System Proxy On: ") + (global ? I18N.GetString("Global") : I18N.GetString("PAC"))) : string.Format(I18N.GetString("Running: Port {0}"), configurationCopy.localPort)) + "\n" + name;
            this._notifyIcon.Text = str2.Substring(0, Math.Min(0x3f, str2.Length));
        }
    }
}

