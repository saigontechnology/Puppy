using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Topcore.Framework.Coordinate.Testing
{
    [TestClass]
    public class CoordinateUnitTest
    {
        [TestMethod]
        public void TestDistance()
        {
            Coordinate home = new Coordinate(10.768984, 106.630116);
            Coordinate damSenPark = new Coordinate(10.768420, 106.637482);

            double distanceInMeter = home.DistanceTo(damSenPark, CoordinateDistanceExtension.UnitOfLength.Meter);

            Coordinate farHome1KmTopLeft = home.GetTopLeftOfSquare(1);
            double distanceTopLeftToHome = home.DistanceTo(farHome1KmTopLeft, CoordinateDistanceExtension.UnitOfLength.Meter);

            Coordinate farHome1KmBotRight = home.GetBotRightOfSquare(1);
            double distanceBotRightToHome = home.DistanceTo(farHome1KmBotRight, CoordinateDistanceExtension.UnitOfLength.Meter);

        }
    }
}
