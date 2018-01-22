using Microsoft.VisualStudio.TestTools.UnitTesting;
using Puppy.Coordinate.Clustering;
using Puppy.Coordinate.TripUtils;
using System.Collections.Generic;

namespace Puppy.Testing
{
    [TestClass]
    public class CoordinateUnitTest
    {
        [TestMethod]
        public void ClusterTest()
        {
            // Use Gebweb to test https://gebweb.net/optimap/

            //(10.8002712, 106.66691070000002)
            //(10.8001182, 106.66909299999998)
            //(10.7993045, 106.6677267)
            //(10.798618, 106.66926460000002)
            //(10.7978616, 106.6654264)
            //(10.7991835, 106.66459699999996)
            //(10.7985912, 106.66446250000001)
            //(10.7982545, 106.6644063)
            //(10.7972986, 106.66749460000005)
            //(10.7990973, 106.66693399999997)

            // Own Data

            //(106.66691070000002,10.8002712)
            //(106.66909299999998,10.8001182)
            //(106.6677267       ,10.7993045)
            //(106.66926460000002,10.798618)
            //(106.6654264       ,10.7978616)
            //(106.66459699999996,10.7991835)
            //(106.66446250000001,10.7985912)
            //(106.6644063       ,10.7982545)
            //(106.66749460000005,10.7972986)
            //(106.66693399999997,10.7990973)

            var coordinates = new List<Coordinate.Models.Coordinate>
            {
                new Coordinate.Models.Coordinate(106.66691070000002, 10.8002712),
                new Coordinate.Models.Coordinate(106.66909299999998, 10.8001182),
                new Coordinate.Models.Coordinate(106.6677267, 10.7993045),
                new Coordinate.Models.Coordinate(106.66926460000002, 10.798618),
                new Coordinate.Models.Coordinate(106.6654264, 10.7978616),
                new Coordinate.Models.Coordinate(106.66459699999996, 10.7991835),
                new Coordinate.Models.Coordinate(106.66446250000001, 10.7985912),
                new Coordinate.Models.Coordinate(106.6644063, 10.7982545),
                new Coordinate.Models.Coordinate(106.66749460000005, 10.7972986),
                new Coordinate.Models.Coordinate(106.66693399999997, 10.7990973)
            };

            GeoClustering gc = new GeoClustering();

            var cluster = gc.Cluster(coordinates, 3);
        }

        [TestMethod]
        public void FastestTripTest()
        {
            // Use Gebweb to test https://gebweb.net/optimap/

            // 31 Trương phước phan, 10.7756192, 106.6237788

            // 435 hoàng văn thụ hồ chí minh, 10.7975076, 106.6558153

            // 60 cộng hoà, 10.8011653, 106.657283

            // 10 Tân kỳ tân quý hồ chí minh, 10.7922362, 106.605514

            // 30 âu cơ hồ chí minh, 10.7877314, 106.64095250000003

            // (10.7756192, 106.6237788)

            // (10.7975076, 106.6558153)

            // (10.8011653, 106.657283)

            // (10.7922362, 106.605514)

            // (10.7877314, 106.64095250000003)

            // Own Data

            // (106.6237788,10.7756192)

            // (106.6558153,10.7975076)

            // (106.657283,10.8011653)

            // (106.605514,10.7922362)

            // (106.64095250000003,10.7877314)

            var coordinates = new List<Coordinate.Models.Coordinate>
            {
                new Coordinate.Models.Coordinate(106.6237788,10.7756192),
                new Coordinate.Models.Coordinate(106.6558153,10.7975076),
                new Coordinate.Models.Coordinate(106.657283,10.8011653) ,
                new Coordinate.Models.Coordinate(106.605514,10.7922362),
                new Coordinate.Models.Coordinate(106.64095250000003,10.7877314)
            };

            // A to Z TRIP

            FastestAzTrip fastestAzTrip = new FastestAzTrip(coordinates);

            List<Coordinate.Models.Coordinate> azTripCoordinates = fastestAzTrip.GetTrip();

            // ROUND TRIP

            FastestRoundTrip fastestRoundTrip = new FastestRoundTrip(coordinates);

            List<Coordinate.Models.Coordinate> roundTripCoordinates = fastestRoundTrip.GetTrip();
        }

        [TestMethod]
        public static void FastestOptimizeTest()
        {
            // Use Gebweb to test https://gebweb.net/optimap/

            // 31 Trương phước phan, 10.7756192, 106.6237788

            // 435 hoàng văn thụ hồ chí minh, 10.7975076, 106.6558153

            // 60 cộng hoà, 10.8011653, 106.657283

            // 10 Tân kỳ tân quý hồ chí minh, 10.7922362, 106.605514

            // 30 âu cơ hồ chí minh, 10.7877314, 106.64095250000003

            // (10.7756192, 106.6237788)

            // (10.7975076, 106.6558153)

            // (10.8011653, 106.657283)

            // (10.7922362, 106.605514)

            // (10.7877314, 106.64095250000003)

            // Own Data

            // (106.6237788,10.7756192)

            // (106.6558153,10.7975076)

            // (106.657283,10.8011653)

            // (106.605514,10.7922362)

            // (106.64095250000003,10.7877314)

            var coordinates = new List<Coordinate.Models.Coordinate>
            {
                new Coordinate.Models.Coordinate(106.6237788,10.7756192),
                new Coordinate.Models.Coordinate(106.6558153,10.7975076),
                new Coordinate.Models.Coordinate(106.657283,10.8011653) ,
                new Coordinate.Models.Coordinate(106.605514,10.7922362),
                new Coordinate.Models.Coordinate(106.64095250000003,10.7877314)
            };

            // ROUND TRIP

            FastestTrip fastestRoundTrip = new FastestTrip(coordinates, FastestTrip.FastestTripMode.RoundTrip);

            List<Coordinate.Models.Coordinate> roundTrip = fastestRoundTrip.GetTrip();

            double totalRoundTripDistance = fastestRoundTrip.GetTotalDistance();

            double totalRoundTripDuration = fastestRoundTrip.GetTotalDuration();

            // A -> Z TRIP

            FastestTrip fastestAzTrip = new FastestTrip(coordinates, FastestTrip.FastestTripMode.AtoZ);

            List<Coordinate.Models.Coordinate> azTrip = fastestAzTrip.GetTrip();

            double totalAzTripDistance = fastestAzTrip.GetTotalDistance();

            double totalAzTripDuration = fastestAzTrip.GetTotalDuration();
        }
    }
}