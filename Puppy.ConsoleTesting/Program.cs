using Puppy.Coordinate.Clustering;
using Puppy.Coordinate.TripUtils;
using System;
using System.Collections.Generic;

namespace Puppy.ConsoleTesting
{
    internal class Program
    {
        private static void Main()
        {
            //ClusterTest();
            //FastestTest();
            FastestOptimizeTest();
            Console.ReadKey();
        }

        public static void ClusterTest()
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

            var gc = new GeoClustering();

            var cluster = gc.Cluster(coordinates, 3);

            Console.WriteLine();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine();

            foreach (var clusterPoint in cluster)
            {
                foreach (var coordinate in clusterPoint.Coordinates)
                {
                    Console.WriteLine($"{coordinates.FindIndex(x => Math.Abs(x.Longitude - coordinate.Longitude) <= 0) + 1} | {coordinate}");
                }
            }
        }

        public static void FastestTest()
        {
            // 31 Trương phước phan, 10.7756192, 106.6237788

            // 435 hoàng văn thụ hồ chí minh, 10.7975076, 106.6558153

            // 60 cộng hoà, 10.8011653, 106.657283

            // 10 Tân kỳ tân quý hồ chí minh, 10.7922362, 106.605514

            // 30 âu cơ hồ chí minh, 10.7877314, 106.64095250000003

            var coordinates = new List<Coordinate.Models.Coordinate>
            {
                new Coordinate.Models.Coordinate(106.6237788,10.7756192),
                new Coordinate.Models.Coordinate(106.6558153,10.7975076),
                new Coordinate.Models.Coordinate(106.657283,10.8011653) ,
                new Coordinate.Models.Coordinate(106.605514,10.7922362),
                new Coordinate.Models.Coordinate(106.64095250000003,10.7877314)
            };

            Console.WriteLine();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("AToZ TRIP");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine();

            FastestAzTrip fastestAzTrip = new FastestAzTrip(coordinates);

            List<Coordinate.Models.Coordinate> azTripCoordinates = fastestAzTrip.GetTrip();

            foreach (var coordinate in azTripCoordinates)
            {
                Console.WriteLine($"{coordinates.FindIndex(x => Math.Abs(x.Longitude - coordinate.Longitude) <= 0) + 1} | {coordinate}");
            }

            Console.WriteLine();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("ROUND TRIP");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine();

            FastestRoundTrip fastestRoundTrip = new FastestRoundTrip(coordinates);

            List<Coordinate.Models.Coordinate> roundTripCoordinates = fastestRoundTrip.GetTrip();

            foreach (var coordinate in roundTripCoordinates)
            {
                Console.WriteLine($"{coordinates.FindIndex(x => Math.Abs(x.Longitude - coordinate.Longitude) <= 0) + 1} | {coordinate}");
            }
        }

        public static void FastestOptimizeTest()
        {
            // 31 Trương phước phan, 10.7756192, 106.6237788

            // 435 hoàng văn thụ hồ chí minh, 10.7975076, 106.6558153

            // 60 cộng hoà, 10.8011653, 106.657283

            // 10 Tân kỳ tân quý hồ chí minh, 10.7922362, 106.605514

            // 30 âu cơ hồ chí minh, 10.7877314, 106.64095250000003

            var coordinates = new List<Coordinate.Models.Coordinate>
            {
                new Coordinate.Models.Coordinate(106.6237788,10.7756192),
                new Coordinate.Models.Coordinate(106.6558153,10.7975076),
                new Coordinate.Models.Coordinate(106.657283,10.8011653) ,
                new Coordinate.Models.Coordinate(106.605514,10.7922362),
                new Coordinate.Models.Coordinate(106.64095250000003,10.7877314)
            };

            Console.WriteLine();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("AToZ TRIP");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine();

            FastestTrip fastestAzTrip = new FastestTrip(coordinates, FastestTrip.FastestTripMode.AtoZ);

            List<Coordinate.Models.Coordinate> azTrip = fastestAzTrip.GetTrip();

            foreach (var coordinate in azTrip)
            {
                Console.WriteLine($"{coordinates.FindIndex(x => Math.Abs(x.Longitude - coordinate.Longitude) <= 0) + 1} | {coordinate}");
            }

            double totalAzTripDistance = fastestAzTrip.GetTotalDistance();

            Console.WriteLine($"Total Distance {totalAzTripDistance}");

            double totalAzTripDuration = fastestAzTrip.GetTotalDuration();

            Console.WriteLine($"Total Duration {totalAzTripDuration}");

            Console.WriteLine();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("ROUND TRIP");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine();

            FastestTrip fastestRoundTrip = new FastestTrip(coordinates, FastestTrip.FastestTripMode.RoundTrip);

            List<Coordinate.Models.Coordinate> roundTrip = fastestRoundTrip.GetTrip();

            foreach (var coordinate in roundTrip)
            {
                Console.WriteLine($"{coordinates.FindIndex(x => Math.Abs(x.Longitude - coordinate.Longitude) <= 0) + 1} | {coordinate}");
            }

            double totalRoundTripDistance = fastestRoundTrip.GetTotalDistance();

            Console.WriteLine($"Total Distance {totalRoundTripDistance}");

            double totalRoundTripDuration = fastestRoundTrip.GetTotalDuration();

            Console.WriteLine($"Total Duration {totalRoundTripDuration}");
        }
    }
}