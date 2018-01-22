using Puppy.Coordinate;
using System;
using System.Collections.Generic;

namespace Puppy.ConsoleTesting
{
    internal class Program
    {
        private static void Main()
        {
            ClusterTest();

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

            var coordinates = new List<Coordinate.Coordinate>
            {
                new Coordinate.Coordinate(106.66691070000002, 10.8002712),
                new Coordinate.Coordinate(106.66909299999998, 10.8001182),
                new Coordinate.Coordinate(106.6677267, 10.7993045),
                new Coordinate.Coordinate(106.66926460000002, 10.798618),
                new Coordinate.Coordinate(106.6654264, 10.7978616),
                new Coordinate.Coordinate(106.66459699999996, 10.7991835),
                new Coordinate.Coordinate(106.66446250000001, 10.7985912),
                new Coordinate.Coordinate(106.6644063, 10.7982545),
                new Coordinate.Coordinate(106.66749460000005, 10.7972986),
                new Coordinate.Coordinate(106.66693399999997, 10.7990973)
            };

            var gc = new GeoClustering();

            var cluster = gc.Cluster(coordinates, 3);

            Console.WriteLine();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine();

            foreach (var clusterPoint in cluster)
            foreach (var coordinate in clusterPoint.Coordinates)
                Console.WriteLine(
                    $"{coordinates.FindIndex(x => Math.Abs(x.Longitude - coordinate.Longitude) <= 0) + 1} | {coordinate}");
        }
    }
}