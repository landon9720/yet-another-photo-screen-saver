using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Org.Kuhn.Yapss {
    public interface IImageSource {
        Image GetImage(int minX, int minY);
    }
    public class ImageSourceFailedException : Exception {
        public ImageSourceFailedException(String message)
            : base(message) {

        }
        public ImageSourceFailedException(String message, Exception innerException)
            : base(message, innerException) {

        }
    }
}
