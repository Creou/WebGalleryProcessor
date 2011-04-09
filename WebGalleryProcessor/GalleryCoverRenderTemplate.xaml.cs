using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Globalization;

namespace WebGalleryProcessor
{
    /// <summary>
    /// Class to provide interaction code with the cover render template.
    /// </summary>
    public partial class GalleryCoverRenderTemplate : UserControl, IRenderTemplate
    {
        public GalleryCoverRenderTemplate()
        {
            InitializeComponent();
        }

        public void ApplyImage(String fileName)
        {
            BitmapImage image = new BitmapImage(new Uri(fileName));
            imgThumb.Source = image;
        }

        public void ApplyTitle(String title)
        {
            lblTitle.Content = title;
        }

        public void FormatTitle(double titleRotation, double titleOffsetX, double titleOffsetY)
        {
            rotationLabelTransform.Angle = titleRotation;
            lblTitle.Margin = new Thickness(titleOffsetX, titleOffsetY, 0, 0);
        }

        public void FormatImage(double imageRotation, double imageOffsetX, double imageOffsetY)
        {

            /* Calculating the bounding dimensions:
             * 
             *    + - - - - - - - - - * - - - - + -
             *    :            A  *    *        :
             *    :           *         *       :
             *    :       *  W           *      :
             *    :   *                   *     : H·cos(A)
             *    *                      H *    :
             *    :*                        *   :
             *    : *                        * A:
             *    :A *                        * :
             *    :   *                        *:
             *    :    * H                      * -
             *    :     *             W     *   :
             *    :      *              *       :
             *    :       *         *           : W·sin(A)
             *    :        *    *  A            :
             *    + - - - - * - - - - - - - - - + -
             *    | H·sin(A)|      W·cos(A)     |
             *   
             * http://www.mathhelpforum.com/math-help/pre-calculus/14540-rectangle-rotation.html
             * 
             */

            double absoluteRotation = Math.Abs(imageRotation);

            this.Width = (bdrThumbBorder.Height * Math.Sin(AngleConversions.DegreesToRadians(absoluteRotation)))
                         +
                         (bdrThumbBorder.Width * Math.Cos(AngleConversions.DegreesToRadians(absoluteRotation)))
                         + 20;

            this.Height = (bdrThumbBorder.Width * Math.Sin(AngleConversions.DegreesToRadians(absoluteRotation)))
                          +
                          (bdrThumbBorder.Height * Math.Cos(AngleConversions.DegreesToRadians(absoluteRotation)))
                          + 20;

            rotationTransform.Angle = imageRotation;

            var newMargin = new Thickness(bdrThumbBorder.Margin.Left + imageOffsetX, bdrThumbBorder.Margin.Top + imageOffsetY, bdrThumbBorder.Margin.Right, bdrThumbBorder.Margin.Bottom);
            bdrThumbBorder.Margin = newMargin;
        }

        public void ResolveFonts(String fontResourcePath)
        {
            /* We attempt to resolve the font by using the 
             * requested family names to search through 
             * the resource path.
             */
            var resourceFonts = Fonts.GetFontFamilies(fontResourcePath);

            var xmlLanguage = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

            String[] requestedFamilyNames = lblTitle.FontFamily.Source.Split(',');
            var requestedFontEnumerator = requestedFamilyNames.GetEnumerator();
            FontFamily matchingFont = null;

            while (matchingFont == null && requestedFontEnumerator.MoveNext())
            {
                var requestedFontFamily = new KeyValuePair<XmlLanguage, string>(xmlLanguage, ((String)requestedFontEnumerator.Current).Trim());    
                matchingFont = resourceFonts.FirstOrDefault(f => f.FamilyNames.Contains(requestedFontFamily));
            }

            if (matchingFont != null)
            {
                lblTitle.FontFamily = matchingFont;
            }
        }
    }
}
