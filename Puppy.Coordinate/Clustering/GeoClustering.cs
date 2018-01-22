#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2018 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> GeoClustering.cs </Name>
//         <Created> 22/01/2018 10:33:05 AM </Created>
//         <Key> 3954718b-cdc0-4f2e-b35d-2171b94e6e39 </Key>
//     </File>
//     <Summary>
//         GeoClustering.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Coordinate.DistanceUtils;
using Puppy.Coordinate.Models;
using Puppy.Core.ObjectUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.Coordinate.Clustering
{
    public class GeoClustering
    {
        public List<Cluster> Cluster(List<Models.Coordinate> coordinates, int totalGroup)
        {
            if (totalGroup <= 1 || totalGroup >= coordinates.Count)
            {
                throw new NotSupportedException($"{nameof(totalGroup)} <= 1 || {nameof(totalGroup)} >= total {nameof(coordinates)}");
            }

            // Step 1: random first center Coordinate

            var centerCoordinates = InitialCenterCoordinates(coordinates, totalGroup);

            bool modified;

            do
            {
                modified = false;

                // Step 2: put Coordinates to group based on closest distance

                foreach (var coordinate in coordinates)
                {
                    var newGroup = GetClosestCoordinateIndex(centerCoordinates, coordinate);

                    if (newGroup == coordinate.GroupNo)
                    {
                        continue;
                    }

                    coordinate.GroupNo = newGroup;

                    modified = true;
                }

                // Step 3: Re-calculate center Coordinate for each group

                if (modified)
                {
                    RecalculateCenterCoordinates(centerCoordinates, coordinates);
                }
            } while (modified);

            var clusters = new List<Cluster>(totalGroup);

            for (int i = 0; i < totalGroup; i++)
            {
                clusters.Add(new Cluster
                {
                    CenterCoordinate = centerCoordinates[i],

                    Coordinates = coordinates.Where(p => p.GroupNo == i).ToList()
                });
            }

            return clusters;
        }

        private List<Models.Coordinate> InitialCenterCoordinates(IList<Models.Coordinate> coordinates, int totalGroup)
        {
            if (totalGroup <= 1 || totalGroup >= coordinates.Count)
            {
                throw new NotSupportedException($"{nameof(totalGroup)} <= 1 || {nameof(totalGroup)} >= total ${nameof(coordinates)}");
            }

            Random random = new Random();

            for (var i = 0; i < totalGroup; i++)
            {
                var j = random.Next(coordinates.Count);

                Swap(coordinates, i, j);
            }

            List<Models.Coordinate> centerCoordinates = new List<Models.Coordinate>();

            var selectedCoordinates = coordinates.Take(totalGroup).ToList();

            foreach (var selectedCoordinate in selectedCoordinates)
            {
                var centerCoordinate = selectedCoordinate.Clone();

                centerCoordinates.Add(centerCoordinate);
            }

            return centerCoordinates;
        }

        private static void RecalculateCenterCoordinates(IReadOnlyList<Models.Coordinate> centerCoordinates, IEnumerable<Models.Coordinate> coordinates)
        {
            var totalGroup = centerCoordinates.Count;

            var sumLat = new double[totalGroup];

            var sumLng = new double[totalGroup];

            var count = new int[totalGroup];

            foreach (var coordinate in coordinates)
            {
                sumLng[coordinate.GroupNo] += coordinate.Longitude;

                sumLat[coordinate.GroupNo] += coordinate.Latitude;

                count[coordinate.GroupNo]++;
            }
            for (var i = 0; i < totalGroup; i++)
            {
                centerCoordinates[i].Longitude = sumLng[i] / count[i];

                centerCoordinates[i].Latitude = sumLat[i] / count[i];
            }
        }

        private static int GetClosestCoordinateIndex(IReadOnlyList<Models.Coordinate> coordinates, Models.Coordinate coordinate)
        {
            var result = 0;

            var minDistance = coordinate.FlatDistanceTo(coordinates[result]);

            for (int i = 1; i < coordinates.Count; i++)
            {
                var distance = coordinate.FlatDistanceTo(coordinates[i]);

                if (distance < minDistance)
                {
                    result = i;
                }
            }

            return result;
        }

        private static void Swap<T>(IList<T> list, int firstIndex, int secondIndex)
        {
            var temp = list[firstIndex];

            list[firstIndex] = list[secondIndex];

            list[secondIndex] = temp;
        }
    }
}