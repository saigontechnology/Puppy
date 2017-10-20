using System;
using Puppy.Core.FileUtils;
using Puppy.Web.HtmlUtils;
using System.Collections.Generic;
using System.Text;

namespace Puppy.ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            string docxPath = "E:\\SampleConvert.docx";
            string pdfPath = @"E:\SampleConvert.pdf";
            string pdfPassword = "topnguyen";

            WordHelper.Replace(docxPath, new Dictionary<string, string>
            {
                {"{{ContactName}}", "Top Nguyen"},
                {"{{GeneratedTime}}", DateTimeOffset.UtcNow.ToString("F")},
                {"{{FinancialYear}}", "2017"},
            });

            byte[] htmlBytes = HtmlHelper.FromDocx(docxPath);
            HtmlHelper.ToPdfFromHtml(Encoding.UTF8.GetString(htmlBytes), pdfPath);
            PdfHelper.SetPassword(pdfPath, pdfPassword);
        }
    }
}