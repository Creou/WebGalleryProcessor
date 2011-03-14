using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebGalleryProcessor.Model
{
    public interface IGalleryItem
    {
        GalleryItemType ItemType
        {
            get;
        }

        int SortOrder
        {
            get;
            set;
        }

        String TextData
        {
            get;
            set;
        }
    }
}
