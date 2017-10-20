using DocumentFormat.OpenXml.Packaging;
using Puppy.Core.StringUtils;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

                StringBuilder builder = new StringBuilder(docText);

                foreach (var key in data.Keys)
                {
                    builder = builder.Replace(key, data[key]);
                }

                using (var sw = new StreamWriter(doc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(builder);
                }
            }
        }
    }
}