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
        /// <summary>
        ///     Cleanup folders in project folders 
        /// </summary>
        /// <param name="directory">     </param>
        /// <param name="searchPatterns"></param>
        public static void CleanupFolders(string directory, params string[] searchPatterns)
        {
            var workingDirectoryInfo = new DirectoryInfo(directory);
            var cleanupFolders = searchPatterns.SelectMany(x => workingDirectoryInfo.EnumerateDirectories(x, SearchOption.AllDirectories)).ToList();

            if (!cleanupFolders.Any())
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

        /// <summary>
        ///     Cleanup files in project folder 
        /// </summary>
        /// <param name="directory">     </param>
        /// <param name="searchPatterns"></param>
        public static void CleanupFiles(string directory, params string[] searchPatterns)
        {
            var workingDirectoryInfo = new DirectoryInfo(directory);
            var cleanupFiles = searchPatterns.SelectMany(x => workingDirectoryInfo.EnumerateFiles(x, SearchOption.AllDirectories)).ToList();

            if (!cleanupFiles.Any())
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
        ///     Cleanup Visual Studio Cache - Component Model Cache &gt;&gt; Cleanup Website Cache
        ///     &gt;&gt; Temporary ASP.NET Files &gt;&gt; Team Foundation\{Version}\Cache &gt;&gt; Temp
        /// </summary>
        /// <remarks> Need to close visual studio before cleanup </remarks>
        public static void CleanupVsCache()
        {
            CleanupMefCache();

            Console.WriteLine();

            CleanupWebsiteCache();

            Console.WriteLine();

            CleanupTemporaryDotNet();

            Console.WriteLine();

            CleanupTeamFoundationServerCache();

            Console.WriteLine();

            CleanupTemp();
        }

        // Cleanup MEF Cache
        public static void CleanupMefCache()
        {
            string visualStudioAppDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\VisualStudio");
            List<DirectoryInfo> mefCacheFolders = GetListDirectoryInPath(visualStudioAppDataFolderPath, "ComponentModelCache");

            if (!mefCacheFolders.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Your machine not have any Visual Studio MEF Cache Folder");
                Console.ResetColor();
                return;
            }

            var progressBar = new ProgressBar(mefCacheFolders.Count, '#',
                "MEF Cache Folder {0,-" + mefCacheFolders.Count.ToString().Length + "} of {1,-" + mefCacheFolders.Count.ToString().Length + "} ({2,-3}%) ",
                ConsoleColor.Cyan, new ConsoleWriter());

            progressBar.Refresh(0, "Cleanup MEF cache folders...");

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

        // Cleanup Website Cache
        public static void CleanupWebsiteCache()
        {
            string websiteCacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\WebsiteCache");

            DirectoryInfo websiteCacheFolderInfo = new DirectoryInfo(websiteCacheFolder);

            if (!websiteCacheFolderInfo.Exists)
            {
                return;
            }

            var allFileSystemInfo = websiteCacheFolderInfo.GetFileSystemInfos().ToList();

            if (!allFileSystemInfo.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Your machine not have any Website Cache folders");
                Console.ResetColor();
                return;
            }

            var progressBar = new ProgressBar(allFileSystemInfo.Count, '#',
                "Website Cache Item {0,-" + allFileSystemInfo.Count.ToString().Length + "} of {1,-" + allFileSystemInfo.Count.ToString().Length + "} ({2,-3}%) ",
                ConsoleColor.Cyan, new ConsoleWriter());

            progressBar.Refresh(0, "Cleanup Website Cache folder...");

            foreach (var fileSystemInfo in allFileSystemInfo)
            {
                progressBar.Next($"Delete item '{fileSystemInfo.Name.ConsoleNormalize()}'...");
                if (!fileSystemInfo.Exists)
                {
                    continue;
                }
                try
                {
                    if (fileSystemInfo.Attributes.HasFlag(FileAttributes.Directory))
                    {
                        Directory.Delete(fileSystemInfo.FullName, true);
                    }
                    else
                    {
                        File.Delete(fileSystemInfo.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        // Cleanup Temporary ASP.NET
        public static void CleanupTemporaryDotNet()
        {
            string netFrameworkFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework");
            List<DirectoryInfo> temporaryNetFrameworkFolders = GetListDirectoryInPath(netFrameworkFolderPath, "Temporary ASP.NET Files");

            if (!temporaryNetFrameworkFolders.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Your machine not have any Temporary ASP.NET folder");
                Console.ResetColor();
                return;
            }

            var progressBar = new ProgressBar(temporaryNetFrameworkFolders.Count, '#',
                "Temporary ASP.NET folder {0,-" + temporaryNetFrameworkFolders.Count.ToString().Length + "} of {1,-" + temporaryNetFrameworkFolders.Count.ToString().Length + "} ({2,-3}%) ",
                ConsoleColor.Cyan, new ConsoleWriter());

            progressBar.Refresh(0, "Cleanup Temporary ASP.NET folders...");

            foreach (var temporaryNetFrameworkFolder in temporaryNetFrameworkFolders)
            {
                progressBar.Next($"Delete folder '{temporaryNetFrameworkFolder.Name.ConsoleNormalize()}'...");
                if (!temporaryNetFrameworkFolder.Exists)
                {
                    continue;
                }
                try
                {
                    Directory.Delete(temporaryNetFrameworkFolder.FullName, true);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        // Cleanup Team Foundation Server Cache
        public static void CleanupTeamFoundationServerCache()
        {
            string teamFoundationServerFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Team Foundation");
            List<DirectoryInfo> teamFoundationServerCacheFolders = GetListDirectoryInPath(teamFoundationServerFolder, "Cache");

            if (!teamFoundationServerCacheFolders.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Your machine not have any Team Foundation folder");
                Console.ResetColor();
                return;
            }

            var progressBar = new ProgressBar(teamFoundationServerCacheFolders.Count, '#',
                "Team Foundation folder {0,-" + teamFoundationServerCacheFolders.Count.ToString().Length + "} of {1,-" + teamFoundationServerCacheFolders.Count.ToString().Length + "} ({2,-3}%) ",
                ConsoleColor.Cyan, new ConsoleWriter());

            progressBar.Refresh(0, "Cleanup Team Foundation folders...");

            foreach (var temporaryNetFrameworkFolder in teamFoundationServerCacheFolders)
            {
                progressBar.Next($"Delete folder '{temporaryNetFrameworkFolder.Name.ConsoleNormalize()}'...");
                if (!temporaryNetFrameworkFolder.Exists)
                {
                    continue;
                }
                try
                {
                    Directory.Delete(temporaryNetFrameworkFolder.FullName, true);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        // Cleanup Temp Folder
        public static void CleanupTemp()
        {
            string tempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Temp");

            DirectoryInfo tempFolderInfo = new DirectoryInfo(tempFolderPath);

            if (!tempFolderInfo.Exists)
            {
                return;
            }

            var allFileSystemInfo = tempFolderInfo.GetFileSystemInfos().ToList();

            if (!allFileSystemInfo.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Your machine not have any item in Temp folder");
                Console.ResetColor();
                return;
            }

            var progressBar = new ProgressBar(allFileSystemInfo.Count, '#',
                "Temp Item {0,-" + allFileSystemInfo.Count.ToString().Length + "} of {1,-" + allFileSystemInfo.Count.ToString().Length + "} ({2,-3}%) ",
                ConsoleColor.Cyan, new ConsoleWriter());

            progressBar.Refresh(0, "Cleanup Temp folder...");

            foreach (var fileSystemInfo in allFileSystemInfo)
            {
                progressBar.Next($"Delete Temp item '{fileSystemInfo.Name.ConsoleNormalize()}'...");
                if (!fileSystemInfo.Exists)
                {
                    continue;
                }
                try
                {
                    if (fileSystemInfo.Attributes.HasFlag(FileAttributes.Directory))
                    {
                        Directory.Delete(fileSystemInfo.FullName, true);
                    }
                    else
                    {
                        File.Delete(fileSystemInfo.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        // Helper
        private static List<DirectoryInfo> GetListDirectoryInPath(string rootPath, string searchPattern)
        {
            if (!Directory.Exists(rootPath))
            {
                return null;
            }

            List<DirectoryInfo> mefCacheFolderInfos = Directory
                .GetDirectories(rootPath, searchPattern, SearchOption.AllDirectories)
                .Select(x => new DirectoryInfo(x)).ToList();

            return mefCacheFolderInfos;
        }
    }
}