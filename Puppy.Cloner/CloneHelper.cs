#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ClonerHelper.cs </Name>
//         <Created> 01/08/17 12:52:57 AM </Created>
//         <Key> c144a0c7-43c1-4dbe-b5f0-c1ac0ee28066 </Key>
//     </File>
//     <Summary>
//         ClonerHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.IO;
using System.Text;

namespace Puppy.Cloner
{
    public static class CloneHelper
    {
        public static void ReplaceFolderNames(string directory, string oldValue, string newValue, bool isSkipHidden)
        {
            var subDirectories = Directory.GetDirectories(directory, $"*{oldValue}*", SearchOption.AllDirectories);

            foreach (var subDirectory in subDirectories)
            {
                DirectoryInfo subDirectoryInfo = new DirectoryInfo(subDirectory);

                // Skip Hidden File
                if (isSkipHidden && subDirectoryInfo.IsHidden()) continue;

                // Replace Folder Name
                var newDirectoryName = subDirectory.Replace(oldValue, newValue);
                if (newDirectoryName != subDirectory)
                {
                    Directory.Move(subDirectory, newDirectoryName);
                }
            }

            subDirectories = Directory.GetDirectories(directory);

            foreach (var subDirectory in subDirectories)
            {
                ReplaceFolderNames(subDirectory, oldValue, newValue, isSkipHidden);
            }
        }

        public static void ReplaceFileNames(string directory, string oldValue, string newValue, bool isSkipHidden)
        {
            var files = Directory.GetFiles(directory, $"*{oldValue}*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                // Skip Hidden File
                if (isSkipHidden && fileInfo.IsHidden()) continue;

                // Replace File Name
                var newFileName = file.Replace(oldValue, newValue);
                if (newFileName != file)
                {
                    Directory.Move(file, newFileName);
                }
            }

            var subDirectories = Directory.GetDirectories(directory);

            foreach (var subDirectory in subDirectories)
            {
                ReplaceFileNames(subDirectory, oldValue, newValue, isSkipHidden);
            }
        }

        public static void ReplaceFileContents(string directory, string oldValue, string newValue, bool isSkipHidden)
        {
            var files = Directory.GetFiles(directory, $"*{oldValue}*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.EndsWith(".exe"))
                {
                    continue;
                }

                FileInfo fileInfo = new FileInfo(file);

                // Skip Hidden File
                if (isSkipHidden && fileInfo.IsHidden()) continue;

                var contents = File.ReadAllText(file);

                contents = contents.Replace(oldValue, newValue);

                File.WriteAllText(file, contents, Encoding.UTF8);
            }

            var subDirectories = Directory.GetDirectories(directory);

            foreach (var subDirectory in subDirectories)
            {
                ReplaceFileContents(subDirectory, oldValue, newValue, isSkipHidden);
            }
        }

        public static bool IsHidden(this FileSystemInfo fileSystemInfo)
        {
            bool isHidden = (fileSystemInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
            return isHidden;
        }
    }
}