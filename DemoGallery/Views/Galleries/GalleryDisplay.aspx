<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WebGalleryProcessor.Model.Gallery>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Name %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        <%: Model.Name %></h2>
    <div class="galleryZipDownload">
        Download:
        <ul>
            <li><a href="<%: Model.ZipLQHttpPath %>">Normal quality</a>(<%
                                                                            if (Model.ZipLQFileSizeMBytes < 1)
                                                                            {
            %>&lt;1<%
                       }
                                                                            else
                                                                            {
            %>
                <%:Model.ZipLQFileSizeMBytes%>
                <%
                    }    
                %>Mb)</li>
            <li><a href="<%: Model.ZipHQHttpPath %>">High quality</a>(<%
                                                                          if (Model.ZipHQFileSizeMBytes < 1)
                                                                          {
            %>&lt;1<%
                       }
                                                                          else
                                                                          {
            %>
                <%:Model.ZipHQFileSizeMBytes%>
                <%
                    }    
                %>Mb)</li>
        </ul>
    </div>
    <p class="galleryDescription">
        <%: Model.Description %></p>
    <div class="gallerytable">
        <ul class="gallery">
            <% var orderedGalleryItems = Model.Items.OrderBy(i => i.SortOrder);
               foreach (WebGalleryProcessor.Model.IGalleryItem item in orderedGalleryItems)
               {
                   switch (item.ItemType)
                   {
                       case WebGalleryProcessor.Model.GalleryItemType.Image:
                           WebGalleryProcessor.Model.GalleryImageItem image = (WebGalleryProcessor.Model.GalleryImageItem)item;
            %>
            <li class="galleryImage"><a href="<%: image.ScaledImageHttpPath %>" rel="lightbox-AUTOGROUP0-<%: image.FullImageHttpPath %>"
                title="<%: image.TextData %>">
                <img src="<%: image.ThumbImageHttpPath %>" title="<%: image.TextData %>" alt="<%: image.Name %>" />
            </a>
                <noscript>
                    <p>
                        [<a href='<%: image.ScaledImageHttpPath %>'>normal</a>/<a href='<%: image.FullImageHttpPath %>'>large</a>]
                    </p>
                </noscript>
            </li>
            <%
break;
                           case WebGalleryProcessor.Model.GalleryItemType.Text:
WebGalleryProcessor.Model.GalleryTextItem text = (WebGalleryProcessor.Model.GalleryTextItem)item;
            %>
            <li class="galleryText">
                <%= text.TextData %></li>
            <%
break;

                           case WebGalleryProcessor.Model.GalleryItemType.Unknown:
                           default:
break;
                       }
                   }
            %>
        </ul>
    </div>
</asp:Content>
