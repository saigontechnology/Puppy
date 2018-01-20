using System.ComponentModel;

namespace Puppy.Core.ImageUtils.ImageCompressor
{
    public enum CompressAlgorithm
    {
        [Description("Png lossless algorithm")]
        PngPrimary,

        [Description("Png 256 bit color algorithm")]
        PngSecondary,

        [Description("Jpeg optmize algorithm")]
        Jpeg,

        [Description("Gif lossy algorithm")]
        Gif,

        [Description("Svg optimize algorithm")]
        Svg
    }

    public enum ImageType
    {
        [Description("Invalid image format")]
        Invalid,

        [Description(".png")]
        Png,

        [Description(".jpeg")]
        Jpeg,

        [Description(".gif")]
        Gif,

        [Description(".svg")]
        Svg,
    }
}