using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Org.Kuhn.Yapss {
    class ColorSquareImageSource : IImageSource {
        public Image GetImage(int minX, int minY) {           
            Bitmap image = new Bitmap(minX, minY, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(image)) {
                using (Brush brush = new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)))) {
                    g.FillRectangle(brush, 0, 0, minX, minY);
                    g.DrawRectangle(Pens.Black, 0, 0, minX - 1, minY - 1);
                }
            }
            return image;
        }
        private Random random = new Random();
    }
}