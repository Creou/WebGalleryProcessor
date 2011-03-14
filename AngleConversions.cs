using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebGalleryProcessor
{
    internal static class AngleConversions
    {
        public static double DegreesToRadians(double degrees)
        {
            return degrees / (180 / Math.PI);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }
    }
}
