namespace Shadowsocks.View
{
    using Shadowsocks.Controller;
    using Shadowsocks.Model;
    using Shadowsocks.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class ConfigForm : Form
    {
        private Configuration _modifiedConfiguration;
        private int _oldSelectedIndex = -1;
        private Button AddButton;
        private IContainer components;
        private ShadowsocksController controller;
        private Button DeleteButton;
        private Label EncryptionLabel;
        private ComboBox EncryptionSelect;
        private Label IPLabel;
        private TextBox IPTextBox;
        private Button MyCancelButton;
        private Button OKButton;
        private Panel panel2;
        private Label PasswordLabel;
        private TextBox PasswordTextBox;
        private Label ProxyPortLabel;
        private TextBox ProxyPortTextBox;
        private Label RemarksLabel;
        private TextBox RemarksTextBox;
        private GroupBox ServerGroupBox;
        private Label ServerPortLabel;
        private TextBox ServerPortTextBox;
        private ListBox ServersListBox;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel5;

        public ConfigForm(ShadowsocksController controller)
        {
            this.Font = SystemFonts.MessageBoxFont;
            this.InitializeComponent();
            this.ServersListBox.Dock = DockStyle.Fill;
            base.PerformLayout();
            this.UpdateTexts();
            base.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());
            this.controller = controller;
            controller.ConfigChanged += new EventHandler(this.controller_ConfigChanged);
            this.LoadCurrentConfiguration();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (this.SaveOldSelectedServer())
            {
                Server defaultServer = Configuration.GetDefaultServer();
                this._modifiedConfiguration.configs.Add(defaultServer);
                this.LoadConfiguration(this._modifiedConfiguration);
                this.ServersListBox.SelectedIndex = this._modifiedConfiguration.configs.Count - 1;
                this._oldSelectedIndex = this.ServersListBox.SelectedIndex;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.controller.ConfigChanged -= new EventHandler(this.controller_ConfigChanged);
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
        }

        private void ConfigForm_Shown(object sender, EventArgs e)
        {
            this.IPTextBox.Focus();
        }
        private void controller_ConfigChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)this.LoadCurrentConfiguration);
            }
            else
            {
                this.LoadCurrentConfiguration();
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            this._oldSelectedIndex = this.ServersListBox.SelectedIndex;
            if ((this._oldSelectedIndex >= 0) && (this._oldSelectedIndex < this._modifiedConfiguration.configs.Count))
            {
                this._modifiedConfiguration.configs.RemoveAt(this._oldSelectedIndex);
            }
            if (this._oldSelectedIndex >= this._modifiedConfiguration.configs.Count)
            {
                this._oldSelectedIndex = this._modifiedConfiguration.configs.Count - 1;
            }
            this.ServersListBox.SelectedIndex = this._oldSelectedIndex;
            this.LoadConfiguration(this._modifiedConfiguration);
            this.ServersListBox.SelectedIndex = this._oldSelectedIndex;
            this.LoadSelectedServer();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this.RemarksTextBox = new TextBox();
            this.RemarksLabel = new Label();
            this.IPLabel = new Label();
            this.ServerPortLabel = new Label();
            this.PasswordLabel = new Label();
            this.IPTextBox = new TextBox();
            this.ServerPortTextBox = new TextBox();
            this.PasswordTextBox = new TextBox();
            this.EncryptionLabel = new Label();
            this.EncryptionSelect = new ComboBox();
            this.panel2 = new Panel();
            this.OKButton = new Button();
            this.MyCancelButton = new Button();
            this.DeleteButton = new Button();
            this.AddButton = new Button();
            this.ServerGroupBox = new GroupBox();
            this.ServersListBox = new ListBox();
            this.tableLayoutPanel2 = new TableLayoutPanel();
            this.tableLayoutPanel5 = new TableLayoutPanel();
            this.ProxyPortTextBox = new TextBox();
            this.ProxyPortLabel = new Label();
            this.tableLayoutPanel3 = new TableLayoutPanel();
            this.tableLayoutPanel4 = new TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.ServerGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            base.SuspendLayout();
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.RemarksTextBox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.RemarksLabel, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.IPLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ServerPortLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.PasswordLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.IPTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ServerPortTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.PasswordTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.EncryptionLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.EncryptionSelect, 1, 3);
            this.tableLayoutPanel1.Location = new Point(8, 0x15);
            this.tableLayoutPanel1.Margin = new Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new Padding(3);
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel1.Size = new Size(0xee, 0x89);
            this.tableLayoutPanel1.TabIndex = 0;
            this.RemarksTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left;
            this.RemarksTextBox.Location = new Point(0x48, 0x6f);
            this.RemarksTextBox.MaxLength = 0x20;
            this.RemarksTextBox.Name = "RemarksTextBox";
            this.RemarksTextBox.Size = new Size(160, 20);
            this.RemarksTextBox.TabIndex = 10;
            this.RemarksTextBox.WordWrap = false;
            this.RemarksLabel.Anchor = AnchorStyles.Right;
            this.RemarksLabel.AutoSize = true;
            this.RemarksLabel.Location = new Point(0x11, 0x72);
            this.RemarksLabel.Name = "RemarksLabel";
            this.RemarksLabel.Size = new Size(0x31, 13);
            this.RemarksLabel.TabIndex = 9;
            this.RemarksLabel.Text = "Remarks";
            this.IPLabel.Anchor = AnchorStyles.Right;
            this.IPLabel.AutoSize = true;
            this.IPLabel.Location = new Point(15, 9);
            this.IPLabel.Name = "IPLabel";
            this.IPLabel.Size = new Size(0x33, 13);
            this.IPLabel.TabIndex = 0;
            this.IPLabel.Text = "Server IP";
            this.ServerPortLabel.Anchor = AnchorStyles.Right;
            this.ServerPortLabel.AutoSize = true;
            this.ServerPortLabel.Location = new Point(6, 0x23);
            this.ServerPortLabel.Name = "ServerPortLabel";
            this.ServerPortLabel.Size = new Size(60, 13);
            this.ServerPortLabel.TabIndex = 1;
            this.ServerPortLabel.Text = "Server Port";
            this.PasswordLabel.Anchor = AnchorStyles.Right;
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new Point(13, 0x3d);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new Size(0x35, 13);
            this.PasswordLabel.TabIndex = 2;
            this.PasswordLabel.Text = "Password";
            this.IPTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left;
            this.IPTextBox.Location = new Point(0x48, 6);
            this.IPTextBox.MaxLength = 0x200;
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new Size(160, 20);
            this.IPTextBox.TabIndex = 0;
            this.IPTextBox.WordWrap = false;
            this.ServerPortTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left;
            this.ServerPortTextBox.Location = new Point(0x48, 0x20);
            this.ServerPortTextBox.MaxLength = 10;
            this.ServerPortTextBox.Name = "ServerPortTextBox";
            this.ServerPortTextBox.Size = new Size(160, 20);
            this.ServerPortTextBox.TabIndex = 1;
            this.ServerPortTextBox.WordWrap = false;
            this.PasswordTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left;
            this.PasswordTextBox.Location = new Point(0x48, 0x3a);
            this.PasswordTextBox.MaxLength = 0x100;
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.Size = new Size(160, 20);
            this.PasswordTextBox.TabIndex = 2;
            this.PasswordTextBox.WordWrap = false;
            this.EncryptionLabel.Anchor = AnchorStyles.Right;
            this.EncryptionLabel.AutoSize = true;
            this.EncryptionLabel.Location = new Point(9, 0x58);
            this.EncryptionLabel.Name = "EncryptionLabel";
            this.EncryptionLabel.Size = new Size(0x39, 13);
            this.EncryptionLabel.TabIndex = 8;
            this.EncryptionLabel.Text = "Encryption";
            this.EncryptionSelect.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.EncryptionSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            this.EncryptionSelect.FormattingEnabled = true;
            this.EncryptionSelect.ImeMode = ImeMode.NoControl;
            this.EncryptionSelect.ItemHeight = 13;
            this.EncryptionSelect.Items.AddRange(new object[] { "table", "rc4-md5", "salsa20", "chacha20", "aes-256-cfb", "aes-192-cfb", "aes-128-cfb", "rc4" });
            this.EncryptionSelect.Location = new Point(0x48, 0x54);
            this.EncryptionSelect.Name = "EncryptionSelect";
            this.EncryptionSelect.Size = new Size(160, 0x15);
            this.EncryptionSelect.TabIndex = 3;
            this.panel2.Anchor = AnchorStyles.Top;
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.panel2.Location = new Point(0xcf, 0xbb);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(0, 0);
            this.panel2.TabIndex = 1;
            this.OKButton.DialogResult = DialogResult.OK;
            this.OKButton.Dock = DockStyle.Right;
            this.OKButton.Location = new Point(3, 3);
            this.OKButton.Margin = new Padding(3, 3, 3, 0);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new Size(0x4b, 0x17);
            this.OKButton.TabIndex = 8;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new EventHandler(this.OKButton_Click);
            this.MyCancelButton.DialogResult = DialogResult.Cancel;
            this.MyCancelButton.Dock = DockStyle.Right;
            this.MyCancelButton.Location = new Point(0x54, 3);
            this.MyCancelButton.Margin = new Padding(3, 3, 0, 0);
            this.MyCancelButton.Name = "MyCancelButton";
            this.MyCancelButton.Size = new Size(0x4b, 0x17);
            this.MyCancelButton.TabIndex = 9;
            this.MyCancelButton.Text = "Cancel";
            this.MyCancelButton.UseVisualStyleBackColor = true;
            this.MyCancelButton.Click += new EventHandler(this.CancelButton_Click);
            this.DeleteButton.Dock = DockStyle.Right;
            this.DeleteButton.Location = new Point(0x56, 6);
            this.DeleteButton.Margin = new Padding(3, 6, 0, 3);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new Size(80, 0x17);
            this.DeleteButton.TabIndex = 7;
            this.DeleteButton.Text = "&Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new EventHandler(this.DeleteButton_Click);
            this.AddButton.Dock = DockStyle.Left;
            this.AddButton.Location = new Point(0, 6);
            this.AddButton.Margin = new Padding(0, 6, 3, 3);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new Size(80, 0x17);
            this.AddButton.TabIndex = 6;
            this.AddButton.Text = "&Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new EventHandler(this.AddButton_Click);
            this.ServerGroupBox.AutoSize = true;
            this.ServerGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.ServerGroupBox.Controls.Add(this.tableLayoutPanel1);
            this.ServerGroupBox.Location = new Point(0xb2, 0);
            this.ServerGroupBox.Margin = new Padding(12, 0, 0, 0);
            this.ServerGroupBox.Name = "ServerGroupBox";
            this.ServerGroupBox.Size = new Size(0xf9, 0xae);
            this.ServerGroupBox.TabIndex = 6;
            this.ServerGroupBox.TabStop = false;
            this.ServerGroupBox.Text = "Server";
            this.ServersListBox.FormattingEnabled = true;
            this.ServersListBox.IntegralHeight = false;
            this.ServersListBox.Location = new Point(0, 0);
            this.ServersListBox.Margin = new Padding(0);
            this.ServersListBox.Name = "ServersListBox";
            this.ServersListBox.Size = new Size(0xa6, 0x94);
            this.ServersListBox.TabIndex = 5;
            this.ServersListBox.SelectedIndexChanged += new EventHandler(this.ServersListBox_SelectedIndexChanged);
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.ServersListBox, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ServerGroupBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel2.Location = new Point(12, 12);
            this.tableLayoutPanel2.Margin = new Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel2.Size = new Size(0x1ab, 0xee);
            this.tableLayoutPanel2.TabIndex = 7;
            this.tableLayoutPanel5.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.ProxyPortTextBox, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.ProxyPortLabel, 0, 0);
            this.tableLayoutPanel5.Location = new Point(0xf1, 0xae);
            this.tableLayoutPanel5.Margin = new Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.Padding = new Padding(3);
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));
            this.tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));
            this.tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));
            this.tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));
            this.tableLayoutPanel5.Size = new Size(0xba, 0x20);
            this.tableLayoutPanel5.TabIndex = 9;
            this.ProxyPortTextBox.Anchor = AnchorStyles.Left;
            this.ProxyPortTextBox.Location = new Point(0x43, 6);
            this.ProxyPortTextBox.MaxLength = 10;
            this.ProxyPortTextBox.Name = "ProxyPortTextBox";
            this.ProxyPortTextBox.Size = new Size(0x71, 20);
            this.ProxyPortTextBox.TabIndex = 4;
            this.ProxyPortTextBox.WordWrap = false;
            this.ProxyPortLabel.Anchor = AnchorStyles.Right;
            this.ProxyPortLabel.AutoSize = true;
            this.ProxyPortLabel.Location = new Point(6, 9);
            this.ProxyPortLabel.Name = "ProxyPortLabel";
            this.ProxyPortLabel.Size = new Size(0x37, 13);
            this.ProxyPortLabel.TabIndex = 3;
            this.ProxyPortLabel.Text = "Proxy Port";
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            this.tableLayoutPanel3.Controls.Add(this.MyCancelButton, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.OKButton, 0, 0);
            this.tableLayoutPanel3.Dock = DockStyle.Right;
            this.tableLayoutPanel3.Location = new Point(0x10c, 0xd1);
            this.tableLayoutPanel3.Margin = new Padding(3, 3, 0, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel3.Size = new Size(0x9f, 0x1a);
            this.tableLayoutPanel3.TabIndex = 8;
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.DeleteButton, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.AddButton, 0, 0);
            this.tableLayoutPanel4.Dock = DockStyle.Top;
            this.tableLayoutPanel4.Location = new Point(0, 0xae);
            this.tableLayoutPanel4.Margin = new Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new RowStyle());
            this.tableLayoutPanel4.Size = new Size(0xa6, 0x20);
            this.tableLayoutPanel4.TabIndex = 8;
            base.AcceptButton = this.OKButton;
            base.AutoScaleDimensions = new SizeF(96f, 96f);
            base.AutoScaleMode = AutoScaleMode.Dpi;
            this.AutoSize = true;
            base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            base.CancelButton = this.MyCancelButton;
            base.ClientSize = new Size(0x23e, 0x16f);
            base.Controls.Add(this.tableLayoutPanel2);
            base.Controls.Add(this.panel2);
            base.FormBorderStyle = FormBorderStyle.FixedSingle;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ConfigForm";
            base.Padding = new Padding(12, 12, 12, 9);
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Edit Servers";
            base.FormClosed += new FormClosedEventHandler(this.ConfigForm_FormClosed);
            base.Load += new EventHandler(this.ConfigForm_Load);
            base.Shown += new EventHandler(this.ConfigForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ServerGroupBox.ResumeLayout(false);
            this.ServerGroupBox.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadConfiguration(Configuration configuration)
        {
            this.ServersListBox.Items.Clear();
            foreach (Server server in this._modifiedConfiguration.configs)
            {
                this.ServersListBox.Items.Add(server.FriendlyName());
            }
        }

        private void LoadCurrentConfiguration()
        {
            this._modifiedConfiguration = this.controller.GetConfigurationCopy();
            this.LoadConfiguration(this._modifiedConfiguration);
            this._oldSelectedIndex = this._modifiedConfiguration.index;
            if (this._oldSelectedIndex < 0)
            {
                this._oldSelectedIndex = 0;
            }
            this.ServersListBox.SelectedIndex = this._oldSelectedIndex;
            this.LoadSelectedServer();
        }

        private void LoadSelectedServer()
        {
            if ((this.ServersListBox.SelectedIndex >= 0) && (this.ServersListBox.SelectedIndex < this._modifiedConfiguration.configs.Count))
            {
                Server server = this._modifiedConfiguration.configs[this.ServersListBox.SelectedIndex];
                this.IPTextBox.Text = server.server;
                this.ServerPortTextBox.Text = server.server_port.ToString();
                this.PasswordTextBox.Text = server.password;
                this.ProxyPortTextBox.Text = this._modifiedConfiguration.localPort.ToString();
                this.EncryptionSelect.Text = server.method ?? "aes-256-cfb";
                this.RemarksTextBox.Text = server.remarks;
                this.ServerGroupBox.Visible = true;
            }
            else
            {
                this.ServerGroupBox.Visible = false;
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (this.SaveOldSelectedServer())
            {
                if (this._modifiedConfiguration.configs.Count == 0)
                {
                    MessageBox.Show(I18N.GetString("Please add at least one server"));
                }
                else
                {
                    this.controller.SaveServers(this._modifiedConfiguration.configs, this._modifiedConfiguration.localPort);
                    base.Close();
                }
            }
        }

        private bool SaveOldSelectedServer()
        {
            try
            {
                if ((this._oldSelectedIndex != -1) && (this._oldSelectedIndex < this._modifiedConfiguration.configs.Count))
                {
                    Server server2 = new Server();
                    server2.server = this.IPTextBox.Text;
                    server2.server_port = int.Parse(this.ServerPortTextBox.Text);
                    server2.password = this.PasswordTextBox.Text;
                    server2.method = this.EncryptionSelect.Text;
                    server2.remarks = this.RemarksTextBox.Text;
                    Server server = server2;
                    int port = int.Parse(this.ProxyPortTextBox.Text);
                    Configuration.CheckServer(server);
                    Configuration.CheckLocalPort(port);
                    this._modifiedConfiguration.configs[this._oldSelectedIndex] = server;
                    this._modifiedConfiguration.localPort = port;
                }
                return true;
            }
            catch (FormatException)
            {
                MessageBox.Show(I18N.GetString("Illegal port number format"));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            return false;
        }

        private void ServersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._oldSelectedIndex != this.ServersListBox.SelectedIndex)
            {
                if (!this.SaveOldSelectedServer())
                {
                    this.ServersListBox.SelectedIndex = this._oldSelectedIndex;
                }
                else
                {
                    this.LoadSelectedServer();
                    this._oldSelectedIndex = this.ServersListBox.SelectedIndex;
                }
            }
        }

        private void ShowWindow()
        {
            base.Opacity = 1.0;
            base.Show();
            this.IPTextBox.Focus();
        }

        private void UpdateTexts()
        {
            this.AddButton.Text = I18N.GetString("&Add");
            this.DeleteButton.Text = I18N.GetString("&Delete");
            this.IPLabel.Text = I18N.GetString("Server IP");
            this.ServerPortLabel.Text = I18N.GetString("Server Port");
            this.PasswordLabel.Text = I18N.GetString("Password");
            this.EncryptionLabel.Text = I18N.GetString("Encryption");
            this.ProxyPortLabel.Text = I18N.GetString("Proxy Port");
            this.RemarksLabel.Text = I18N.GetString("Remarks");
            this.ServerGroupBox.Text = I18N.GetString("Server");
            this.OKButton.Text = I18N.GetString("OK");
            this.MyCancelButton.Text = I18N.GetString("Cancel");
            this.Text = I18N.GetString("Edit Servers");
        }
    }
}

