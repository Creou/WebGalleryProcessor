<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WebGalleryProcessor.Model.GalleriesModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        <%: ViewData["Message"] %></h2>
    <p>
        <a href="Galleries/Simple">Simple Galleries</a>
        <a href="Galleries/Enhanced">Enhanced Galleries</a>
    </p>
</asp:Content>
