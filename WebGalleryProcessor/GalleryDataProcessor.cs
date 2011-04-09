using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using WebGalleryProcessor.Model;
using System.Globalization;

namespace WebGalleryProcessor
{
    internal class GalleryDataProcessor
    {
        private const String _elementName_Root = "Gallery";
        private const String _elementName_Image = "Image";
        private const String _elementName_Text = "Text";

        private const String _attributeName_GalleryName = "Name";
        private const String _attributeName_GalleryLastUpdateTime = "LastUpdatedTime";
        private const String _attributeName_GalleryCreatedTime = "CreatedTime";
        private const String _attributeName_Description = "Description";
        private const String _attributeName_ImageName = "Name";
        private const String _attributeName_ImageIsCover = "IsCover";
        private const String _attributeName_ImageComment = "Comment";

        public void ReadGalleryData(Gallery gallery)
        {
            if (File.Exists(gallery.DataFilePath))
            {
                XmlReader reader = XmlReader.Create(gallery.DataFilePath);
                XmlDocument data = new XmlDocument();
                data.Load(reader);
                reader.Close();

                // This is what we need to do, but with proper error checking for null and invalid data.
                XmlNode galleryNode = data.SelectSingleNode(_elementName_Root);
                if (galleryNode != null)
                {
                    XmlNode galleryNameNode = galleryNode.Attributes.GetNamedItem(_attributeName_GalleryName);
                    if (galleryNameNode != null)
                    {
                        gallery.Name = galleryNameNode.Value;
                    }

                    XmlNode galleryDescriptionNode = galleryNode.Attributes.GetNamedItem(_attributeName_Description);
                    if (galleryDescriptionNode != null)
                    {
                        gallery.Description = galleryDescriptionNode.Value;
                    }

                    XmlNode galleryLastUpdatedTimeNode = galleryNode.Attributes.GetNamedItem(_attributeName_GalleryLastUpdateTime);
                    if (galleryLastUpdatedTimeNode != null)
                    {
                        gallery.LastUpdatedTime = DateTime.Parse(galleryLastUpdatedTimeNode.Value, CultureInfo.GetCultureInfo("en-GB"));
                    }

                    XmlNode galleryCreatedTimeNode = galleryNode.Attributes.GetNamedItem(_attributeName_GalleryCreatedTime);
                    if (galleryCreatedTimeNode != null)
                    {
                        gallery.CreatedTime = DateTime.Parse(galleryCreatedTimeNode.Value, CultureInfo.GetCultureInfo("en-GB"));
                    }

                    int nodeSortOrder = 0;
                    foreach (var childItem in galleryNode.ChildNodes)
                    {
                        XmlNode childNode = childItem as XmlNode;
                        if (childNode != null)
                        {
                            switch (childNode.Name)
                            {
                                case _elementName_Image:
                                    XmlNode imageNameNode = childNode.Attributes.GetNamedItem(_attributeName_ImageName);
                                    if (imageNameNode != null)
                                    {
                                        String imageName = imageNameNode.Value;

                                        // Check if there is an image item that matches this data node.
                                        var imageItem = gallery.Items.SingleOrDefault(i =>
                                        {
                                            if (i.ItemType == GalleryItemType.Image)
                                            {
                                                var image = i as GalleryImageItem;
                                                if (image.Name == imageName)
                                                {
                                                    return true;
                                                }
                                            }

                                            return false;
                                        }) as GalleryImageItem;

                                        // If we found a matching image, load the data for it from the XML.
                                        if (imageItem != null)
                                        {
                                            imageItem.SortOrder = nodeSortOrder;

                                            XmlNode imageCommentNode = childNode.Attributes.GetNamedItem(_attributeName_ImageComment);
                                            if (imageCommentNode != null)
                                            {
                                                imageItem.TextData = imageCommentNode.Value;
                                            }

                                            XmlNode imageIsCoverNode = childNode.Attributes.GetNamedItem(_attributeName_ImageIsCover);
                                            if (imageIsCoverNode != null)
                                            {
                                                imageItem.IsCover = bool.Parse(imageIsCoverNode.Value);
                                            }
                                        }
                                    }

                                    break;
                                case _elementName_Text:
                                    // Create new text items when processed in the XML data.
                                    GalleryTextItem textItem = new GalleryTextItem();
                                    textItem.SortOrder = nodeSortOrder;
                                    textItem.TextData = childNode.InnerText;

                                    gallery.Items.Add(textItem);

                                    break;
                                default:
                                    // Just ignore unknown element types.
                                    break;
                            }
                        }

                        nodeSortOrder++;
                    }
                }
            }
        }

