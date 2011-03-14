using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WebGalleryProcessor.Model;

namespace WebGalleryProcessor
{
    internal class GalleryImageProcessor
    {
        private ScaleMode _scaledImageScaleMode;
        private double _scaledImageWidth;
        private double _scaledImagedHeight;
        private ScaleMode _thumbScaleMode;
        private double _thumbImageWidth;
        private double _thumbImageHeight;

        public GalleryImageProcessor(ScaleMode scaledImageScaleMode,
                                     double scaledImageWidth,
                                     double scaledImagedHeight,
                                     ScaleMode thumbScaleMode,
                                     double thumbImageWidth,
                                     double thumbImagedHeight)
        {
            _scaledImageScaleMode = scaledImageScaleMode;
            _scaledImageWidth = scaledImageWidth;
            _scaledImagedHeight = scaledImagedHeight;
            _thumbScaleMode = thumbScaleMode;
            _thumbImageWidth = thumbImageWidth;
            _thumbImageHeight = thumbImagedHeight;
        }

        public void ProcessImage(GalleryImageItem image)
        {
            String imageLocation = image.FullImageFileLocation;
            String scaledImageLocation = image.ScaledImageFileLocation;
            String thumbImageLocation = image.ThumbImageFileLocation;
            if (!File.Exists(scaledImageLocation))
            {
                ScaleImage(_scaledImageScaleMode, imageLocation, scaledImageLocation, _scaledImageWidth, _scaledImagedHeight);
            }
            if (!File.Exists(thumbImageLocation))
            {
                ScaleImage(_thumbScaleMode, imageLocation, thumbImageLocation, _thumbImageWidth, _thumbImageHeight);
            }
        }

        private static void ScaleImage(ScaleMode scaleMode, String imageLocation, String outputLocation, double widthScaleValue, double heightScaleValue)
        {
            switch (scaleMode)
            {
                case ScaleMode.Percentage:
                    ScaleByPercent(imageLocation, widthScaleValue, heightScaleValue, outputLocation);
                    break;
                case ScaleMode.MaxDimensions:
                    ScaleByMaxDimensions(imageLocation, (int)widthScaleValue, (int)heightScaleValue, outputLocation);
                    break;
                case ScaleMode.FixedWidth:
                    ScaleByFixedWidth(imageLocation, (int)widthScaleValue, outputLocation);
                    break;
                case ScaleMode.FixedHeight:
                    ScaleByFixedHeight(imageLocation, (int)heightScaleValue, outputLocation);
                    break;
                default:
                    break;
            }
        }

        private static void CheckAndCreatePath(String outputFile)
        {
            String directoryName = Path.GetDirectoryName(outputFile);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        private static void ScaleByFixedWidth(String fileName, int newWidth, String outputFile)
        {
            System.Drawing.Bitmap resizedImage = null;

            try
            {
                using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(fileName))
                {
                    double resizeRatio = (double)newWidth / originalImage.Size.Width;
                    int newHeight = (int)(originalImage.Size.Height * resizeRatio);

                    System.Drawing.Size newSize = new System.Drawing.Size(newWidth, newHeight);

                    resizedImage = new System.Drawing.Bitmap(originalImage, newSize);
                }

                // Save resized picture.
                CheckAndCreatePath(outputFile);
                resizedImage.Save(outputFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            finally
            {
                if (resizedImage != null)
                {
                    resizedImage.Dispose();
                }
            }
        }

        private static void ScaleByFixedHeight(String fileName, int newHeight, String outputFile)
        {
            System.Drawing.Bitmap resizedImage = null;

            try
            {
                using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(fileName))
                {
                    double resizeRatio = (double)newHeight / originalImage.Size.Height;
                    int newWidth = (int)(originalImage.Size.Width * resizeRatio);

                    System.Drawing.Size newSize = new System.Drawing.Size(newWidth, newHeight);

                    resizedImage = new System.Drawing.Bitmap(originalImage, newSize);
                }

                // Save resized picture.
                CheckAndCreatePath(outputFile);
                resizedImage.Save(outputFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            finally
            {
                if (resizedImage != null)
                {
                    resizedImage.Dispose();
                }
            }
        }

        private static void ScaleByMaxDimensions(String fileName, int maxHeight, int maxWidth, String outputFile)
        {
            System.Drawing.Bitmap resizedImage = null;

            try
            {
                using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(fileName))
                {
                    double resizeRatioByHeight = (double)maxHeight / originalImage.Size.Height;
                    double resizeRatioByWidth = (double)maxWidth / originalImage.Size.Width;

                    int newWidth = 0;
                    int newHeight = 0;
                    if (resizeRatioByHeight > resizeRatioByWidth)
                    {
                        newWidth = (int)(originalImage.Size.Width * resizeRatioByWidth);
                        newHeight = (int)(originalImage.Size.Height * resizeRatioByWidth);
                    }
                    else
                    {
                        newWidth = (int)(originalImage.Size.Width * resizeRatioByHeight);
                        newHeight = (int)(originalImage.Size.Height * resizeRatioByHeight);
                    }

                    System.Drawing.Size newSize = new System.Drawing.Size(newWidth, newHeight);

                    resizedImage = new System.Drawing.Bitmap(originalImage, newSize);
                }

                // Save resized picture.
                CheckAndCreatePath(outputFile);
                resizedImage.Save(outputFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            finally
            {
                if (resizedImage != null)
                {
                    resizedImage.Dispose();
                }
            }
        }

        private static void ScaleByPercent(String fileName, double resizeRatio, String outputFile)
        {
            ScaleByPercent(fileName, resizeRatio, resizeRatio, outputFile);
        }

        private static void ScaleByPercent(String fileName, double widthResizeRatio, double heightResizeRatio, String outputFile)
        {
            System.Drawing.Bitmap resizedImage = null;

            try
            {
                using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(fileName))
                {
                    int newWidth = (int)(originalImage.Size.Width * widthResizeRatio);
                    int newHeight = (int)(originalImage.Size.Height * heightResizeRatio);

                    System.Drawing.Size newSize = new System.Drawing.Size(newWidth, newHeight);

                    resizedImage = new System.Drawing.Bitmap(originalImage, newSize);
                }

                // Save resized picture
                CheckAndCreatePath(outputFile);
                resizedImage.Save(outputFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            finally
            {
                if (resizedImage != null)
                {
                    resizedImage.Dispose();
                }
            }
        }
    }
}
