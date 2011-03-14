using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGalleryProcessor.Model
{
    public class GalleryImageItem : IGalleryItem
    {        
        public GalleryItemType ItemType{get{return GalleryItemType.Image;}}

        public string Name { get; set; }

        public string FullImageHttpPath { get; set; }

        public string ScaledImageHttpPath { get; set; }

        public string ThumbImageHttpPath { get; set; }

        public string ThumbImageFileLocation { get; set; }

        public string ScaledImageFileLocation { get; set; }

        public string FullImageFileLocation { get; set; }

        public string TextData { get; set; }

        public int SortOrder { get; set; }

        public bool IsCover  { get; set; }
    }
}