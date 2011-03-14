using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace WebGalleryProcessor.Model
{
    public class Gallery
    {
        private List<IGalleryItem> _images;

        public Gallery()
        {
            _images = new List<IGalleryItem>();
        }

        public List<IGalleryItem> Items
        {
            get { return _images; }
            set { _images = value; }
        }

        public String UrlFriendlyName { get { return new Regex(@"\s*").Replace(this.Name, string.Empty); } }

        public String FolderName { get; set; }

        public String Name { get; set; }

        public String CoverImageHttpPath { get; set; }

        public String CoverImageFilePath { get; set; }

        public String ZipLQHttpPath { get; set; }

        public String ZipHQHttpPath { get; set; }

        public String ZipLQFileLocation { get; set; }

        public String ZipHQFileLocation { get; set; }

        public long ZipLQFileSizeBytes { get; set; }

        public long ZipHQFileSizeBytes { get; set; }

        public long ZipLQFileSizeMBytes { get { return ZipLQFileSizeBytes / 1024 / 1024; } }

        public long ZipHQFileSizeMBytes { get { return ZipHQFileSizeBytes / 1024 / 1024; } }

        public String DataFilePath { get; set; }

        public String Description { get; set; }

        public String GalleryExternalHttpPath { get; set; }

        public String GalleryInternalHttpPath { get; set; }

        public String GalleryExternalEnhancedHttpPath { get; set; }

        public String GalleryInternalEnhancedHttpPath { get; set; }

        public DateTime LastUpdatedTime { get; set; }

        public DateTime CreatedTime { get; set; }

        public String FeedImageHttpPath { get; set; }

        public bool Display
        {
            get
            {
#if DEBUG
                return true;
#else 
                return CreatedTime < DateTime.Now;
#endif
            }
            private set { return; }
        }
    }
}