        public void WriteGalleriesData(Gallery gallery)
        {
            if (!File.Exists(gallery.DataFilePath))
            {
                XmlDocument galleryData = new XmlDocument();

                XmlElement galleryElement = galleryData.CreateElement(_elementName_Root);
                galleryData.AppendChild(galleryElement);

                XmlAttribute nameAttribute = galleryData.CreateAttribute(_attributeName_GalleryName);
                nameAttribute.Value = gallery.Name;
                galleryElement.Attributes.Append(nameAttribute);

                XmlAttribute descriptionAttribute = galleryData.CreateAttribute(_attributeName_Description);
                descriptionAttribute.Value = gallery.Description;
                galleryElement.Attributes.Append(descriptionAttribute);

                XmlAttribute lastUpdatedTimeAttribute = galleryData.CreateAttribute(_attributeName_GalleryLastUpdateTime);
                lastUpdatedTimeAttribute.Value = gallery.LastUpdatedTime.ToString(CultureInfo.GetCultureInfo("en-GB"));
                galleryElement.Attributes.Append(lastUpdatedTimeAttribute);

                XmlAttribute createdTimeAttribute = galleryData.CreateAttribute(_attributeName_GalleryCreatedTime);
                createdTimeAttribute.Value = gallery.CreatedTime.ToString(CultureInfo.GetCultureInfo("en-GB"));
                galleryElement.Attributes.Append(createdTimeAttribute);

                int textItemCount = 0;
                foreach (var galleryItem in gallery.Items.OrderBy(i => i.SortOrder))
                {
                    XmlElement galleryItemElement = null;
                    switch (galleryItem.ItemType)
                    {
                        case GalleryItemType.Unknown:
                            break;
                        case GalleryItemType.Image:
                            galleryItemElement = galleryData.CreateElement(_elementName_Image);
                            galleryElement.AppendChild(galleryItemElement);

                            XmlAttribute galleryItemNameAttribute = galleryData.CreateAttribute(_attributeName_ImageName);
                            galleryItemNameAttribute.Value = ((GalleryImageItem)galleryItem).Name;
                            galleryItemElement.Attributes.Append(galleryItemNameAttribute);

                            XmlAttribute galleryItemTextDataAttribute = galleryData.CreateAttribute(_attributeName_ImageComment);
                            galleryItemTextDataAttribute.Value = galleryItem.TextData;
                            galleryItemElement.Attributes.Append(galleryItemTextDataAttribute);

                            if (((GalleryImageItem)galleryItem).IsCover)
                            {
                                XmlAttribute galleryItemIsCoverAttribute = galleryData.CreateAttribute(_attributeName_ImageIsCover);
                                galleryItemIsCoverAttribute.Value = true.ToString();
                                galleryItemElement.Attributes.Append(galleryItemIsCoverAttribute);
                            }
                            break;
                        case GalleryItemType.Text:
                            galleryItemElement = galleryData.CreateElement(_elementName_Text);
                            galleryElement.AppendChild(galleryItemElement);

                            var cdata = galleryData.CreateCDataSection(galleryItem.TextData);
                            galleryItemElement.AppendChild(cdata);

                            textItemCount++;

                            break;
                        default:
                            break;
                    }
                }

                if (textItemCount == 0)
                {
                    // If no text items exist in the gallery, we insert an empty one at the end
                    // This is to provide an example to the administor of how to include text in the gallery.

                    //XmlComment textExplanationCommentElement = galleryData.CreateComment("You can use Text elements to include blocks of information within your gallery. Images and text will appear in the order they are defined.");
                    //galleryElement.AppendChild(textExplanationCommentElement);

                    XmlElement placeholderTextItemElement = galleryData.CreateElement(_elementName_Text);
                    galleryElement.AppendChild(placeholderTextItemElement);

                    XmlCDataSection placeholderCData = galleryData.CreateCDataSection(String.Empty);
                    placeholderTextItemElement.AppendChild(placeholderCData);
                }

                using (XmlWriter writer = XmlWriter.Create(gallery.DataFilePath))
                {
                    galleryData.WriteTo(writer);
                }
            }
        }
    }
}
