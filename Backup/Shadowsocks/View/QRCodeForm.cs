namespace Shadowsocks.View
{
    using Shadowsocks.Controller;
    using Shadowsocks.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using ZXing.QrCode.Internal;

    public class QRCodeForm : Form
    {
        private string code;
        private IContainer components;
        private PictureBox pictureBox1;

        public QRCodeForm(string code)
        {
            this.code = code;
            this.InitializeComponent();
            base.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());
            this.Text = I18N.GetString("QRCode");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void GenQR(string ssconfig)
        {
            string content = ssconfig;
            ByteMatrix matrix = Encoder.encode(content, ErrorCorrectionLevel.M).Matrix;
            int width = Math.Max(this.pictureBox1.Height / matrix.Height, 1);
            Bitmap image = new Bitmap(matrix.Width * width, matrix.Height * width);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.White);
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    for (int i = 0; i < matrix.Width; i++)
                    {
                        for (int j = 0; j < matrix.Height; j++)
                        {
                            if (matrix[i, j] != 0)
                            {
                                graphics.FillRectangle(brush, width * i, width * j, width, width);
                            }
                        }
                    }
                }
            }
            this.pictureBox1.Image = image;
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new PictureBox();
            ((ISupportInitialize) this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.pictureBox1.Location = new Point(10, 10);
            this.pictureBox1.Margin = new Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(210, 210);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            base.AutoScaleDimensions = new SizeF(96f, 96f);
            base.AutoScaleMode = AutoScaleMode.Dpi;
            this.AutoSize = true;
            base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.BackColor = Color.White;
            base.ClientSize = new Size(0x152, 0x112);
            base.Controls.Add(this.pictureBox1);
            base.FormBorderStyle = FormBorderStyle.FixedSingle;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "QRCodeForm";
            base.Padding = new Padding(10);
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "QRCode";
            base.Load += new EventHandler(this.QRCodeForm_Load);
            ((ISupportInitialize) this.pictureBox1).EndInit();
            base.ResumeLayout(false);
        }

        private void QRCodeForm_Load(object sender, EventArgs e)
        {
            this.GenQR(this.code);
        }
    }
}

