using DocumentFormat.OpenXml.Packaging;
using Puppy.Core.StringUtils;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Puppy.Core.FileUtils
{
    public class WordHelper
    {
        public static void Replace(string path, Dictionary<string, string> data)
        {
            path = path.GetFullPath();

            using (WordprocessingDocument doc = WordprocessingDocument.Open(path, true))
            {
                string docText;

                using (var sr = new StreamReader(doc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                foreach (var key in data.Keys)
                {
                    docText = new Regex(key, RegexOptions.CultureInvariant).Replace(docText, data[key]);
                }

                using (var sw = new StreamWriter(doc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
        }
    }
}