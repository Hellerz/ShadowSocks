namespace Shadowsocks.View
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    public class QRCodeSplashForm : PerPixelAlphaForm
    {
        private static int ANIMATION_STEPS = ((int) (ANIMATION_TIME * FPS));
        private static double ANIMATION_TIME = 0.5;
        private Bitmap bitmap;
        private SolidBrush brush;
        private int flashStep;
        private static double FPS = 66.666666666666671;
        private Graphics g;
        private int h;
        private Pen pen;
        private Stopwatch sw;
        public Rectangle TargetRect;
        private Timer timer;
        private int w;
        private int x;
        private int y;

        public QRCodeSplashForm()
        {
            base.Load += new EventHandler(this.QRCodeSplashForm_Load);
            base.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.White;
            base.ClientSize = new Size(1, 1);
            base.ControlBox = false;
            base.FormBorderStyle = FormBorderStyle.None;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "QRCodeSplashForm";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.SizeGripStyle = SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.Manual;
            base.TopMost = true;
        }

        private void QRCodeSplashForm_Load(object sender, EventArgs e)
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.flashStep = 0;
            this.x = 0;
            this.y = 0;
            this.w = base.Width;
            this.h = base.Height;
            this.sw = Stopwatch.StartNew();
            this.timer = new Timer();
            this.timer.Interval = (int) ((ANIMATION_TIME * 1000.0) / ((double) ANIMATION_STEPS));
            this.timer.Tick += new EventHandler(this.timer_Tick);
            this.timer.Start();
            this.bitmap = new Bitmap(base.Width, base.Height, PixelFormat.Format32bppArgb);
            this.g = Graphics.FromImage(this.bitmap);
            this.pen = new Pen(Color.Red, 3f);
            this.brush = new SolidBrush(Color.FromArgb(30, Color.Red));
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            double num = (((double) this.sw.ElapsedMilliseconds) / 1000.0) / ANIMATION_TIME;
            if (num < 1.0)
            {
                num = 1.0 - Math.Pow(1.0 - num, 4.0);
                this.x = (int) (this.TargetRect.X * num);
                this.y = (int) (this.TargetRect.Y * num);
                this.w = (int) ((this.TargetRect.Width * num) + (base.Size.Width * (1.0 - num)));
                this.h = (int) ((this.TargetRect.Height * num) + (base.Size.Height * (1.0 - num)));
                this.pen.Color = Color.FromArgb((int) (255.0 * num), Color.Red);
                this.brush.Color = Color.FromArgb((int) (30.0 * num), Color.Red);
                this.g.Clear(Color.Transparent);
                this.g.FillRectangle(this.brush, this.x, this.y, this.w, this.h);
                this.g.DrawRectangle(this.pen, this.x, this.y, this.w, this.h);
                base.SetBitmap(this.bitmap);
            }
            else
            {
                if (this.flashStep == 0)
                {
                    this.timer.Interval = 100;
                    this.g.Clear(Color.Transparent);
                    base.SetBitmap(this.bitmap);
                }
                else if (this.flashStep == 1)
                {
                    this.timer.Interval = 50;
                    this.g.FillRectangle(this.brush, this.x, this.y, this.w, this.h);
                    this.g.DrawRectangle(this.pen, this.x, this.y, this.w, this.h);
                    base.SetBitmap(this.bitmap);
                }
                else if (this.flashStep == 1)
                {
                    this.g.Clear(Color.Transparent);
                    base.SetBitmap(this.bitmap);
                }
                else if (this.flashStep == 2)
                {
                    this.g.FillRectangle(this.brush, this.x, this.y, this.w, this.h);
                    this.g.DrawRectangle(this.pen, this.x, this.y, this.w, this.h);
                    base.SetBitmap(this.bitmap);
                }
                else if (this.flashStep == 3)
                {
                    this.g.Clear(Color.Transparent);
                    base.SetBitmap(this.bitmap);
                }
                else if (this.flashStep == 4)
                {
                    this.g.FillRectangle(this.brush, this.x, this.y, this.w, this.h);
                    this.g.DrawRectangle(this.pen, this.x, this.y, this.w, this.h);
                    base.SetBitmap(this.bitmap);
                }
                else
                {
                    this.sw.Stop();
                    this.timer.Stop();
                    this.pen.Dispose();
                    this.brush.Dispose();
                    this.bitmap.Dispose();
                    base.Close();
                }
                this.flashStep++;
            }
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x80000;
                return createParams;
            }
        }
    }
}

