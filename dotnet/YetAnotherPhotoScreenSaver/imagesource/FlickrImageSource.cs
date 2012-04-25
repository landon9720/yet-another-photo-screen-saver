using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Net;
using FlickrNet;

namespace Org.Kuhn.Yapss {
    class FlickrImageSource : IImageSource {
        private Flickr f = new Flickr("8bd435140d387b4a471322c0b6b67e84");
        private Photos photos = null;
        private int page = 0;
        private int index = 0;

        private string tags;
        private bool tagsAndLogic;
        private string userId;
        private string text;
        private bool crop;

        private int errorCount = 0;
        private const int MaxErrorCount = 10;

        public FlickrImageSource(string tags, bool tagsAndLogic, string userName, string text, bool crop) {
            f.HttpTimeout = 10000;
            this.tags = tags;
            this.tagsAndLogic = tagsAndLogic;
            try {
                if (!String.IsNullOrEmpty(userName))
                    userId = f.PeopleFindByUsername(userName).UserId;
            }
            catch (Exception ex){
                Log.Instance.Write("Failed getting user ID for user " + userName, ex);
            }
            this.text = text;
            this.crop = crop;
        }

        public Image GetImage(int minX, int minY)  {
            Image image = null;
            while (image == null) {
                try {
                    if (photos == null || index >= photos.PhotoCollection.Length) {
                        if (photos == null || photos.TotalPhotos == 0 || photos.PageNumber >= photos.TotalPages - 1) {
                            page = 0;
                        }
                        else {
                            ++page;
                        }
                        PhotoSearchOptions op = new PhotoSearchOptions();
                        op.SortOrder = PhotoSearchSortOrder.DatePostedDesc;
                        op.Page = page;
                        op.PerPage = 100;
                        if (!String.IsNullOrEmpty(tags))
                            op.Tags = tags;
                        op.TagMode = tagsAndLogic ? TagMode.AllTags : TagMode.AnyTag;
                        if (!String.IsNullOrEmpty(userId))
                            op.UserId = userId;
                        if (!String.IsNullOrEmpty(text))
                            op.Text = text;
                        if (String.IsNullOrEmpty(tags) && String.IsNullOrEmpty(userId) && String.IsNullOrEmpty(text)) {
                            op.MinUploadDate = DateTime.Now - TimeSpan.FromDays(10);
                            op.MaxUploadDate = DateTime.Now - TimeSpan.FromHours(1);
                        }
                        photos = f.PhotosSearch(op);
                        if (photos.TotalPhotos == 0) {
                            if (tagsAndLogic)
                                tagsAndLogic = false;
                            else if (!String.IsNullOrEmpty(tags))
                                tags = null;
                            else if (!String.IsNullOrEmpty(text))
                                text = null;
                            else if (!String.IsNullOrEmpty(userId))
                                userId = null;
                            else
                                throw new ImageSourceFailedException("Flickr search returned no images");
                            continue;
                        }
                        index = 0;
                    }

                    // choose the smallest photo larger than the requested size
                    Photo photo = photos.PhotoCollection[index++];
                    FlickrNet.Size[] sizes = f.PhotosGetSizes(photo.PhotoId).SizeCollection;
                    String url = null;
                    for (int i = sizes.Length - 1; i >= 0; --i) {
                        FlickrNet.Size s = sizes[i];
                        if (!crop && s.Label == "Square")
                            continue;
                        if (s.Width >= minX && s.Height >= minY)
                            url = s.Source;
                    }
                    if (url == null)
                        continue; // nothing large enough

                    image = Image.FromStream(f.DownloadPicture(url));
                }
                catch (Exception ex) {
                    if (++errorCount <= MaxErrorCount) {
                        Log.Instance.Write(string.Format("Failure {0} of {1} loading Flickr photo(s)", errorCount, MaxErrorCount), ex);
                        // try next photo
                    }
                    else {
                        throw new ImageSourceFailedException("Maximum error count reached, failing Flickr image source", ex);
                    }
                }
            }
            errorCount = 0;
            return image;
        }
    }
}
