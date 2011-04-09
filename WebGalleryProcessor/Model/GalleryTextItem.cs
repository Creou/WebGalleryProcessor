using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebGalleryProcessor.Model
{
    public class GalleryTextItem : IGalleryItem
    {
        public GalleryItemType ItemType{get{return GalleryItemType.Text;}}

        public int SortOrder
        {
            get;
            set;
        }

        public String TextData
        {
            get;
            set;
        }
    }
}
