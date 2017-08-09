#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Constant.cs </Name>
//         <Created> 08/08/17 6:44:48 PM </Created>
//         <Key> 00f48db3-ad03-4ed7-b163-5b9124afba84 </Key>
//     </File>
//     <Summary>
//         Constant.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Http;
using System;

namespace Puppy.Swagger
{
    public static class Constant
    {
        public const string ApiDocAssetFolderPath = "apidoc";

        public static readonly string IndexHtmlPath = $"{ApiDocAssetFolderPath}/index.html";

        public static readonly string ViewerHtmlPath = $"{ApiDocAssetFolderPath}/json-viewer.html";

        public static readonly PathString ApiDocAssetRequestPath = new PathString("/.well-known/api-define/assets");

        public static readonly TimeSpan? ApiDocAssetMaxAgeResponseHeader = new TimeSpan(365, 0, 0, 0);

        public const string DefaultConfigSection = "ApiDocument";
    }
}