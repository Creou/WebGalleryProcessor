using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;
using System.ServiceModel.Syndication;
using WebGalleryProcessor.Model;

namespace WebGalleryProcessor
{
    public class GalleryModelBuilder
    {
        private String _contentFilePath;
        private String _baseHttpUrl;
        private String _galleryContentRootFilePath;
        private String _galleryContentRootHttpPath;

        private String _galleryHttpPathFormat;
        private String _galleryHttpEnhancedPathFormat;
        private String _galleriesIndexHttpPathFormat;
        private String _galleriesIndexHttpPath;
        private String _galleryCoverFileName;

        private String _galleryFolderOriginalImages;
        private String _galleryFolderScaledImages;
        private String _galleryFolderThumbs;

        private String _zipFileLQFormat;
        private String _zipFileHQFormat;

        private String _imageFileTypeFilter;

        private GalleryImageProcessor _imageProcessor;
        private GalleryDataProcessor _dataProcessor;
        private GalleryFeedBuilder _feedBuilder;
        private GalleryCoverRenderer _coverRenderer;

        public GalleryModelBuilder(String baseHttpUrl, String siteName, String galleryHttpRootPath, String contentFilePath, String galleryRootFilePath)
            : this(baseHttpUrl, 
                   siteName, 
                   galleryHttpRootPath,
                   contentFilePath, 
                   galleryRootFilePath,
                   Settings.Default.ImageProcess_Default_ScaledImageScaleMode,
                   Settings.Default.ImageProcess_Default_ScaledImageWidth,
                   Settings.Default.ImageProcess_Default_ScaledImageHeight,
                   Settings.Default.ImageProcess_Default_ThumbScaleMode,
                   Settings.Default.ImageProcess_Default_ThumbWidth,
                   Settings.Default.ImageProcess_Default_ThumbHeight)
        {
        }

        public GalleryModelBuilder(String baseHttpUrl, String siteName, String galleryHttpRootPath, String contentFilePath, String galleryRootFilePath, ScaleMode scaledImageScaleMode, double scaledImageWidth, double scaledImagedHeight, ScaleMode thumbScaleMode, double thumbImageWidth, double thumbImagedHeight)
        {
            _contentFilePath = contentFilePath;
            _baseHttpUrl = baseHttpUrl;
            _galleryContentRootFilePath = galleryRootFilePath;
            _galleryContentRootHttpPath = galleryHttpRootPath;

            _galleryHttpPathFormat = Settings.Default.Http_GalleryHttpPathFormat;
            _galleryHttpEnhancedPathFormat = Settings.Default.Http_GalleryHttpEnhancedPathFormat;
            _galleriesIndexHttpPathFormat = Settings.Default.Http_GalleriesIndexHttpPathFormat;
            _galleryCoverFileName = Settings.Default.File_GalleryCoverFileName;

            _imageFileTypeFilter = Settings.Default.Image_FileTypeFilter;

            _galleryFolderOriginalImages = Settings.Default.Folders_OringinalImages;
            _galleryFolderScaledImages = Settings.Default.Folders_ScaledImages;
            _galleryFolderThumbs = Settings.Default.Folders_Thumbs;

            _zipFileLQFormat = Settings.Default.Zip_FileNameFormat_LowQuality;
            _zipFileHQFormat = Settings.Default.Zip_FileNameFormat_HighQuality;

            _galleriesIndexHttpPath = String.Format(_galleriesIndexHttpPathFormat, _baseHttpUrl);

            _imageProcessor = new GalleryImageProcessor(scaledImageScaleMode, scaledImageWidth, scaledImagedHeight, thumbScaleMode, thumbImageWidth, thumbImagedHeight);
            _dataProcessor = new GalleryDataProcessor();
            _feedBuilder = new GalleryFeedBuilder(siteName, _galleriesIndexHttpPath);
            _coverRenderer = new GalleryCoverRenderer(_contentFilePath);
        }

