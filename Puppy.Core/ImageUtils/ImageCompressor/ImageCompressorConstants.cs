using System.ComponentModel;

namespace Puppy.Core.ImageUtils.ImageCompressor
{
    internal class ImageCompressorConstants
    {
        public const int DefaultPngQualityPercent = 90;

        public const int DefaultJpegQualityPercent = 75;

        public const int DefaultGifQualityPercent = 75;

        public const int TimeoutMillisecond = 600000;

        public const string GifWorkerFileName = "Puppy_ImageCompressor_GIF.exe";

        public const string JpegLibFileName = "libjpeg-62.dll";

        public const string JpegWorkerFileName = "Puppy_ImageCompressor_JPEG.exe";

        public const string JpegOptimizeWorkerFileName = "Puppy_ImageCompressor_JPEG_Optimize.exe";

        public const string PngPrimaryWorkerFileName = "Puppy_ImageCompressor_PNG_Primary.exe";

        public const string PngSecondaryWorkerFileName = "Puppy_ImageCompressor_PNG_Secondary.exe";

        public const string PngOptimizeWorkerFileName = "Puppy_ImageCompressor_PNG_Optimize.exe";
    }

    public enum CompressAlgorithm
    {
        [Description("Png lossless algorithm")]
        PngPrimary,

        [Description("Png 256 bit color algorithm")]
        PngSecondary,

        [Description("Jpeg optmize algorithm")]
        Jpeg,

        [Description("Gif lossy algorithm")]
        Gif
    }

    public enum CompressImageType
    {
        [Description("Invalid image format")]
        Invalid,

        [Description(".png")]
        Png,

        [Description(".jpeg")]
        Jpeg,

        [Description(".gif")]
        Gif
    }
}