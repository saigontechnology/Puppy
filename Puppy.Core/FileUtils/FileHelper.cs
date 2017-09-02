#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> FileHelper.cs </Name>
//         <Created> 30/08/17 5:29:36 PM </Created>
//         <Key> f3912706-12ac-4685-a6b6-7ac383fdaa49 </Key>
//     </File>
//     <Summary>
//         FileHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Core.StringUtils;
using System.IO;

namespace Puppy.Core.FileUtils
{
    public static class FileHelper
    {
        public static void CreateIfNotExist(params string[] paths)
        {
            foreach (var path in paths)
            {
                StringHelper.CheckNullOrWhiteSpace(path);

                if (!File.Exists(path))
                {
                    File.Create(path);
                }
            }
        }

        public static bool IsLocked(string path)
        {
            StringHelper.CheckNullOrWhiteSpace(path);
            var fileInfo = new FileInfo(path);
            return fileInfo.IsLocked();
        }
    }
}