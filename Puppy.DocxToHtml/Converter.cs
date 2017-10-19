#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Converter.cs </Name>
//         <Created> 19/10/17 10:18:17 AM </Created>
//         <Key> c8bd4fcb-a4c6-4b45-b2de-d1b51a034b4c </Key>
//     </File>
//     <Summary>
//         Converter.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Puppy.DocxToHtml
{
    public class Converter
    {
        public static FileInfo ToHtml(string docxPath, string htmlPath)
        {
            var docxFileInfo = new FileInfo(docxPath);
            byte[] docxBytes = File.ReadAllBytes(docxFileInfo.FullName);
            var htmlFileInfo = new FileInfo(htmlPath);
            htmlFileInfo.Directory.Create();

            if (docxBytes.Length <= 0)
            {
                // Create file with empty content
                if (!File.Exists(htmlFileInfo.FullName))
                {
                    File.Create(htmlFileInfo.FullName);
                }

                return htmlFileInfo;
            }

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(docxBytes, 0, docxBytes.Length);
                    using (WordprocessingDocument wDoc = WordprocessingDocument.Open(memoryStream, true))
                    {
                        var imageDirectoryName = Path.Combine(htmlFileInfo.DirectoryName, $"{Path.GetFileNameWithoutExtension(htmlFileInfo.FullName)}_files");

                        int imageCounter = 0;

                        var pageTitle = Path.GetFileNameWithoutExtension(docxFileInfo.Name);

                        // TODO: Determine max-width from size of content area.
                        HtmlConverterSettings settings = new HtmlConverterSettings
                        {
                            AdditionalCss = "body { margin: 1cm auto; max-width: 20cm; padding: 0; }",
                            PageTitle = pageTitle,
                            FabricateCssClasses = true,
                            CssClassPrefix = "pt-",
                            RestrictToSupportedLanguages = false,
                            RestrictToSupportedNumberingFormats = false,
                            ImageHandler = imageInfo =>
                            {
                                ++imageCounter;

                                string extension = imageInfo.ContentType.Split('/')[1].ToLower();

                                ImageFormat imageFormat = null;

                                if (extension == "png")
                                    imageFormat = ImageFormat.Png;
                                else if (extension == "gif")
                                    imageFormat = ImageFormat.Gif;
                                else if (extension == "bmp")
                                    imageFormat = ImageFormat.Bmp;
                                else if (extension == "jpeg")
                                    imageFormat = ImageFormat.Jpeg;
                                else if (extension == "tiff")
                                {
                                    // Convert tiff to gif.
                                    extension = "gif";
                                    imageFormat = ImageFormat.Gif;
                                }
                                else if (extension == "x-wmf")
                                {
                                    extension = "wmf";
                                    imageFormat = ImageFormat.Wmf;
                                }

                                // If the image format isn't one that we expect, ignore it, and don't
                                // return markup for the link.
                                if (imageFormat == null)
                                    return null;

                                string imageFileName = $"{imageDirectoryName}/image_{imageCounter.ToString()}.{extension}";

                                try
                                {
                                    imageInfo.Bitmap.Save(imageFileName, imageFormat);
                                }
                                catch (System.Runtime.InteropServices.ExternalException)
                                {
                                    return null;
                                }

                                string imageSource = $"{htmlFileInfo.Directory.Name}/image_{imageCounter.ToString()}.{extension}";

                                XElement img =
                                    new XElement(
                                        Xhtml.img,
                                        new XAttribute(NoNamespace.src, imageSource),
                                        imageInfo.ImgStyleAttribute,
                                        imageInfo.AltText != null
                                            ? new XAttribute(NoNamespace.alt, imageInfo.AltText)
                                            : null);
                                return img;
                            }
                        };

                        XElement htmlElement = HtmlConverter.ConvertToHtml(wDoc, settings);

                        // Produce HTML document with <!DOCTYPE html > declaration to tell the
                        // browser we are using HTML5.
                        var html = new XDocument(new XDocumentType("html", null, null, null), htmlElement);

                        // Note: the xhtml returned by ConvertToHtmlTransform contains objects of type
                        // XEntity. PtOpenXmlUtil.cs define the XEntity class. See
                        // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
                        // for detailed explanation. // If you further transform the XML tree
                        // returned by ConvertToHtmlTransform, you must do it correctly, or entities
                        // will not be serialized properly.

                        var htmlString = html.ToString(SaveOptions.DisableFormatting);
                        File.WriteAllText(htmlFileInfo.FullName, htmlString, Encoding.UTF8);
                    }
                }
            }
            catch
            {
                // Ignore
            }

            return htmlFileInfo;
        }
    }
}