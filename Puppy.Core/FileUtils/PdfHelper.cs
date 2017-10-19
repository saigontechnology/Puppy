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

using iTextSharp.text.pdf;
using Puppy.Core.StringUtils;
using System.IO;

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
    }
}