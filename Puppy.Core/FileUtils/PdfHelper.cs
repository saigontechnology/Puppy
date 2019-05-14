#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> PdfHelper.cs </Name>
//         <Created> 19/10/17 2:20:05 PM </Created>
//         <Key> 5c013ee6-3944-45dc-a322-475c8c420c11 </Key>
//     </File>
//     <Summary>
//         PdfHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using Puppy.Core.StringUtils;
using System.IO;
using iTextSharp.text;

namespace Puppy.Core.FileUtils
{
    public class PdfHelper
    {
        public static void SetPassword(string pdfPath, string password)
        {
            // Generate Temp File for Html
            string temp = Path.GetTempFileName();
            var outputPath = Path.ChangeExtension(temp, ".pdf");
            File.Move(temp, outputPath);

            try
            {
                // Convert
                SetPassword(pdfPath, password, outputPath);

                // Replace original pdf by result
                FileHelper.SafeDelete(pdfPath);
                File.Move(outputPath, pdfPath);
            }
            finally
            {
                // Remove Temp
                FileHelper.SafeDelete(outputPath);
            }
        }

        public static void SetPassword(string pdfPath, string password, string outputPath)
        {
            pdfPath = pdfPath?.GetFullPath();
            outputPath = outputPath?.GetFullPath();

            using (Stream input = new FileStream(pdfPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (Stream output = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfReader reader = new PdfReader(input);
                    PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_SCREENREADERS);
                }
            }
        }

        /// <summary>
        /// Merge pdf files.
        /// </summary>
        /// <param name="sourceFiles">PDF files being merged.</param>
        /// <returns></returns>
        public static byte[] MergeFiles(List<byte[]> sourceFiles)
        {
            Document document = new Document();
            using (MemoryStream ms = new MemoryStream())
            {
                PdfCopy copy = new PdfCopy(document, ms);
                document.Open();
                int documentPageCounter = 0;

                // Iterate through all pdf documents
                for (int fileCounter = 0; fileCounter < sourceFiles.Count; fileCounter++)
                {
                    // Create pdf reader
                    PdfReader reader = new PdfReader(sourceFiles[fileCounter]);
                    int numberOfPages = reader.NumberOfPages;

                    // Iterate through all pages
                    for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                    {
                        documentPageCounter++;
                        PdfImportedPage importedPage = copy.GetImportedPage(reader, currentPageIndex);
                        PdfCopy.PageStamp pageStamp = copy.CreatePageStamp(importedPage);

                        //// Write header
                        //ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                        //    new Phrase("PDF Merger "), importedPage.Width / 2, importedPage.Height - 30,
                        //    importedPage.Width < importedPage.Height ? 0 : 1);

                        // Write footer
                        ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                            new Phrase(String.Format("Page {0}", documentPageCounter)), importedPage.Width / 2, 30,
                            importedPage.Width < importedPage.Height ? 0 : 1);

                        pageStamp.AlterContents();

                        copy.AddPage(importedPage);
                    }

                    copy.FreeReader(reader);
                    reader.Close();
                }

                document.Close();
                return ms.GetBuffer();
            }
        }
    }
}