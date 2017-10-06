#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> HttpResponseExtensions.cs </Name>
//         <Created> 10/09/17 8:08:21 PM </Created>
//         <Key> 89f5eb54-5a2f-4ea1-bda8-cdf2583887e2 </Key>
//     </File>
//     <Summary>
//         HttpResponseExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Puppy.Core.DictionaryUtils;
using Puppy.Core.FileUtils;
using Puppy.Core.StringUtils;
using Puppy.Web.Constants;
using System;

namespace Puppy.Web.HttpUtils
{
    public static class HttpResponseExtensions
    {
        public static ActionResult CreateResponseFile(this HttpResponse httpResponse, FileModel fileModel, FileHttpRequestMode mode)
        {
            if (fileModel == null)
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }

            string physicalFullPath = fileModel.Location.GetFullPath();

            if (!System.IO.File.Exists(physicalFullPath))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(physicalFullPath);
            string fileName = string.IsNullOrWhiteSpace(fileModel.OriginalFileName) ? string.Empty : fileModel.OriginalFileName;

            FileContentResult fileContentResult;

            // Response file as Download
            if (mode == FileHttpRequestMode.Download || fileModel.FileType != FileType.Image)
            {
                fileContentResult = new FileContentResult(fileBytes, fileModel.MimeType) { FileDownloadName = fileName };
                return fileContentResult;
            }

            // Response type as View
            httpResponse.Headers.AddOrUpdate(HeaderKey.ContentDisposition, $"inline; filename={fileName}");

            fileContentResult =
                string.Compare(fileModel.Extension, ".svg", StringComparison.OrdinalIgnoreCase) == 0
                    ? new FileContentResult(fileBytes, "application/xml")
                    : new FileContentResult(fileBytes, fileModel.MimeType);

            return fileContentResult;
        }
    }
}