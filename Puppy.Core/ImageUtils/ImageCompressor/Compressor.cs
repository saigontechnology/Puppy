using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Puppy.Core.ImageUtils.ImageCompressor
{
    public static class Compressor
    {
        #region Compress

        /// <summary>
        ///     Runs the process to optimize the image. 
        /// </summary>
        /// <param name="inputPath">     </param>
        /// <param name="outputPath">    </param>
        /// <param name="qualityPercent"></param>
        /// <param name="timeout">       </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> input path is invalid image format </exception>
        public static CompressResult Compress(string inputPath, string outputPath, int qualityPercent = 0, int timeout = 0)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (FileStream file = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
                {
                    // Copy file to stream
                    file.Position = 0;
                    file.CopyTo(stream);

                    // Do compress
                    CompressResult compressResult = Compress(stream, qualityPercent, timeout);

                    // Save to file
                    if (compressResult != null)
                    {
                        Helper.WriteToFile(outputPath, compressResult.ResultFileStream);
                    }
                    return compressResult;
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
        public static CompressResult Compress(MemoryStream stream, int qualityPercent = 0, int timeout = 0)
        {
            // Create new stopwatch.
            Stopwatch stopwatch = new Stopwatch();

            bool isValidImage = Helper.TryGetImageType(stream, out var imageType);

            if (!isValidImage || imageType == ImageType.Invalid)
            {
                throw new ArgumentException($"{nameof(stream)} is invalid image format", nameof(stream));
            }

            // Handle default value
            qualityPercent = Helper.GetQualityPercent(qualityPercent, imageType);
            timeout = Helper.GetTimeOut(timeout);

            CompressResult compressResult = null;

            // Begin timing.
            stopwatch.Start();
            bool isCanOptimize = false;
            if (imageType == ImageType.Png)
            {
                while (qualityPercent > 0 && !isCanOptimize)
                {
                    compressResult = Process(stream, CompressAlgorithm.PngPrimary, qualityPercent, timeout);
                    qualityPercent -= 10;

                    if (compressResult == null || (compressResult.PercentSaving > 0 && compressResult.PercentSaving < 100))
                    {
                        isCanOptimize = true;
                    }
                }

                // if quality percent < 0 then try compress by png secondary algorithm (this
                // algorithm not related to quality percent)
                if (!isCanOptimize)
                {
                    compressResult = Process(stream, CompressAlgorithm.PngSecondary, qualityPercent, timeout);
                }
            }
            else if (imageType == ImageType.Jpeg)
            {
                while (qualityPercent > 0 && !isCanOptimize)
                {
                    compressResult = Process(stream, CompressAlgorithm.Jpeg, qualityPercent, timeout);
                    qualityPercent -= 10;

                    if (compressResult == null || (compressResult.PercentSaving > 0 && compressResult.PercentSaving < 100))
                    {
                        isCanOptimize = true;
                    }
                }
            }
            else if (imageType == ImageType.Gif)
            {
                while (qualityPercent > 0 && !isCanOptimize)
                {
                    compressResult = Process(stream, CompressAlgorithm.Gif, qualityPercent, timeout);
                    qualityPercent -= 10;

                    if (compressResult == null || (compressResult.PercentSaving > 0 && compressResult.PercentSaving < 100))
                    {
                        isCanOptimize = true;
                    }
                }
            }
            else
            {
                compressResult = Process(stream, CompressAlgorithm.Svg, qualityPercent, timeout);
            }

            // Stop timing.
            stopwatch.Stop();

            // if cannot at all, return null
            if (compressResult == null)
            {
                return null;
            }

            if ((compressResult.PercentSaving > 0 && compressResult.PercentSaving < 100))
            {
                // update total millisecond took only
                compressResult.TotalMillisecondsTook = stopwatch.ElapsedMilliseconds;
            }
            else
            {
                // update total millisecond took
                compressResult.TotalMillisecondsTook = stopwatch.ElapsedMilliseconds;

                // Cannot optimize Use origin for destination => update file size and stream
                compressResult.CompressedFileSize = compressResult.OriginalFileSize;

                // Copy origin steam to result
                compressResult.ResultFileStream.SetLength(0);
                stream.Position = 0;
                stream.CopyTo(compressResult.ResultFileStream);
            }
            return compressResult;
        }

        #endregion Compress

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
        private static CompressResult Process(MemoryStream stream, CompressAlgorithm algorithm, int qualityPercent = 0, int timeout = 0)
        {
            bool isValidImage = Helper.TryGetImageType(stream, out var imageType);

            if (!isValidImage || imageType == ImageType.Invalid)
            {
                throw new ArgumentException($"{nameof(stream)} is invalid image format", nameof(stream));
            }

            // Create a source temporary file with the correct extension.
            var filePath = Helper.CreateTemporaryFile(stream, imageType.GetEnumDescription(), out _);

            CompressResult compressResult = Process(filePath, algorithm, qualityPercent, timeout);

            if (compressResult != null)
            {
                // update file type, because in process not update it
                compressResult.FileType = imageType;
            }

            // Cleanup temp file
            Helper.DeleteFile(filePath);

            return compressResult;
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
        private static CompressResult Process(string filePath, CompressAlgorithm algorithm, int qualityPercent = 0, int timeout = 0)
        {
            Helper.CheckFilePath(filePath);
            long fileSizeBeforeCompress = new FileInfo(filePath).Length;

            CompressResult compressResult = null;

            var processInfo = new ProcessStartInfo("cmd")
            {
                WorkingDirectory = Bootstrapper.Instance.WorkingPath,
                Arguments = GetArguments(filePath, out var fileTempPath, algorithm, qualityPercent),
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
            };

#if !DEBUG
            // Use user have full permission of tools to run process of cmd Currently, Only case svg
            // in nodejs need user name and password of window login to execute nodejs svgo package
            if (Constants.IsProcessRunAsUser)
            {
                System.Security.SecureString runAsPassword = new System.Security.SecureString();
                foreach (char c in Constants.ProcessRunAsPassword)
                {
                    runAsPassword.AppendChar(c);
                }

                processInfo.UserName = Constants.ProcessRunAsUserName;
                processInfo.Password = runAsPassword;
            }
#endif

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
                    compressResult = new CompressResult(filePath, fileSizeBeforeCompress);
                    process.Dispose();
                    eventHandled = true;

                    // Remove temp file if have
                    Helper.DeleteFile(fileTempPath);
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
            if (compressResult != null)
            {
                Helper.WriteToStream(filePath, compressResult.ResultFileStream);
            }
            return compressResult;
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
        private static string GetArguments(
                                            string filePath,
                                            out string fileTempPath,
                                            CompressAlgorithm algorithm,
                                            int qualityPercent = 0)
        {
            fileTempPath = null;
            Helper.CheckFilePath(filePath);
            qualityPercent = Helper.GetQualityPercent(qualityPercent, algorithm);
            if (algorithm == CompressAlgorithm.Svg)
            {
                return GetSvgCommand(filePath);
            }
            if (algorithm == CompressAlgorithm.Gif)
            {
                return GetGifCommand(filePath);
            }
            if (algorithm == CompressAlgorithm.Jpeg)
            {
                return GetJpegCommand(filePath, out fileTempPath, qualityPercent);
            }
            if (algorithm == CompressAlgorithm.PngPrimary)
            {
                return GetPngPrimaryCommand(filePath, qualityPercent);
            }
            return GetPngSecondaryCommand(filePath);
        }

        /// <summary>
        ///     SVG 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string GetSvgCommand(string filePath)
        {
            // need install nodejs and svgo by nodejs to do this https://github.com/svg/svgo
            return $"/c svgo \"{filePath}\" --quiet";
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
                qualityPercent = Constants.DefaultGifQualityPercent;
            }

            // giflossy have lossy % from 30-200 => qualityLossyPersent = (100 - qualityPercent) * 2

            int qualityLossyPersent = (100 - qualityPercent) * 2;

            // https://www.lcdf.org/gifsicle/man.html https://linux.die.net/man/1/gifsicle + lossy (https://github.com/pornel/giflossy/releases)
            // --use-col=web Adjust --lossy argument to suit quality (30 is very light compression,
            // 200 is heavy).
            var cmd = $"/c TN_ImageCompressor_GIF --no-warnings --no-app-extensions --no-comments --no-extensions --no-names -optimize=03 --lossy={qualityLossyPersent} \"{filePath}\" --output=\"{filePath}\"";

            return cmd;
        }

        // Jpeg
        private static string GetJpegCommand(string filePath, out string fileTempPath, int qualityPercent = 0)
        {
            if (qualityPercent == 0)
            {
                qualityPercent = Constants.DefaultJpegQualityPercent;
            }

            // Idea: create temp file from source file then optimize temp file and copy to source
            // file (because cjpeg not support override input file) temporary file will be delete
            // after process exit

            MemoryStream streamTemp = new MemoryStream();

            Helper.WriteToStream(filePath, streamTemp);

            fileTempPath = Helper.CreateTemporaryFile(streamTemp, ImageType.Jpeg.GetEnumDescription(), out _);

            // cjpeg after optimize => copy temp file to source file
            string jpegCommand = $"TN_ImageCompressor_JPEG -optimize -quality {qualityPercent} \"{fileTempPath}\" > \"{filePath}\"";

            // jpegoptim lossless not lossy
            string jpegOptimizeCommand = GetJpegOptimizeCommand(filePath);

            return $"/c {jpegCommand} & {jpegOptimizeCommand}";
        }

        private static string GetJpegOptimizeCommand(string filePath)
        {
            // jpegoptim lossless not lossy
            string jpegOptimizeCommand = $"TN_ImageCompressor_JPEG_Optimize -o -q -p --force --strip-all --strip-iptc --strip-icc --all-progressive \"{filePath}\"";
            return jpegOptimizeCommand;
        }

        // Png
        private static string GetPngPrimaryCommand(string filePath, int qualityPercent = 0)
        {
            if (qualityPercent == 0)
            {
                qualityPercent = Constants.DefaultPngQualityPercent;
            }

            // First, use pnguqnat to compress
            int maxQuality = qualityPercent + 15;

            // max is 99
            maxQuality = maxQuality > 99 ? 99 : maxQuality;
            string pngPrimaryCommand = $"TN_ImageCompressor_PNG_Primary --speed 1 --quality={qualityPercent}-{maxQuality} --skip-if-larger --strip --output \"{filePath}\" --force \"{filePath}\"";

            // Then use pngout to optimize Recompress by pngout to make maximum recompress
            var pngOptimizeCommand = GetPngOptimizeCommand(filePath);
            return $"/c {pngPrimaryCommand} & {pngOptimizeCommand}";
        }

        private static string GetPngSecondaryCommand(string filePath)
        {
            // First, use pngqn to compress view more detail http://pngnq.sourceforge.net/
            string pngSecondaryCommand = $"TN_ImageCompressor_PNG_Secondary -f -s1 -e.png -n 256 \"{filePath}\"";

            // Then use pngout to optimize Recompress by pngout to make maximum recompress
            var pngOptimizeCommand = GetPngOptimizeCommand(filePath);
            return $"/c {pngSecondaryCommand} & {pngOptimizeCommand}";
        }

        private static string GetPngOptimizeCommand(string filePath)
        {
            // view more detail http://www.advsys.net/ken/util/pngout.htm use s2 f5 to make fastest
            // with quality (s0 and f0 take minutes)
            string pngOptimize = $"TN_ImageCompressor_PNG_Optimize \"{filePath}\" \"{filePath}\" /s2 /f5 /y /q";
            return pngOptimize;
        }

        #endregion Command
    }
}