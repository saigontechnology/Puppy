namespace Puppy.Core.ImageUtils.ImageCompressor
{
    public static class Constants
    {
        public static readonly int DefaultPngQualityPercent = 90;

        public static readonly int DefaultJpegQualityPercent = 75;

        public static readonly int DefaultGifQualityPercent = 75;

        public static readonly int TimeoutMillisecond = 600000;

        public static readonly string ProcessRunAsUserName = "";

        public static readonly string ProcessRunAsPassword = "";

        public static bool IsProcessRunAsUser => !string.IsNullOrWhiteSpace(ProcessRunAsUserName) && !string.IsNullOrWhiteSpace(ProcessRunAsPassword);
    }
}