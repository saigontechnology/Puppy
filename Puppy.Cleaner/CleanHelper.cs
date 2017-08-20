#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> CleanHelper.cs </Name>
//         <Created> 01/08/17 1:09:09 AM </Created>
//         <Key> 4bfd203d-659a-47f7-a1f4-a66edf466826 </Key>
//     </File>
//     <Summary>
//         CleanHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Cleaner.ConsoleUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Puppy.Cleaner
{
    public static class CleanHelper
    {
        public static void CleanupFolders(string directory, params string[] searchPatterns)
        {
            var workingDirectoryInfo = new DirectoryInfo(directory);
            var cleanupFolders = searchPatterns.SelectMany(x => workingDirectoryInfo.EnumerateDirectories(x, SearchOption.AllDirectories)).ToList();

            if (cleanupFolders.Count <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Folder {directory} not have any match cleanup folder");
                Console.ResetColor();
                return;
            }

            var progressBar = new ProgressBar(cleanupFolders.Count, '#',
                "Folder {0,-" + cleanupFolders.Count.ToString().Length + "} of {1,-" + cleanupFolders.Count.ToString().Length + "} ({2,-3}%) ",
                ConsoleColor.Cyan, new ConsoleWriter());

            progressBar.Refresh(0, $"Cleanup folders in '{directory}'");

            foreach (var cleanupFolder in cleanupFolders)
            {
                progressBar.Next($"Delete folder '{cleanupFolder.Name.ConsoleNormalize()}'...");
                if (!cleanupFolder.Exists)
                {
                    continue;
                }
                try
                {
                    Directory.Delete(cleanupFolder.FullName, true);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        public static void CleanupFiles(string directory, params string[] searchPatterns)
        {
            var workingDirectoryInfo = new DirectoryInfo(directory);
            var cleanupFiles = searchPatterns.SelectMany(x => workingDirectoryInfo.EnumerateFiles(x, SearchOption.AllDirectories)).ToList();

            if (cleanupFiles.Count <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Folder {directory} not have any match cleanup files");
                Console.ResetColor();
                return;
            }

            var progressBar = new ProgressBar(cleanupFiles.Count, '#',
                "File {0,-" + cleanupFiles.Count.ToString().Length + "} of {1,-" + cleanupFiles.Count.ToString().Length + "} ({2,-3}%) ",
                ConsoleColor.Cyan, new ConsoleWriter());

            progressBar.Refresh(0, $"Cleanup files in '{directory}'");

            foreach (var cleanupFile in cleanupFiles)
            {
                progressBar.Next($"Delete folder '{cleanupFile.Name.ConsoleNormalize()}'...");
                if (!cleanupFile.Exists)
                {
                    continue;
                }
                try
                {
                    File.Delete(cleanupFile.FullName);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        ///     Cleanup Visual Studio Cache: Component Model Cache 
        /// </summary>
        /// <remarks> Need to close visual studio before cleanup </remarks>
        public static void CleanupVsCache()
        {
            List<DirectoryInfo> mefCacheFolders = GetVisualStudioMefCacheFolders();

            if (mefCacheFolders.Count <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Your machine not have any Visual Studio MEF Cache Folder");
                Console.ResetColor();
                return;
            }

            var progressBar = new ProgressBar(mefCacheFolders.Count, '#',
                "MEF Cache Folder {0,-" + mefCacheFolders.Count.ToString().Length + "} of {1,-" + mefCacheFolders.Count.ToString().Length + "} ({2,-3}%) ",
                ConsoleColor.Cyan, new ConsoleWriter());

            progressBar.Refresh(0, "Cleanup MEF cache for Visual Studio");

            foreach (var mefCacheFolder in mefCacheFolders)
            {
                progressBar.Next($"Delete folder '{mefCacheFolder.Name.ConsoleNormalize()}'...");
                if (!mefCacheFolder.Exists)
                {
                    continue;
                }
                try
                {
                    Directory.Delete(mefCacheFolder.FullName, true);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        public static List<DirectoryInfo> GetVisualStudioMefCacheFolders()
        {
            const string mefFolderName = "ComponentModelCache";
            string visualStudioAppDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\VisualStudio");

            if (!Directory.Exists(visualStudioAppDataFolderPath))
            {
                return null;
            }

            List<DirectoryInfo> mefCacheFolderInfos = Directory
                .GetDirectories(visualStudioAppDataFolderPath, mefFolderName, SearchOption.AllDirectories)
                .Select(x => new DirectoryInfo(x)).ToList();

            return mefCacheFolderInfos;
        }
    }
}