using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Puppy.Cleaner
{
    internal class Program
    {
        private static readonly List<string> ListFolderSearchPattern = new List<string>
        {
            ".vs",
            "packages",
            "bin",
            "obj",
            "bld",
            "Backup",
            "_UpgradeReport_Files",
            "ipch",
            "Debug",
            "Release"
        };

        private static readonly List<string> ListFileSearchPattern = new List<string>
        {
            "*.suo",
            "*.user"
        };

        private static void Main()
        {
            ProcessCleanup();
        }

        private static void ProcessCleanup(string workSpaceFolderPath = null)
        {
            var stopwatch = Stopwatch.StartNew();

            #region Start

            workSpaceFolderPath = string.IsNullOrWhiteSpace(workSpaceFolderPath)
                ? Directory.GetCurrentDirectory()
                : workSpaceFolderPath;
            Console.WriteLine($@"Start Information");
            Console.WriteLine($@"Target folder:   ");
            Console.WriteLine($@"    {workSpaceFolderPath}");

            #endregion Start

            #region Get Info and Pre Check

            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($@"Scan to cleanup...");
            Console.ResetColor();

            var listError = new List<string>();
            var di = new DirectoryInfo(workSpaceFolderPath);
            var directories = new List<DirectoryInfo>();

            var listScanDirectoryTask = new List<Task>();
            var listScanFileTask = new List<Task>();

            foreach (var folderSearchPattern in ListFolderSearchPattern)
                listScanDirectoryTask.Add(
                    Task.Run(
                        () =>
                            directories.AddRange(di.EnumerateDirectories(folderSearchPattern,
                                SearchOption.AllDirectories))));

            var files = new List<FileInfo>();
            foreach (var fileSearchPattern in ListFileSearchPattern)
                listScanFileTask.Add(
                    Task.Run(
                        () =>
                            files.AddRange(di.EnumerateFiles(fileSearchPattern, SearchOption.AllDirectories))));

            Task.WhenAll(listScanDirectoryTask).Wait();
            if (directories.Count > 0)
                Console.WriteLine($@"{directories.Count} directories");

            Task.WhenAll(listScanFileTask).Wait();
            if (files.Count > 0)
                Console.WriteLine($@"{files.Count} files");

            var totalCleanup = directories.Count + files.Count;
            if (totalCleanup > 0)
                Console.WriteLine($@"Total cleanup is {totalCleanup}");
            if (totalCleanup <= 0)
            {
                stopwatch.Stop();
                Console.WriteLine();
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Done.");
                Console.WriteLine($@"Total time: {stopwatch.ElapsedMilliseconds / 1000:N} s");
                Console.WriteLine("All C# Projects in current folder is cleanup.");
                Console.ResetColor();
                Console.WriteLine();
                Console.Write("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            #endregion Get Info and Pre Check

            #region Process

            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Cleanup in progress...");
            Console.ResetColor();
            Console.WriteLine();

            var totalProcessed = 0;
            var lockObject = new object();
            var listProcessTask = new List<Task>();
            foreach (var directory in directories)
                listProcessTask.Add(Task.Run(() =>
                {
                    if (Directory.Exists(directory.FullName))
                        try
                        {
                            // Delete folder
                            Directory.Delete(directory.FullName, true);
                        }
                        catch (Exception ex)
                        {
                            listError.Add($"{directory.FullName}, detail: {ex.Message}");
                        }

                    // Write process
                    lock (lockObject)
                    {
                        totalProcessed += 1;
                        DrawTextProgressBar(totalProcessed, totalCleanup);
                    }
                }));

            foreach (var file in files)
                listProcessTask.Add(Task.Run(() =>
                {
                    if (File.Exists(file.FullName))
                        try
                        {
                            // Delete file
                            File.Delete(file.FullName);
                        }
                        catch (Exception ex)
                        {
                            listError.Add($"{file.FullName}, detail: {ex.Message}");
                        }

                    // Write process
                    lock (lockObject)
                    {
                        totalProcessed += 1;
                        DrawTextProgressBar(totalProcessed, totalCleanup);
                    }
                }));

            // wait for all process done
            Task.WhenAll(listProcessTask).Wait();

            #endregion Process

            stopwatch.Stop();

            #region Result

            Console.WriteLine();
            Console.WriteLine();

            if (listError.Any())
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Fail.");
                Console.WriteLine($@"Total time: {stopwatch.ElapsedMilliseconds / 1000:N} s");
                Console.WriteLine($"Finish with {listError.Count} erros below.");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red;
                var errorNo = 0;
                foreach (var error in listError)
                {
                    errorNo++;
                    Console.WriteLine($"{errorNo}. {error}");
                }
                Console.ResetColor();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Finish.");
                Console.WriteLine($@"Total time: {stopwatch.ElapsedMilliseconds / 1000:N} s");
                Console.WriteLine("All C# Projects in current folder is cleanup.");
            }

            Console.ResetColor();
            Console.WriteLine();
            Console.Write("Press any key to exit.");
            Console.ReadKey();

            #endregion Result
        }

        private static void DrawTextProgressBar(int progress, int total)
        {
            Console.ResetColor();
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("|"); //start
            Console.CursorLeft = 32;
            Console.Write("|"); //end
            Console.CursorLeft = 1;
            var onechunk = 30.0f / total;

            //draw filled part
            var position = 1;
            for (var i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (var i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress + " of " + total + "    "); //blanks at the end remove any excess
        }
    }
}