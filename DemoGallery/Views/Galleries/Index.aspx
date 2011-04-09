<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WebGalleryProcessor.Model.GalleriesModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        <%: ViewData["Message"] %></h2>
    <div class="galleryMenu">
        <ul class="galleryindex">
            <% foreach (var gallery in Model.Galleries.Where(g => g.Display).OrderByDescending(g => g.LastUpdatedTime))
               { %>
            <li><a class="galleryLink" href="<%: gallery.GalleryInternalHttpPath %>">
                <img src="<%: gallery.CoverImageHttpPath %>" alt="<%: gallery.Name %>" title="<%: gallery.Name %>" /></a></li>
            <% } %>
        </ul>
    </div>
</asp:Content>
