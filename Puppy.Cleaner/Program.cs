using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Puppy.Cleaner
{
    internal class Program
    {
        private static List<string> _listSearchPattern = new List<string>
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
            "Release",
            "*.suo",
            "*.user"
        };

        private static void Main()
        {
            Console.Title = "[Puppy Cleaner] - Fastest way to cleanup project!";

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Welcome to Puppy Cleaner!");

            // Working Folder
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[!] Set empty for [Puppy Cleaner] working in current folder");
            Console.ResetColor();

            // New Project folder path
            Console.Write("What is your project's folder [path]: ");
            var workingFolderPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(workingFolderPath))
            {
                workingFolderPath = Directory.GetCurrentDirectory();
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("> Working in: " + workingFolderPath);
            Console.ResetColor();

            // Normalized Ignore Files
            _listSearchPattern = _listSearchPattern.Distinct().ToList();
            _listSearchPattern.Sort();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[!] Puppy Cleaner cleanup by file pattern: {string.Join(", ", _listSearchPattern)} by default.");
            Console.WriteLine("[!] Set empty if default setting is enough for you. Use \",\" to add multiple pattern (ex: \".vs, *.sou\")");
            Console.ResetColor();

            Console.WriteLine();
            Console.Write("Add more [cleanup] folders or files by [regular pattern]: ");
            var searchPattern = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                var listNewSearchPattern = searchPattern.Split(',').Select(x => x.Trim()).Where(x => x.StartsWith(".")).ToList();
                _listSearchPattern.AddRange(listNewSearchPattern);

                // Normalized Ignore Files
                _listSearchPattern = _listSearchPattern.Distinct().ToList();
                _listSearchPattern.Sort();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"> Cleanup folders and files by regular pattern: {string.Join(", ", _listSearchPattern)}");
            Console.WriteLine();
            Console.ResetColor();

            // Start Process
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start cleanup progress!");
            Console.ResetColor();

            // Cleanup Folder
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Cleanup Folders...");

            Stopwatch stopwatch = Stopwatch.StartNew();

            Console.WriteLine();
            CleanHelper.CleanupFolders(workingFolderPath, _listSearchPattern.ToArray());
            Console.WriteLine();
            stopwatch.Stop();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"> Finish Cleanup Folders - {stopwatch.Elapsed.Milliseconds} ms!");
            Console.ResetColor();

            // Cleanup File
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Cleanup Files...");

            stopwatch.Restart();
            Console.WriteLine();
            CleanHelper.CleanupFiles(workingFolderPath, _listSearchPattern.ToArray());
            Console.WriteLine();
            stopwatch.Stop();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"> Finish Cleanup Files - {stopwatch.Elapsed.Milliseconds} ms!");
            Console.ResetColor();

            // Cleanup VS Cache
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Cleanup Visual Studio Cache...");
            Console.WriteLine("[!] You need to close Visual Studio to nuke the VS cache");

            stopwatch.Restart();
            Console.WriteLine();
            CleanHelper.CleanupVsCache();
            Console.WriteLine();
            stopwatch.Stop();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"> Finish Cleanup Visual Studio Cache - {stopwatch.Elapsed.Milliseconds} ms!");
            Console.ResetColor();

            // Finish
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[Done], thank for using [Puppy Cleaner]!");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}