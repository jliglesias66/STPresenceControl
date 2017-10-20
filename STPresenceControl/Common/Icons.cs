using System;
using System.Drawing;

namespace STPresenceControl.Common
{
    public static class Icons
    {
        public static Icon CreateTextIcon(string str , Color colorText)
        {
            Font fontToUse = new Font("Microsoft Sans Serif", 16, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(colorText);
            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmapText);

            graphics.Clear(Color.Transparent);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            graphics.DrawString(str, fontToUse, brushToUse, -4, -2);
            IntPtr hIcon = (bitmapText.GetHicon());
            return Icon.FromHandle(hIcon);
        }
    }
}
