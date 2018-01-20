using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Puppy.Core.ImageUtils.ImageCompressor
{
    /// <summary>
    ///     The ImageCompressor Bootstrapper. 
    /// </summary>
    internal sealed class ImageCompressorBootstrapper
    {
        /// <summary>
        ///     The assembly version. 
        /// </summary>
        private static readonly string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        ///     A new instance of the <see cref="T:.Web.Config.Config" /> class. with lazy initialization. 
        /// </summary>
        private static readonly Lazy<ImageCompressorBootstrapper> Lazy = new Lazy<ImageCompressorBootstrapper>(() => new ImageCompressorBootstrapper());

        /// <summary>
        ///     Prevents a default instance of the <see cref="ImageCompressorBootstrapper" /> class
        ///     from being created.
        /// </summary>
        private ImageCompressorBootstrapper()
        {
            if (!Lazy.IsValueCreated)
            {
                RegisterExecutable();
            }
        }

        /// <summary>
        ///     Gets the current instance of the <see cref="ImageCompressorBootstrapper" /> class. 
        /// </summary>
        public static ImageCompressorBootstrapper Instance => Lazy.Value;

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

            Assembly assembly = Assembly.GetExecutingAssembly();

            // Create the folder for storing temporary images and tools.

            WorkingPath = Path.GetFullPath(Path.Combine(new Uri(assembly.Location).LocalPath, "..\\Puppy_ImageCompressorTools." + AssemblyVersion + "\\"));

            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(WorkingPath));

            if (directoryInfo.Exists)
            {
                // Already have folder mean already copied tools before => return
                return;
            }

            directoryInfo.Create();

            // Load tools
            var librariesNameSpace = $"{nameof(Puppy)}.{nameof(Core)}.{nameof(ImageUtils)}.{nameof(ImageUtils.ImageCompressor)}.ImageCompressorTools";

            // Get the resources and copy them across.
            Dictionary<string, string> resources = new Dictionary<string, string>
            {
                { ImageCompressorConstants.GifWorkerFileName, $"{librariesNameSpace}.{ImageCompressorConstants.GifWorkerFileName}" },
                { ImageCompressorConstants.JpegLibFileName, $"{librariesNameSpace}.{ImageCompressorConstants.JpegLibFileName}" },
                { ImageCompressorConstants.JpegWorkerFileName, $"{librariesNameSpace}.{ImageCompressorConstants.JpegWorkerFileName}" },
                { ImageCompressorConstants.JpegOptimizeWorkerFileName, $"{librariesNameSpace}.{ImageCompressorConstants.JpegOptimizeWorkerFileName}" },
                { ImageCompressorConstants.PngPrimaryWorkerFileName, $"{librariesNameSpace}.{ImageCompressorConstants.PngPrimaryWorkerFileName}" },
                { ImageCompressorConstants.PngSecondaryWorkerFileName, $"{librariesNameSpace}.{ImageCompressorConstants.PngSecondaryWorkerFileName}" },
                { ImageCompressorConstants.PngOptimizeWorkerFileName, $"{librariesNameSpace}.{ImageCompressorConstants.PngOptimizeWorkerFileName}" }
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

                    var toolFullPath = Path.Combine(WorkingPath, resource.Key);

                    using (FileStream fileStream = File.OpenWrite(toolFullPath))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
            }
        }
    }
}