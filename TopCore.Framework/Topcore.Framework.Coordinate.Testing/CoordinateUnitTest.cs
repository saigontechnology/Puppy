using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Topcore.Framework.Coordinate.Testing
{
    [TestClass]
    public class CoordinateUnitTest
    {
        [TestMethod]
        public void TestDistance()
        {
            var home = new Coordinate(106.630116, 10.768984);
            var damSenPark = new Coordinate(106.637482, 10.768420);

            var distanceInMeter = home.DistanceTo(damSenPark, CoordinateDistanceExtension.UnitOfLength.Meter);

            var farHome1KmTopLeft = home.GetTopLeftOfSquare(1);
            var distanceTopLeftToHome = home.DistanceTo(farHome1KmTopLeft,
                CoordinateDistanceExtension.UnitOfLength.Meter);

            var farHome1KmBotRight = home.GetBotRightOfSquare(1);
            var distanceBotRightToHome = home.DistanceTo(farHome1KmBotRight,
                CoordinateDistanceExtension.UnitOfLength.Meter);
        }
    }
}