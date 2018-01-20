using System;
using System.IO;
using System.Text;

namespace Puppy.Core.ImageUtils.ImageCompressor
{
    public class CompressResult : EventArgs
    {
        public MemoryStream ResultFileStream { get; set; } = new MemoryStream();

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:Puppy.Core.ImageUtils.ImageCompressor.CompressResult" /> class.
        /// </summary>
        /// <param name="filePath">               The result file name. </param>
        /// <param name="fileSizeBeforeCompress">
        ///     The original file fileSizeBeforeCompress in bytes.
        /// </param>
        public CompressResult(string filePath, long fileSizeBeforeCompress)
        {
            OriginalFileSize = fileSizeBeforeCompress;
            FileInfo result = new FileInfo(filePath);
            if (!result.Exists)
            {
                return;
            }
            CompressedFileSize = result.Length;
        }

        /// <summary>
        ///     Gets or sets the original file size in bytes. 
        /// </summary>
        public ImageType FileType { get; set; }

        public string FileExtension => FileType.GetEnumDescription();

        /// <summary>
        ///     Gets or sets the original file size in bytes. 
        /// </summary>
        public long OriginalFileSize { get; set; }

        /// <summary>
        ///     Gets or sets the result file size in bytes. 
        /// </summary>
        public long CompressedFileSize { get; set; }

        /// <summary>
        ///     Gets or sets the total time took. 
        /// </summary>
        public long TotalMillisecondsTook { get; set; }

        /// <summary>
        ///     Gets the difference in file size in bytes. 
        /// </summary>
        public long BytesSaving => OriginalFileSize - CompressedFileSize;

        /// <summary>
        ///     Gets the difference in file size as a percentage. 
        /// </summary>
        public double PercentSaving => BytesSaving == 0 ? 0 : Math.Round(((double)BytesSaving / OriginalFileSize) * 100, 2);

        /// <summary>
        ///     Returns a string that represents the current object. 
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Before: " + OriginalFileSize + " bytes");
            stringBuilder.AppendLine("After: " + CompressedFileSize + " bytes");
            stringBuilder.AppendLine($"BytesSaving: {BytesSaving} bytes / {PercentSaving:0.##}%");
            stringBuilder.AppendLine($"Total Milliseconds: {TotalMillisecondsTook} ms");
            return stringBuilder.ToString();
        }
    }
}