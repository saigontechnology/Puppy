using Microsoft.VisualStudio.TestTools.UnitTesting;
using Puppy.Core.ImageUtils.ImageCompressor;

namespace Puppy.Testing
{
    [TestClass]
    public class ImageCompressorUnitTest
    {
        [TestMethod]
        public void CompressorUnitTest()
        {
            var imageCompressJpegResult = ImageCompressor.Compress("<file name>.jpg", "<outlet file name>.jpg");

            var imageCompressGifResult = ImageCompressor.Compress("<file name>.gif", "<outlet file name>.gif");

            var imageCompressPngResult = ImageCompressor.Compress("<file name>.png", "<outlet file name>.png");
        }
    }
}