using Puppy.Core.FileUtils;
using Puppy.Web.HtmlUtils;
using System.Text;

namespace Puppy.ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            byte[] htmlBytes = HtmlHelper.FromDocx("E:\\SampleConvert.docx");
            HtmlHelper.ToPdfFromHtml(Encoding.UTF8.GetString(htmlBytes), @"E:\SampleConvert.pdf");
            PdfHelper.SetPassword(@"E:\SampleConvert.pdf", "topnguyen");
        }
    }
}