using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using WebGalleryProcessor.Model;

namespace WebGalleryProcessor
{
    internal class GalleryFeedBuilder
    {
        private String _feedTitle;
        private String _feedDescription;
        private Uri _feedUrl;
        private String _feedId;

        public GalleryFeedBuilder(String siteName, String galleriesIndexHttpPath)
        {
            var reg = new Regex(@"\s*");
            var strippedSiteName = reg.Replace(siteName, String.Empty);

            _feedTitle = String.Format(Settings.Default.Feed_TitleFormat, siteName);
            _feedDescription = String.Format(Settings.Default.Feed_DescriptionFormat, siteName);
            _feedUrl = new Uri(galleriesIndexHttpPath);
            _feedId = String.Format(Settings.Default.Feed_DescriptionFormat, strippedSiteName);
        }

        public SyndicationFeed BuildGalleryFeed(List<Gallery> galleries)
        {
            List<SyndicationItem> feedItems = new List<SyndicationItem>();
            DateTime lastUpdatedTime = DateTime.MinValue;

            // Create a new feed item for each of the galleries.
            foreach (var gallery in galleries.Where(p => p.Display).OrderByDescending(g => g.LastUpdatedTime))
            {
                StringBuilder imageLinkBuilder = new StringBuilder();
                imageLinkBuilder.Append("<a href=\"");
                imageLinkBuilder.Append(gallery.GalleryExternalEnhancedHttpPath);
                imageLinkBuilder.Append("\"><img src=\"");
                imageLinkBuilder.Append(gallery.FeedImageHttpPath);
                imageLinkBuilder.Append("\"/></a>");

                String galleryLink = String.Format("<a href=\"{0}\">{1}</a>", gallery.GalleryExternalEnhancedHttpPath, gallery.GalleryExternalEnhancedHttpPath);

                String feedItemDescription;
                if (String.IsNullOrEmpty(gallery.Description))
                {
                    feedItemDescription = String.Format("{0}<br/>{1}", imageLinkBuilder.ToString(), galleryLink);
                }
                else
                {
                    feedItemDescription = String.Format("{0}<br/>{1}<br/>{2}", imageLinkBuilder.ToString(), gallery.Description, galleryLink);
                }

                SyndicationItem galleryFeedItem = new SyndicationItem(gallery.Name, feedItemDescription, new Uri(gallery.GalleryExternalEnhancedHttpPath), String.Format("Gallery-{0}", gallery.Name), gallery.LastUpdatedTime);
                galleryFeedItem.PublishDate = gallery.CreatedTime;

                feedItems.Add(galleryFeedItem);

                if (gallery.LastUpdatedTime > lastUpdatedTime)
                {
                    lastUpdatedTime = gallery.LastUpdatedTime;
                }
            }

            SyndicationFeed feed = new SyndicationFeed(_feedTitle, _feedDescription, _feedUrl, _feedId, lastUpdatedTime, feedItems);

            return feed;
        }
    }
}
