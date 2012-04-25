using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Org.Kuhn.Yapss {
    class FileImageSource : IImageSource {
        public FileImageSource(string rootPath) {
            this.rootPath = rootPath;
        }

        public Image GetImage(int minX, int minY) {
            if (files == null)
                try {
                    files = Directory.GetFiles(rootPath, "*.jpg", SearchOption.AllDirectories);
                    if (files.Length == 0)
                    {
                        throw new ImageSourceFailedException("No image files found");
                    }
                    ShufflePhotos();
                }
                catch (Exception ex) {
                    throw new ImageSourceFailedException("Failed loading file list", ex);
                }
            
            Image image = null;
            while (image == null)
                try
                {
                    _currentImageIndex++;
                    if (_currentImageIndex == files.Length)
                    {
                        _currentImageIndex = 0;
                        ShufflePhotos();
                    }

                    image = Image.FromFile(files[_currentImageIndex]);

                    Rotate(image);
                    if (image.Width < minX || image.Height < minY)
                        image = null;
                }
                catch (Exception ex) {
                    Log.Instance.Write("Failed loading disk image", ex);
                }
            return image;
        }

        private void ShufflePhotos()
        {
            // Shuffle photos
            for (var xx = 0; xx < files.Length; xx++)
            {
                var holding = files[xx];
                var swapIndex = random.Next(files.Length);
                files[xx] = files[swapIndex];
                files[swapIndex] = holding;
            }
        }

        private void Rotate(Image image) {
            foreach (PropertyItem pi in image.PropertyItems) {
                if (pi.Id == 274) { // that's the orientation EXIF ID
                    RotateFlipType t = RotateFlipType.RotateNoneFlipNone;
                    switch (pi.Value[0]) {
                        case 1:
                            t = RotateFlipType.RotateNoneFlipNone;
                            break;
                        case 2:
                            t = RotateFlipType.RotateNoneFlipX;
                            break;
                        case 3:
                            t = RotateFlipType.Rotate180FlipNone;
                            break;
                        case 4:
                            t = RotateFlipType.Rotate180FlipX;
                            break;
                        case 5:
                            t = RotateFlipType.Rotate90FlipX;
                            break;
                        case 6:
                            t = RotateFlipType.Rotate90FlipNone;
                            break;
                        case 7:
                            t = RotateFlipType.Rotate270FlipX;
                            break;
                        case 8:
                            t = RotateFlipType.Rotate270FlipNone;
                            break;
                    }
                    image.RotateFlip(t);
                    break;
                }
            }
        }
        private string rootPath;
        private string[] files;
        private int _currentImageIndex = -1;
        private Random random = new Random();
    }
}
