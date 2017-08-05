using System;
using System.IO;

namespace Puppy.Cloner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Puppy Cloner - Fastest way to clone project!";
            Console.WriteLine("Welcome to Puppy Cloner!");

            // Working Folder
            Console.WriteLine();
            Console.WriteLine("[Tip] Set empty for [Puppy Cloner] working in current folder");
            Console.Write("What is your new project's folder: ");
            var workingFolderPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(workingFolderPath))
            {
                workingFolderPath = Directory.GetCurrentDirectory();
            }

            Console.WriteLine("[Puppy Cloner] working in: " + workingFolderPath);
            Console.WriteLine();

            Console.WriteLine("[Tip] [Puppy Cloner] Auto make extra search for Upper Case and Lower Case to replace old name to new name!");
            Console.WriteLine();

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

            // Replace Directories
            CloneHelper.ReplaceFolderNames(workingFolderPath, oldValue, newValue);

            // Replace Files Name
            CloneHelper.ReplaceFileNames(workingFolderPath, oldValue, newValue);

            // Replace Content Files
            CloneHelper.ReplaceFileContents(workingFolderPath, oldValue, newValue);


            // Finish
            Console.WriteLine();
            Console.WriteLine("Done!");
        }
    }
}