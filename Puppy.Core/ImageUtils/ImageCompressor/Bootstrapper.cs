using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Puppy.Core.ImageUtils.ImageCompressor
{
    /// <summary>
    ///     The Bootstrapper. 
    /// </summary>
    internal sealed class Bootstrapper
    {
        /// <summary>
        ///     The assembly version. 
        /// </summary>
        private static readonly string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        ///     A new instance of the <see cref="T:.Web.Config.Config" /> class. with lazy initialization. 
        /// </summary>
        private static readonly Lazy<Bootstrapper> Lazy = new Lazy<Bootstrapper>(() => new Bootstrapper());

        /// <summary>
        ///     Prevents a default instance of the <see cref="Bootstrapper" /> class from being created. 
        /// </summary>
        private Bootstrapper()
        {
            if (!Lazy.IsValueCreated)
            {
                RegisterExecutable();
            }
        }

        /// <summary>
        ///     Gets the current instance of the <see cref="Bootstrapper" /> class. 
        /// </summary>
        public static Bootstrapper Instance => Lazy.Value;

        /// <summary>
        ///     Gets the working directory path. 
        /// </summary>
        public string WorkingPath { get; private set; }

        /// <summary>
        ///     Registers the embedded executable. 
        /// </summary>
        public void RegisterExecutable()
        {
            // None of the tools used here are called using dll import so we don't go through the
            // normal registration channel.
            string folder = Environment.Is64BitProcess ? "x64" : "x86";
            Assembly assembly = Assembly.GetExecutingAssembly();

            WorkingPath = Path.GetFullPath(
                Path.Combine(new Uri(assembly.Location).LocalPath, "..\\." + AssemblyVersion + "\\"));

            // Create the folder for storing temporary images.
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(WorkingPath));

            if (directoryInfo.Exists)
            {
                // If already have folder => do nothingS
                return;
            }

            directoryInfo.Create();

            // Get the resources and copy them across. [Important] Make sure all file .exe is have
            // set properties with Embedded Resource if not =>
            // Assembly.GetExecutingAssembly().GetManifestResourceStream(resource.Value) will null
            Dictionary<string, string> resources = new Dictionary<string, string>
            {
                // -------------------------- GIF --------------------------

                // Puppy_ImageCompressor_GIF = gifsicle
                { "Puppy_ImageCompressor_GIF.exe", "Puppy.Core.ImageUtils.ImageCompressor.Libraries." + folder + ".Puppy_ImageCompressor_GIF.exe" },

                // -------------------------- JPG/JPEG --------------------------S

                // Puppy_ImageCompressor_JPEG = cjpeg
                { "Puppy_ImageCompressor_JPEG.exe", "Puppy.Core.ImageUtils.ImageCompressor.Libraries.x86.Puppy_ImageCompressor_JPEG.exe" },

                // libjpeg-62.dll = cjpeg lib
                { "libjpeg-62.dll", "Puppy.Core.ImageUtils.ImageCompressor.Libraries.x86.libjpeg-62.dll" },

                // Puppy_ImageCompressor_JPEG_Optimize = jpegoptim
                { "Puppy_ImageCompressor_JPEG_Optimize.exe", "Puppy.Core.ImageUtils.ImageCompressor.Libraries.x86.Puppy_ImageCompressor_JPEG_Optimize.exe" },

                // -------------------------- PNG --------------------------

                // Puppy_ImageCompressor_PNG_Primary = pngquant
                { "Puppy_ImageCompressor_PNG_Primary.exe", "Puppy.Core.ImageUtils.ImageCompressor.Libraries.x86.Puppy_ImageCompressor_PNG_Primary.exe" },

                // Puppy_ImageCompressor_PNG_Secondary = pngqn
                { "Puppy_ImageCompressor_PNG_Secondary.exe", "Puppy.Core.ImageUtils.ImageCompressor.Libraries.x86.Puppy_ImageCompressor_PNG_Secondary.exe" },

                // Puppy_ImageCompressor_PNG_Optimize = pngout
                { "Puppy_ImageCompressor_PNG_Optimize.exe", "Puppy.Core.ImageUtils.ImageCompressor.Libraries.x86.Puppy_ImageCompressor_PNG_Optimize.exe" }
            };

            // Write the files out to the bin folder.
            foreach (KeyValuePair<string, string> resource in resources)
            {
                using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource.Value))
                {
                    if (resourceStream == null)
                    {
                        continue;
                    }

                    using (FileStream fileStream = File.OpenWrite(Path.Combine(WorkingPath, resource.Key)))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
            }
        }
    }
}