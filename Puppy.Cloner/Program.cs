using System;
using System.IO;

namespace Puppy.Cloner
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = Directory.GetCurrentDirectory();

            Console.WriteLine("Puppy Cloner");

            Console.WriteLine("Working in: " + Directory.GetCurrentDirectory());

            var oldValue = string.Empty;
            while (string.IsNullOrWhiteSpace(oldValue))
            {
                Console.Write("What is your project's old name: ");
                oldValue = Console.ReadLine();
            }

            var newValue = string.Empty;
            while (string.IsNullOrWhiteSpace(newValue))
            {
                Console.Write("What is your project's name: ");
                newValue = Console.ReadLine();
            }

            Console.WriteLine("Renaming directories...");

            CloneHelper.ReplaceFolderNames(directory, oldValue, newValue);

            Console.WriteLine("Directories renamed.");

            Console.WriteLine("Renaming files...");

            CloneHelper.ReplaceFolderNames(directory, oldValue, newValue);

            Console.WriteLine("Files renamed.");

            Console.WriteLine("Renaming file contents...");

            CloneHelper.ReplaceFileContents(directory, oldValue, newValue);

            Console.WriteLine("File contents renamed.");

            Console.WriteLine("Done!");
        }
    }
}