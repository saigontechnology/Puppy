using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Puppy.Cloner
{
    internal class Program
    {
        private static List<string> _ignoreReplaceContentFileEx = new List<string>
        {
            ".exe",
            ".ico",
            ".png",
            ".jpg",
            ".jpeg",
            ".gif",
            ".svg",
            ".bmp",
            ".tiff",
            ".mmdb",
            ".eot",
            ".ttf",
            ".woff",
            ".woff2",
            ".mp3",
            ".mp4",
            ".ogg",
            ".ogv",
            ".webm",
            ".svgz",
            ".otf",
            ".crx",
            ".xpi",
            ".safariextz",
            ".flv",
            ".f4v"
        };

        private static void Main(string[] args)
        {
            Console.Title = "[Puppy Cloner] - Fastest way to clone project!";

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Welcome to Puppy Cloner!");

            // Working Folder
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[!] Set empty for [Puppy Cloner] working in current folder");
            Console.ResetColor();

            // New Project folder path
            Console.Write("What is your [new] project's folder [path]: ");
            var workingFolderPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(workingFolderPath))
            {
                workingFolderPath = Directory.GetCurrentDirectory();
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("> Working in: " + workingFolderPath);
            Console.WriteLine();
            Console.ResetColor();

            // Old Name
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[!] Set empty for [Puppy Cloner] use working folder name as old project name");
            Console.ResetColor();

            Console.Write("What is your [old] project [name]: ");
            var oldValue = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(oldValue))
            {
                oldValue = Path.GetFileName(workingFolderPath);
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("> Old project name is: " + oldValue);
            Console.WriteLine();
            Console.ResetColor();

            // New Name
            Console.WriteLine();
            var newValue = string.Empty;
            while (string.IsNullOrWhiteSpace(newValue))
            {
                Console.Write("What is your [new] project [name]: ");
                newValue = Console.ReadLine();
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("> New project name is: " + newValue);
            Console.ResetColor();

            // Ignore Replace Content by File Extensions

            // Normalized Ignore Files
            _ignoreReplaceContentFileEx = _ignoreReplaceContentFileEx.Distinct().ToList();
            _ignoreReplaceContentFileEx.Sort();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[!] Puppy Cloner ignore replace content by file extensions: {string.Join(", ", _ignoreReplaceContentFileEx)} by default.");
            Console.WriteLine("[!] Set empty if default setting is enough for you. Use \",\" to add multiple extensions (ex: \".exe, .png\")");
            Console.ResetColor();

            Console.WriteLine();
            Console.Write("Add more [ignore] replace content by file [extension]: ");
            var ignoreFileExs = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(ignoreFileExs))
            {
                var listAddIgnoreFileByEx = ignoreFileExs.Split(',').Select(x => x.Trim()).Where(x => x.StartsWith(".")).ToList();
                _ignoreReplaceContentFileEx.AddRange(listAddIgnoreFileByEx);

                // Normalized Ignore Files
                _ignoreReplaceContentFileEx = _ignoreReplaceContentFileEx.Distinct().ToList();
                _ignoreReplaceContentFileEx.Sort();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"> Ignore replace content by file extensions: {string.Join(", ", _ignoreReplaceContentFileEx)}");
            Console.WriteLine();
            Console.ResetColor();

            // Start Process
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Start progress replace name and content from '{oldValue}' to '{newValue}', ignore replace content by extensions: {string.Join(", ", _ignoreReplaceContentFileEx)}.");
            Console.ResetColor();

            // Replace Directories
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Replace Folders Name");

            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine();
            CloneHelper.ReplaceFolderNames(workingFolderPath, oldValue, newValue, true);
            Console.WriteLine();
            stopwatch.Stop();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"> Finish Replace Folders Name - {stopwatch.Elapsed.Milliseconds} ms!");
            Console.ResetColor();

            // Replace Files Name
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Replace Files Name");

            stopwatch.Restart();
            Console.WriteLine();
            CloneHelper.ReplaceFileNames(workingFolderPath, oldValue, newValue, true);
            Console.WriteLine();
            stopwatch.Stop();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"> Finish Replace Files Name - {stopwatch.Elapsed.Milliseconds} ms!");
            Console.ResetColor();

            // Replace Content Files
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Replace Files Content");

            stopwatch.Restart();
            Console.WriteLine();
            CloneHelper.ReplaceFileContents(workingFolderPath, oldValue, newValue, true, _ignoreReplaceContentFileEx.ToArray());
            Console.WriteLine();
            stopwatch.Stop();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"> Finish Replace Files Content - {stopwatch.Elapsed.Milliseconds} ms!");
            Console.ResetColor();

            // Finish
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[Done], thank for using [Puppy Cloner]!");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}