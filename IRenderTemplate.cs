using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebGalleryProcessor
{
    public interface IRenderTemplate
    {
        void ApplyImage(String fileName);
        void ApplyTitle(String title);
        void FormatTitle(double titleRotation, double titleOffsetX, double titleOffsetY);
        void FormatImage(double imageRotation, double imageOffsetX, double imageOffsetY);
        void ResolveFonts(String fontResourcePath);
    }
}
