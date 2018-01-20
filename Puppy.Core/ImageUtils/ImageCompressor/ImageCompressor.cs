using EnumsNET;
using Puppy.Core.FileUtils;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Puppy.Core.ImageUtils.ImageCompressor
{
    public static class ImageCompressor
    {
        #region Properties

        /// <summary>
        ///     Setup Window UserName to Run CMD Progress 
        /// </summary>
        public static string ProcessRunAsUserName = null;

        /// <summary>
        ///     Setup Window User Password to Run CMD Progress 
        /// </summary>
        public static string ProcessRunAsPassword = null;

        /// <summary>
        ///     Process run as specific Window User or not 
        /// </summary>
        public static bool IsProcessRunAsUser => !string.IsNullOrWhiteSpace(ProcessRunAsUserName) && !string.IsNullOrWhiteSpace(ProcessRunAsPassword);

        #endregion Properties

        /// <summary>
        ///     Runs the process to optimize the image. 
        /// </summary>
        /// <param name="inputPath">     </param>
        /// <param name="outputPath">    </param>
        /// <param name="qualityPercent"></param>
        /// <param name="timeout">       </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> input path is invalid image format </exception>
        public static ImageCompressResult Compress(string inputPath, string outputPath, int qualityPercent = 0, int timeout = ImageCompressorConstants.TimeoutMillisecond)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (FileStream file = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
                {
                    // Copy file to stream

                    file.Position = 0;

                    file.CopyTo(stream);

                    // Do compress
                    ImageCompressResult imageCompressResult = Compress(stream, qualityPercent, timeout);

                    // Save to file
                    imageCompressResult?.ResultFileStream.Save(outputPath);

                    return imageCompressResult;
                }
            }
        }

        /// <summary>
        ///     Runs the process to optimize the image. 
        /// </summary>
        /// <param name="stream">         The source image stream. </param>
        /// <param name="qualityPercent">
        ///     Quality of image after compress, 0 is default it mean auto quality by image type
        /// </param>
        /// <param name="timeout">        TimeoutMillisecond of process in millisecond </param>
        /// <returns> The Task containing processing information. </returns>
        /// <exception cref="ArgumentException"> stream is invalid image format </exception>
        public static ImageCompressResult Compress(MemoryStream stream, int qualityPercent = 0, int timeout = ImageCompressorConstants.TimeoutMillisecond)
        {
            // Create new stopwatch.
            Stopwatch stopwatch = new Stopwatch();

            bool isValidImage = ImageCompressorHelper.TryGetCompressImageType(stream, out var imageType);

            if (!isValidImage || imageType == CompressImageType.Invalid)
            {
                throw new ArgumentException($"{nameof(stream)} is invalid image format", nameof(stream));
            }

            // Handle default value
            qualityPercent = ImageCompressorHelper.GetQualityPercent(qualityPercent, imageType);

            ImageCompressResult imageCompressResult = null;

            // Begin timing.
            stopwatch.Start();

            bool isCanOptimize = false;

            switch (imageType)
            {
                case CompressImageType.Png:
                    {
                        while (qualityPercent > 0)
                        {
                            imageCompressResult = Process(stream, CompressAlgorithm.PngPrimary, qualityPercent, timeout);

                            if (imageCompressResult == null || (imageCompressResult.PercentSaving > 0 && imageCompressResult.PercentSaving < 100))
                            {
                                isCanOptimize = true;

                                break;
                            }

                            qualityPercent -= 10;
                        }

                        // if quality percent < 0 then try compress by png secondary algorithm (this
                        // algorithm not related to quality percent)
                        if (!isCanOptimize)
                        {
                            imageCompressResult = Process(stream, CompressAlgorithm.PngSecondary, qualityPercent, timeout);
                        }

                        break;
                    }
                case CompressImageType.Jpeg:
                    {
                        while (qualityPercent > 0)
                        {
                            imageCompressResult = Process(stream, CompressAlgorithm.Jpeg, qualityPercent, timeout);

                            if (imageCompressResult == null || (imageCompressResult.PercentSaving > 0 && imageCompressResult.PercentSaving < 100))
                            {
                                isCanOptimize = true;

                                break;
                            }

                            qualityPercent -= 10;
                        }

                        break;
                    }
                case CompressImageType.Gif:
                    {
                        while (qualityPercent > 0)
                        {
                            imageCompressResult = Process(stream, CompressAlgorithm.Gif, qualityPercent, timeout);

                            if (imageCompressResult == null || (imageCompressResult.PercentSaving > 0 && imageCompressResult.PercentSaving < 100))
                            {
                                isCanOptimize = true;

                                break;
                            }

                            qualityPercent -= 10;
                        }

                        break;
                    }
            }

            // Stop timing.
            stopwatch.Stop();

            // if cannot at all, return null

            if (imageCompressResult == null)
            {
                return null;
            }

            if (imageCompressResult.PercentSaving > 0 && imageCompressResult.PercentSaving < 100)
            {
                // update total millisecond took only
                imageCompressResult.TotalMillisecondsTook = stopwatch.ElapsedMilliseconds;
            }
            else
            {
                // update total millisecond took
                imageCompressResult.TotalMillisecondsTook = stopwatch.ElapsedMilliseconds;

                // Cannot optimize Use origin for destination => update file size and stream
                imageCompressResult.CompressedFileSize = imageCompressResult.OriginalFileSize;

                // Copy origin steam to result
                imageCompressResult.ResultFileStream.SetLength(0);

                stream.Position = 0;

                stream.CopyTo(imageCompressResult.ResultFileStream);
            }

            imageCompressResult.QualityPercent = isCanOptimize ? qualityPercent : 100;

            return imageCompressResult;
        }

        #region Process

        /// <summary>
        ///     Runs the process to optimize the image. 
        /// </summary>
        /// <param name="stream">        </param>
        /// <param name="algorithm">     
        ///     Default is auto depend on file extension, others is force algorithm
        /// </param>
        /// <param name="qualityPercent">
        ///     Quality of image after compress, 0 is default it mean auto quality by image type
        /// </param>
        /// <param name="timeout">        TimeoutMillisecond of process in millisecond </param>
        /// <returns> The Task containing processing information. </returns>
        /// <exception cref="ArgumentException"> stream is invalid image format </exception>
        private static ImageCompressResult Process(MemoryStream stream, CompressAlgorithm algorithm, int qualityPercent = 0, int timeout = 0)
        {
            bool isValidImage = ImageCompressorHelper.TryGetCompressImageType(stream, out var imageType);

            if (!isValidImage || imageType == CompressImageType.Invalid)
            {
                throw new ArgumentException($"{nameof(stream)} is invalid image format", nameof(stream));
            }

            // Create a source temporary file with the correct extension.
            var filePath = FileHelper.CreateTempFile(stream, imageType.AsString(EnumFormat.Description), out _);

            ImageCompressResult imageCompressResult = Process(filePath, algorithm, qualityPercent, timeout);

            if (imageCompressResult != null)
            {
                // update file type, because in process not update it
                imageCompressResult.FileType = imageType;
            }

            // Cleanup temp file
            FileHelper.SafeDelete(filePath);

            return imageCompressResult;
        }

        /// <summary>
        ///     Runs the process to optimize the image. 
        /// </summary>
        /// <param name="filePath">       The source file. </param>
        /// <param name="algorithm">     
        ///     Default is auto depend on file extension, others is force algorithm
        /// </param>
        /// <param name="qualityPercent">
        ///     Quality of image after compress, 0 is default it mean auto quality by image type
        /// </param>
        /// <param name="timeout">        TimeoutMillisecond of process in millisecond </param>
        /// <returns> The Task containing processing information. </returns>
        /// <exception cref="ArgumentException">
        ///     file path is invalid, argument of command is invalid
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     Some security policies don't allow execution of programs in this way
        /// </exception>
        private static ImageCompressResult Process(string filePath, CompressAlgorithm algorithm, int qualityPercent = 0, int timeout = 0)
        {
            ImageCompressorHelper.CheckFilePath(filePath);

            long fileSizeBeforeCompress = new FileInfo(filePath).Length;

            ImageCompressResult imageCompressResult = null;

            var processInfo = new ProcessStartInfo("cmd")
            {
                WorkingDirectory = ImageCompressorBootstrapper.Instance.WorkingPath,
                Arguments = GetArguments(filePath, out var fileTempPath, algorithm, qualityPercent),
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
            };

            if (IsProcessRunAsUser)
            {
                System.Security.SecureString runAsPassword = new System.Security.SecureString();

                foreach (char c in ProcessRunAsPassword)
                {
                    runAsPassword.AppendChar(c);
                }

                processInfo.UserName = ProcessRunAsUserName;

                processInfo.Password = runAsPassword;
            }

            if (string.IsNullOrWhiteSpace(processInfo.Arguments))
            {
                throw new ArgumentException($"Command {nameof(processInfo.Arguments)} is empty", $"{nameof(processInfo.Arguments)}");
            }

            int elapsedTime = 0;

            bool eventHandled = false;

            try
            {
                Process process = new Process
                {
                    StartInfo = processInfo,
                    EnableRaisingEvents = true
                };

                process.Exited += (sender, args) =>
                {
                    // Done compress
                    imageCompressResult = new ImageCompressResult(filePath, fileSizeBeforeCompress);
                    process.Dispose();
                    eventHandled = true;

                    // Remove temp file if have
                    FileHelper.SafeDelete(fileTempPath);
                };

                process.Start();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                throw new NotSupportedException("Some security policies don't allow execution of programs in this way", ex);
            }

            // Wait for Exited event, but not more than config timeout time.
            const int sleepAmount = 100;

            while (!eventHandled)
            {
                elapsedTime += sleepAmount;

                if (elapsedTime > timeout && timeout > 0)
                {
                    break;
                }

                Thread.Sleep(sleepAmount);
            }

            // update compress result stream
            if (imageCompressResult != null)
            {
                FileHelper.WriteToStream(filePath, imageCompressResult.ResultFileStream);
            }
            return imageCompressResult;
        }

        #endregion Process

        #region Command

        /// <summary>
        ///     Gets the correct arguments to pass to the compressor 
        /// </summary>
        /// <param name="filePath">       The source file. </param>
        /// <param name="fileTempPath">  </param>
        /// <param name="algorithm">     
        ///     Default is auto depend on file extension, others is force algorithm
        /// </param>
        /// <param name="qualityPercent"> Quality PercentSaving - Process </param>
        /// <returns> The <see cref="string" /> containing the correct command arguments. </returns>
        /// <exception cref="ArgumentException"> file path is invalid </exception>
        private static string GetArguments(string filePath, out string fileTempPath, CompressAlgorithm algorithm, int qualityPercent = 0)
        {
            fileTempPath = null;

            ImageCompressorHelper.CheckFilePath(filePath);

            qualityPercent = ImageCompressorHelper.GetQualityPercent(qualityPercent, algorithm);

            switch (algorithm)
            {
                case CompressAlgorithm.Gif:
                    {
                        return GetGifCommand(filePath);
                    }
                case CompressAlgorithm.Jpeg:
                    {
                        return GetJpegCommand(filePath, out fileTempPath, qualityPercent);
                    }
                case CompressAlgorithm.PngPrimary:
                    {
                        return GetPngPrimaryCommand(filePath, qualityPercent);
                    }
            }

            return GetPngSecondaryCommand(filePath);
        }

        /// <summary>
        ///     GIF Command 
        /// </summary>
        /// <param name="filePath">      </param>
        /// <param name="qualityPercent"></param>
        /// <returns></returns>
        private static string GetGifCommand(string filePath, int qualityPercent = 0)
        {
            if (qualityPercent == 0)
            {
                qualityPercent = ImageCompressorConstants.DefaultGifQualityPercent;
            }

            int qualityLossyPercent = (100 - qualityPercent) * 2;

            // https://www.lcdf.org/gifsicle/man.html https://linux.die.net/man/1/gifsicle + lossy (https://github.com/pornel/giflossy/releases)
            // --use-col=web Adjust --lossy argument to suit quality (30 is very light compression,
            // 200 is heavy).
            var cmd = $"/c {ImageCompressorConstants.GifWorkerFileName} --no-warnings --no-app-extensions --no-comments --no-extensions --no-names -optimize=03 --lossy={qualityLossyPercent} \"{filePath}\" --output=\"{filePath}\"";

            return cmd;
        }

        // Jpeg
        private static string GetJpegCommand(string filePath, out string fileTempPath, int qualityPercent = 0)
        {
            if (qualityPercent == 0)
            {
                qualityPercent = ImageCompressorConstants.DefaultJpegQualityPercent;
            }

            // Idea: create temp file from source file then optimize temp file and copy to source
            // file (because cjpeg not support override input file) temporary file will be delete
            // after process exit

            MemoryStream streamTemp = new MemoryStream();

            FileHelper.WriteToStream(filePath, streamTemp);

            fileTempPath = FileHelper.CreateTempFile(streamTemp, CompressImageType.Jpeg.AsString(EnumFormat.Description), out _);

            // cjpeg after optimize => copy temp file to source file
            string jpegCommand = $"{ImageCompressorConstants.JpegWorkerFileName} -optimize -quality {qualityPercent} \"{fileTempPath}\" > \"{filePath}\"";

            // jpegoptim lossless not lossy
            string jpegOptimizeCommand = GetJpegOptimizeCommand(filePath);

            return $"/c {jpegCommand} & {jpegOptimizeCommand}";
        }

        private static string GetJpegOptimizeCommand(string filePath)
        {
            // jpegoptim lossless not lossy
            string jpegOptimizeCommand = $"{ImageCompressorConstants.JpegOptimizeWorkerFileName} -o -q -p --force --strip-all --strip-iptc --strip-icc --all-progressive \"{filePath}\"";
            return jpegOptimizeCommand;
        }

        // Png
        private static string GetPngPrimaryCommand(string filePath, int qualityPercent = 0)
        {
            if (qualityPercent == 0)
            {
                qualityPercent = ImageCompressorConstants.DefaultPngQualityPercent;
            }

            // First, use pnguqnat to compress
            int maxQuality = qualityPercent + 15;

            // max is 99
            maxQuality = maxQuality > 99 ? 99 : maxQuality;
            string pngPrimaryCommand = $"{ImageCompressorConstants.PngPrimaryWorkerFileName} --speed 1 --quality={qualityPercent}-{maxQuality} --skip-if-larger --strip --output \"{filePath}\" --force \"{filePath}\"";

            // Then use pngout to optimize Recompress by pngout to make maximum recompress
            var pngOptimizeCommand = GetPngOptimizeCommand(filePath);
            return $"/c {pngPrimaryCommand} & {pngOptimizeCommand}";
        }

        private static string GetPngSecondaryCommand(string filePath)
        {
            // First, use pngqn to compress view more detail http://pngnq.sourceforge.net/
            string pngSecondaryCommand = $"{ImageCompressorConstants.PngSecondaryWorkerFileName} -f -s1 -e.png -n 256 \"{filePath}\"";

            // Then use pngout to optimize Recompress by pngout to make maximum recompress
            var pngOptimizeCommand = GetPngOptimizeCommand(filePath);
            return $"/c {pngSecondaryCommand} & {pngOptimizeCommand}";
        }

        private static string GetPngOptimizeCommand(string filePath)
        {
            // view more detail http://www.advsys.net/ken/util/pngout.htm use s2 f5 to make fastest
            // with quality (s0 and f0 take minutes)
            string pngOptimize = $"{ImageCompressorConstants.PngOptimizeWorkerFileName} \"{filePath}\" \"{filePath}\" /s2 /f5 /y /q";
            return pngOptimize;
        }

        #endregion Command
    }
}