        public GalleriesModel BuildGalleriesModel()
        {
            List<Gallery> galleries = new List<Gallery>();

            String[] galleryPaths = Directory.GetDirectories(_galleryContentRootFilePath);

            foreach (String galleryPath in galleryPaths)
            {
                // Create a new gallery for each directory.
                Gallery gallery = new Gallery();

                // Get the directory name to use as the gallery name.
                DirectoryInfo galleryDirectoryInfo = new DirectoryInfo(galleryPath);
                gallery.FolderName = galleryDirectoryInfo.Name;
                gallery.Name = galleryDirectoryInfo.Name;

                // We set the last updated time to now by default, but this will be read from the data file later if it exists.
                gallery.LastUpdatedTime = DateTime.Now;
                gallery.CreatedTime = DateTime.Now;

                gallery.CoverImageHttpPath = Path.Combine(_galleryContentRootHttpPath, String.Format("{0}/{1}", gallery.FolderName, _galleryCoverFileName));
                gallery.FeedImageHttpPath = _baseHttpUrl + Path.Combine(_galleryContentRootHttpPath, String.Format("{0}/{1}", gallery.FolderName, _galleryCoverFileName));
                gallery.CoverImageFilePath = Path.Combine(_galleryContentRootFilePath, String.Format(@"{0}\{1}", gallery.FolderName, _galleryCoverFileName));

                String imagesPath = Path.Combine(galleryPath, _galleryFolderOriginalImages);
                String[] imageFilePaths = Directory.GetFiles(imagesPath, _imageFileTypeFilter);

                // Create zip paths and files.
                String zipFileNameLQ = String.Format(_zipFileLQFormat, gallery.FolderName);
                String zipFileNameHQ = String.Format(_zipFileHQFormat, gallery.FolderName);
                String zipFileFullPathLQ = Path.Combine(galleryPath, zipFileNameLQ);
                String zipFileFullPathHQ = Path.Combine(galleryPath, zipFileNameHQ);
                ZipFile zipFileLQ = null;
                ZipFile zipFileHQ = null;
                if (!File.Exists(zipFileFullPathLQ))
                {
                    zipFileLQ = new ZipFile(zipFileFullPathLQ);
                    zipFileLQ.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                }
                if (!File.Exists(zipFileFullPathHQ))
                {
                    zipFileHQ = new ZipFile(zipFileFullPathHQ);
                    zipFileHQ.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                }

                // Loop over all of the images found in the path and create 
                // items for each one with the required data.
                foreach (String imagePath in imageFilePaths)
                {
                    GalleryImageItem image = new GalleryImageItem();
                    image.Name = Path.GetFileName(imagePath);

                    image.FullImageHttpPath = Path.Combine(_galleryContentRootHttpPath, String.Format("{0}/{1}/{2}", gallery.FolderName, _galleryFolderOriginalImages, image.Name));
                    image.ScaledImageHttpPath = Path.Combine(_galleryContentRootHttpPath, String.Format("{0}/{1}/{2}", gallery.FolderName, _galleryFolderScaledImages, image.Name));
                    image.ThumbImageHttpPath = Path.Combine(_galleryContentRootHttpPath, String.Format("{0}/{1}/{2}", gallery.FolderName, _galleryFolderThumbs, image.Name));

                    image.FullImageFileLocation = Path.Combine(_galleryContentRootFilePath, String.Format(@"{0}\{1}\{2}", gallery.FolderName, _galleryFolderOriginalImages, image.Name));
                    image.ScaledImageFileLocation = Path.Combine(_galleryContentRootFilePath, String.Format(@"{0}\{1}\{2}", gallery.FolderName, _galleryFolderScaledImages, image.Name));
                    image.ThumbImageFileLocation = Path.Combine(_galleryContentRootFilePath, String.Format(@"{0}\{1}\{2}", gallery.FolderName, _galleryFolderThumbs, image.Name));

                    // Alternative code that maps http paths to local file system.
                    //image.FullImageFileLocation = HttpContext.Current.Server.MapPath(image.FullImageHttpPath);
                    //image.ScaledImageFileLocation = HttpContext.Current.Server.MapPath(image.ScaledImageHttpPath);
                    //image.ThumbImageFileLocation = HttpContext.Current.Server.MapPath(image.ThumbImageHttpPath);

                    // Process each image to create scaled copies and thumbnails.
                    _imageProcessor.ProcessImage(image);

                    // If zip files are required add each image to the appropriate zips (either scaled or full size).
                    if (zipFileLQ != null)
                    {
                        zipFileLQ.AddFile(image.ScaledImageFileLocation, String.Empty);
                    }
                    if (zipFileHQ != null)
                    {
                        zipFileHQ.AddFile(image.FullImageFileLocation, String.Empty);
                    }

                    gallery.Items.Add(image);
                }

                // Build the file path for the gallery data file.
                String galleryDataFileName = String.Format(Settings.Default.File_GalleryDataFileNameFormat, gallery.UrlFriendlyName);
                gallery.DataFilePath = Path.Combine(_galleryContentRootFilePath, galleryDataFileName);

                /* Attempt to read data from the data file.
                 * This data will extend information already 
                 * gathered from the gallery paths and includes 
                 * textual contant such as descriptions and comments.
                 * 
                 * Note: This may potentially change the gallery name. 
                 * The folder name will not be changed, but 
                 * the display name and Url friendly name may 
                 * be changed.
                 */ 
                _dataProcessor.ReadGalleryData(gallery);

                gallery.GalleryExternalHttpPath = String.Format(_galleryHttpPathFormat, _baseHttpUrl, gallery.UrlFriendlyName);
                gallery.GalleryInternalHttpPath = String.Format(_galleryHttpPathFormat, String.Empty, gallery.UrlFriendlyName);
                gallery.GalleryExternalEnhancedHttpPath = String.Format(_galleryHttpEnhancedPathFormat, _baseHttpUrl, gallery.UrlFriendlyName);
                gallery.GalleryInternalEnhancedHttpPath = String.Format(_galleryHttpEnhancedPathFormat, String.Empty, gallery.UrlFriendlyName);

                // Check if we need to create a gallery cover.
                if (!File.Exists(gallery.CoverImageFilePath) && gallery.Items.Count > 0)
                {
                    GalleryImageItem galleryCoverImage = null;

                    // First we attempt to resolve the gallery cover from the information known about the images.
                    galleryCoverImage = gallery.Items.FirstOrDefault(i =>
                    {
                        if (i.ItemType == GalleryItemType.Image)
                        {
                            var image = i as GalleryImageItem;
                            if (image != null)
                            {
                                return image.IsCover;
                            }
                        }
                        return false;
                    }) as GalleryImageItem;

                    //if (galleryCoverImage == null)
                    //{
                    //    // If the first attempt failed, next we try to resolve the image 
                    //    // by looking for an image named:
                    //    //      Cover
                    //    //      GalleryCover

                    //    galleryCoverImage = gallery.Items.FirstOrDefault(i =>
                    //    {
                    //        if (i.ItemType == GalleryItemType.Image)
                    //        {
                    //            var image = i as GalleryImageItem;
                    //            if (String.Compare(image.Name, "cover.jpg", true) == 0 ||
                    //                String.Compare(image.Name, "gallerycover.jpg", true) == 0)
                    //            {
                    //                return image.IsCover;
                    //            }
                    //        }
                    //        return false;
                    //    }) as GalleryImageItem;
                    //}

                    if (galleryCoverImage == null)
                    {
                        // Finally, as a last resort to resolve the cover image, just select the first image in the gallery.
                        galleryCoverImage = (GalleryImageItem)gallery.Items.FirstOrDefault(i => i.ItemType == GalleryItemType.Image);
                    }

                    if (galleryCoverImage != null)
                    {
                        // Now we have the image, render the cover from it.
                        _coverRenderer.GenerateCoverImage(gallery.Name, gallery.CoverImageFilePath, galleryCoverImage.FullImageFileLocation);

                        // Mark this as the cover image. (This will be written to the data file later to guide the administrator if they need to change the cover image.)
                        galleryCoverImage.IsCover = true;
                    }
                }

                // If zip files are required, write the data out to disk.
                if (zipFileLQ != null)
                {
                    zipFileLQ.Save();
                }
                if (zipFileHQ != null)
                {
                    zipFileHQ.Save();
                }

                // Populate some information about the zips on the gallery object for use later when the gallery is displayed.
                gallery.ZipLQFileSizeBytes = new FileInfo(zipFileFullPathLQ).Length;
                gallery.ZipHQFileSizeBytes = new FileInfo(zipFileFullPathHQ).Length;
                gallery.ZipLQHttpPath = Path.Combine(_galleryContentRootHttpPath, String.Format("{0}/{1}", gallery.FolderName, zipFileNameLQ));
                gallery.ZipHQHttpPath = Path.Combine(_galleryContentRootHttpPath, String.Format("{0}/{1}", gallery.FolderName, zipFileNameHQ));
                gallery.ZipLQFileLocation = Path.Combine(_galleryContentRootFilePath, String.Format(@"{0}\{1}", gallery.FolderName, zipFileNameLQ));
                gallery.ZipHQFileLocation = Path.Combine(_galleryContentRootFilePath, String.Format(@"{0}\{1}", gallery.FolderName, zipFileNameHQ));

                _dataProcessor.WriteGalleriesData(gallery);

                // Add the gallery to the list.
                galleries.Add(gallery);
            }

            SyndicationFeed feed = _feedBuilder.BuildGalleryFeed(galleries);

            GalleriesModel model = new GalleriesModel(_galleriesIndexHttpPath, galleries, feed);
            return model;
        }
    }
}
