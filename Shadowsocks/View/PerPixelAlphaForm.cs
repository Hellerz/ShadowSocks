namespace Shadowsocks.View
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    public class PerPixelAlphaForm : Form
    {
        public PerPixelAlphaForm()
        {
            base.FormBorderStyle = FormBorderStyle.None;
        }

        public void SetBitmap(Bitmap bitmap)
        {
            this.SetBitmap(bitmap, 0xff);
        }

        public void SetBitmap(Bitmap bitmap, byte opacity)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");
            }
            IntPtr dC = Win32.GetDC(IntPtr.Zero);
            IntPtr hDC = Win32.CreateCompatibleDC(dC);
            IntPtr zero = IntPtr.Zero;
            IntPtr hObject = IntPtr.Zero;
            try
            {
                zero = bitmap.GetHbitmap(Color.FromArgb(0));
                hObject = Win32.SelectObject(hDC, zero);
                Win32.Size psize = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.Point pprSrc = new Win32.Point(0, 0);
                Win32.Point pptDst = new Win32.Point(base.Left, base.Top);
                Win32.BLENDFUNCTION pblend = new Win32.BLENDFUNCTION();
                pblend.BlendOp = 0;
                pblend.BlendFlags = 0;
                pblend.SourceConstantAlpha = opacity;
                pblend.AlphaFormat = 1;
                Win32.UpdateLayeredWindow(base.Handle, dC, ref pptDst, ref psize, hDC, ref pprSrc, 0, ref pblend, 2);
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, dC);
                if (zero != IntPtr.Zero)
                {
                    Win32.SelectObject(hDC, hObject);
                    Win32.DeleteObject(zero);
                }
                Win32.DeleteDC(hDC);
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

