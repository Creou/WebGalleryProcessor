using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Compression;
using Ionic.Zip;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace WebGalleryProcessor.Model
{
    public class GalleriesModel
    {
        public GalleriesModel(String galleriesIndexHttpPath, IList<Gallery> galleries, SyndicationFeed feed)
        {
            GalleriesIndexHttpPath = galleriesIndexHttpPath;
            Galleries = new ReadOnlyCollection<Gallery>(galleries);
            Feed = feed;
        }

        public ReadOnlyCollection<Gallery> Galleries { get; set; }

        public SyndicationFeed Feed { get; set; }

        public String GalleriesIndexHttpPath { get; set; }
    }
}