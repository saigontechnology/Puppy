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

using Puppy.Cloner.ConsoleUtils;
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Puppy.Cloner
{
    public static class CloneHelper
    {
        public static void ReplaceFolderNames(string directory, string oldValue, string newValue, bool isSkipHidden)
        {
            var subDirectories = Directory.GetDirectories(directory, $"*{oldValue}*", SearchOption.TopDirectoryOnly);

            if (subDirectories.Length <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Folder {directory} not have any sub folder");
                Console.ResetColor();
                return;
            }

            Console.ResetColor();
            var progressBar = new ProgressBar(subDirectories.Length, '#',
                "Folder {0,-" + subDirectories.Length.ToString().Length + "} of {1,-" + subDirectories.Length.ToString().Length + "} ({2,-3}%) ",
                ConsoleColor.Cyan, new ConsoleWriter());

            progressBar.Refresh(0, $"Replace name for folders in '{directory}'");

            foreach (var subDirectory in subDirectories)
            {
                DirectoryInfo subDirectoryInfo = new DirectoryInfo(subDirectory);

                progressBar.Next($"Replace name for '{subDirectoryInfo.Name.ConsoleNormalize()}'...");

                ReplaceFolderNames(subDirectory, oldValue, newValue, isSkipHidden);

                // Skip Hidden File
                if (isSkipHidden && subDirectoryInfo.IsHidden())
                {
                    continue;
                }

                // Replace Folder Name
                var folderName = Path.GetDirectoryName(subDirectory);

                var newDirectoryName = string.IsNullOrWhiteSpace(folderName)
                    ? subDirectoryInfo.FullName.Replace(oldValue, newValue)
                    : Path.Combine(folderName, subDirectoryInfo.Name.Replace(oldValue, newValue));

                if (Directory.Exists(newDirectoryName))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"Folder '{newDirectoryName}' already exist, replace progress skipped!");
                    Console.ResetColor();
                    continue;
                }

                try
                {
                    Directory.Move(subDirectory, newDirectoryName);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {e.Message}");
                    Console.ResetColor();
                }
            }
        }

        public static void ReplaceFileNames(string directory, string oldValue, string newValue, bool isSkipHidden)
        {
            var files = Directory.GetFiles(directory, $"*{oldValue}*", SearchOption.AllDirectories);

            if (files.Length <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Folder {directory} not have any file");
                Console.ResetColor();
                return;
            }

            Console.ResetColor();
            var progressBar = new ProgressBar(files.Length, '#',
                "File {0,-" + files.Length.ToString().Length + "} of {1,-" + files.Length.ToString().Length + "} ({2,-3}%) ", ConsoleColor.Cyan,
                new ConsoleWriter());

            progressBar.Refresh(0, $"Replace name for files in '{directory}'");

            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                progressBar.Next($"Replace name for '{fileInfo.Name.ConsoleNormalize()}'...");

                // Skip Hidden File
                if (isSkipHidden && fileInfo.IsHidden())
                {
                    continue;
                }

                // Replace File Name
                var folderName = Path.GetDirectoryName(file);

                var newFileName = string.IsNullOrWhiteSpace(folderName)
                    ? fileInfo.FullName.Replace(oldValue, newValue)
                    : Path.Combine(folderName, fileInfo.Name.Replace(oldValue, newValue));

                if (File.Exists(newFileName))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"File '{newFileName}' already exist, replace progress skipped!");
                    Console.ResetColor();
                    continue;
                }

                try
                {
                    Directory.Move(file, newFileName);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {e.Message}");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="directory">      </param>
        /// <param name="oldValue">       </param>
        /// <param name="newValue">       </param>
        /// <param name="isSkipHidden">   </param>
        /// <param name="ignoreExtension">
        ///     Ignore file by extension, ex: .exe, .ico and so on (compare extension by <c> Ordinal
        ///     Ignore Case </c>)
        /// </param>
        public static void ReplaceFileContents(string directory, string oldValue, string newValue, bool isSkipHidden, params string[] ignoreExtension)
        {
            var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);

            if (files.Length <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Folder {directory} not have any file");
                Console.ResetColor();
                return;
            }

            Console.ResetColor();

            var progressBar = new ProgressBar(files.Length, '#',
                "File {0,-" + files.Length.ToString().Length + "} of {1,-" + files.Length.ToString().Length + "} ({2,-3}%) ", ConsoleColor.Cyan,
                new ConsoleWriter());

            progressBar.Refresh(0, $"Replace content for files in '{directory}'");

            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                progressBar.Next($"Replace content for '{fileInfo.Name.ConsoleNormalize()}'...");

                // Ignore file by Extensions, ex: .exe, .png, .ico and so on.
                if (ignoreExtension?.Any() == true)
                {
                    if (ignoreExtension.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                }

                // Skip Hidden File
                if (isSkipHidden && fileInfo.IsHidden())
                {
                    continue;
                }

                var contents = File.ReadAllText(file);

                contents = contents.Replace(oldValue, newValue);

                try
                {
                    File.WriteAllText(file, contents, Encoding.UTF8);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {e.Message}");
                    Console.ResetColor();
                }
                catch (SecurityException e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {e.Message}");
                    Console.ResetColor();
                }
            }
        }

        public static bool IsHidden(this FileSystemInfo fileSystemInfo)
        {
            bool isHidden = (fileSystemInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
            return isHidden;
        }
    }
}