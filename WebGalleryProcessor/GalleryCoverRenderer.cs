using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace WebGalleryProcessor
{
    internal partial class GalleryCoverRenderer
    {
        private byte[] imageBuffer;

        private String _contentPath;
        private String _coverImagePath;
        private String _title;

        private int _imageRotationMax;
        private int _imageRotationMin;
        private int _titleRotationMax;
        private int _titleRotationMin;
        private int _titleOffestXMax;
        private int _titleOffestXMin;
        private int _titleOffestYMax;
        private int _titleOffestYMin;
        private int _imageOffestXMax;
        private int _imageOffestXMin;
        private int _imageOffestYMax;
        private int _imageOffestYMin;

        public GalleryCoverRenderer(String contentPath)
            : this(contentPath,
                    Settings.Default.CoverRender_Default_ImageRotationMax,
                    Settings.Default.CoverRender_Default_ImageRotationMin,
                    Settings.Default.CoverRender_Default_TitleRotationMax,
                    Settings.Default.CoverRender_Default_TitleRotationMin,

                    Settings.Default.CoverRender_Default_TitleOffsetXMax,
                    Settings.Default.CoverRender_Default_TitleOffsetXMin,
                    Settings.Default.CoverRender_Default_TitleOffsetYMax,
                    Settings.Default.CoverRender_Default_TitleOffsetYMin,

                    Settings.Default.CoverRender_Default_ImageOffsetXMax,
                    Settings.Default.CoverRender_Default_ImageOffsetXMin,
                    Settings.Default.CoverRender_Default_ImageOffsetYMax,
                    Settings.Default.CoverRender_Default_ImageOffsetYMin)
        {
        }

        public GalleryCoverRenderer(String contentPath, int imageRotationMax, int imageRotationMin, int titleRotationMax, int titleRotationMin, int titleOffestXMax, int titleOffestXMin, int titleOffestYMax, int titleOffestYMin, int imageOffestXMax, int imageOffestXMin, int imageOffestYMax, int imageOffestYMin)
        {
            _contentPath = contentPath;

            _imageRotationMax = imageRotationMax;
            _imageRotationMin = imageRotationMin;
            _titleRotationMax = titleRotationMax;
            _titleRotationMin = titleRotationMin;

            _titleOffestXMax = titleOffestXMax;
            _titleOffestXMin = titleOffestXMin;
            _titleOffestYMax = titleOffestYMax;
            _titleOffestYMin = titleOffestYMin;

            _imageOffestXMax = imageOffestXMax;
            _imageOffestXMin = imageOffestXMin;
            _imageOffestYMax = imageOffestYMax;
            _imageOffestYMin = imageOffestYMin;
        }

        public void GenerateCoverImage(String title, String outputImagePath, String coverImagePath)
        {
            _coverImagePath = coverImagePath;
            _title = title;
            this.RenderImage();

            using (FileStream outputFileStream = new FileStream(outputImagePath, FileMode.Create))
            {
                outputFileStream.Write(imageBuffer, 0, imageBuffer.Length);
            }
        }

        private void RenderImage()
        {
            Thread worker = new Thread(new ThreadStart(this.RenderImageWorker));
            worker.SetApartmentState(ApartmentState.STA);
            worker.Name = "RenderImageWorker";
            worker.Start();
            worker.Join();
        }

        private void RenderImageWorker()
        {
            IRenderTemplate renderTemplate = new GalleryCoverRenderTemplate();

            Random r = new Random();
            double imageRotation = r.Next(_imageRotationMin, _imageRotationMax);
            double titleRotation = r.Next(_titleRotationMin, _titleRotationMax);
            double titleOffsetX = r.Next(_titleOffestXMin, _titleOffestXMax);
            double titleOffsetY = r.Next(_titleOffestYMin, _titleOffestYMax);
            double imageOffsetX = r.Next(_imageOffestXMin, _imageOffestXMax);
            double imageOffsetY = r.Next(_imageOffestYMin, _imageOffestYMax);

            renderTemplate.ApplyImage(_coverImagePath);
            renderTemplate.ApplyTitle(_title);
            renderTemplate.FormatTitle(titleRotation, titleOffsetX, titleOffsetY);
            renderTemplate.FormatImage(imageRotation, imageOffsetX, imageOffsetY);

            renderTemplate.ResolveFonts(_contentPath);

            UserControl renderControl = (UserControl)renderTemplate;

            // Update layout after all the details have been applied.
            renderControl.Measure(new Size(renderControl.Width, renderControl.Height));
            renderControl.Arrange(new Rect(new Size(renderControl.Width, renderControl.Height)));

            // Render the image to a bitmap.
            RenderTargetBitmap bitmapRenderer = new RenderTargetBitmap((int)renderControl.ActualWidth, (int)renderControl.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmapRenderer.Render(renderControl);

            // Encode the bitmap as png.
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bitmapRenderer));

            // Output the png to the file.
            using (MemoryStream memoryStream = new MemoryStream())
            {
                png.Save(memoryStream);
                this.imageBuffer = memoryStream.ToArray();
            }

            if (bitmapRenderer.Dispatcher.Thread.IsAlive)
            {
                bitmapRenderer.Dispatcher.InvokeShutdown();
            }
        }
    }
}
