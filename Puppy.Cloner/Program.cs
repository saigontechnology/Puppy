using System;
using System.Diagnostics;
using System.IO;

namespace Puppy.Cloner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Puppy Cloner - Fastest way to clone project!";

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Welcome to Puppy Cloner!");

            // Working Folder
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[Tip] Set empty for [Puppy Cloner] working in current folder");

            Console.ResetColor();
            Console.Write("What is your new project's folder: ");
            var workingFolderPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(workingFolderPath))
            {
                workingFolderPath = Directory.GetCurrentDirectory();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("> Puppy Cloner working in: " + workingFolderPath);
            Console.WriteLine();
            Console.ResetColor();

            // Old Name
            var oldValue = string.Empty;
            while (string.IsNullOrWhiteSpace(oldValue))
            {
                Console.Write("What is your old project name: ");
                oldValue = Console.ReadLine();
            }

            // New Name
            Console.WriteLine();
            var newValue = string.Empty;
            while (string.IsNullOrWhiteSpace(newValue))
            {
                Console.Write("What is your new project name: ");
                newValue = Console.ReadLine();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Start progress replace from '{oldValue}' to '{newValue}'");
            Console.ResetColor();

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Replace Directories
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Replace Folders Name");

            stopwatch.Restart();
            Console.WriteLine();
            CloneHelper.ReplaceFolderNames(workingFolderPath, oldValue, newValue, true);
            Console.WriteLine();
            stopwatch.Stop();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Finish Replace Folders Name - {stopwatch.Elapsed.Milliseconds} ms!");
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
            Console.WriteLine($"Finish Replace Files Name - {stopwatch.Elapsed.Milliseconds} ms!");
            Console.ResetColor();

            // Replace Content Files
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Replace Files Content");

            stopwatch.Restart();
            Console.WriteLine();
            CloneHelper.ReplaceFileContents(workingFolderPath, oldValue, newValue, true);
            Console.WriteLine();
            stopwatch.Stop();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Finish Replace Files Content - {stopwatch.Elapsed.Milliseconds} ms!");
            Console.ResetColor();

            // Finish
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Done, thank for using [Puppy Cloner]!");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}