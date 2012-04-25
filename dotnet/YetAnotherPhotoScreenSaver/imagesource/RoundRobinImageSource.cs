using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Org.Kuhn.Yapss {
    class RoundRobinImageSource : IImageSource {
        private List<IImageSource> imageSources;
        private IImageSource fallbackImageSource;
        private int index = 0;
        public RoundRobinImageSource(List<IImageSource> imageSources, IImageSource fallbackImageSource) {
            this.imageSources = imageSources;
            this.fallbackImageSource = fallbackImageSource;
        }
        public Image GetImage(int minX, int minY) {
            if (imageSources == null || imageSources.Count == 0)
                return fallbackImageSource.GetImage(minX, minY);
            try {
                Image image = imageSources[index].GetImage(minX, minY);
                index = ++index % imageSources.Count;
                return image;
            }
            catch (ImageSourceFailedException ex) {
                Log.Instance.Write("Image source failure", ex);
                imageSources.RemoveAt(index);
                index = 0;
                return GetImage(minX, minY);
            }
        }
    }
}
