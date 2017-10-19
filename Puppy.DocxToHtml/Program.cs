using System;
using System.IO;
using System.Linq;

namespace Puppy.DocxToHtml
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string docxPath = args.FirstOrDefault();
            string htmlPath = args.LastOrDefault();

            if (string.IsNullOrWhiteSpace(docxPath) || !string.Equals(Path.GetExtension(docxPath), ".docx"))
            {
                throw new ArgumentException($"Docx Path '{docxPath}' is invalid.");
            }

            if (string.IsNullOrWhiteSpace(htmlPath))
            {
                htmlPath = Path.ChangeExtension(docxPath, ".html");
            }

            Converter.ToHtml(docxPath, htmlPath);
        }
    }
}