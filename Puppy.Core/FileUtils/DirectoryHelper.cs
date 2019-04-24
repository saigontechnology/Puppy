#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DirectoryHelper.cs </Name>
//         <Created> 02/09/17 9:06:20 PM </Created>
//         <Key> f96abe47-d4c5-4742-a000-4f3583b3d28d </Key>
//     </File>
//     <Summary>
//         DirectoryHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.IO;
using System.Linq;

namespace Puppy.Core.FileUtils
{
    public static class DirectoryHelper
    {
        public static void CreateIfNotExist(params string[] paths)
        {
            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        public static void Empty(params string[] paths)
        {
            foreach (var path in paths)
            {
                // Remove all files
                var listFileInDirectory = Directory.GetFiles(path);
                foreach (var filePath in listFileInDirectory)
                {
                    File.Delete(filePath);
                }

                // Remove all directories
                var listDirectoryInDirectory = Directory.GetDirectories(path);
                foreach (var directoryPath in listDirectoryInDirectory)
                {
                    Directory.Delete(directoryPath);
                }
            }
        }

        public static bool IsEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        /// <summary>
        ///     Delete directory and all files inside
        /// </summary>
        /// <param name="paths"></param>
        public static void Delete(params string[] paths)
        {
            foreach (var path in paths)
            {
                Directory.Delete(path, true);
            }
        }
    }
}