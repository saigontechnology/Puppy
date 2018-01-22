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

using Puppy.Core.ObjectUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.Coordinate
{
    public class GeoClustering
    {
        public List<Cluster> Cluster(List<Coordinate> coordinates, int maximumGroup)
        {
            if (maximumGroup <= 1 || maximumGroup >= coordinates.Count)
            {
                throw new NotSupportedException($"{nameof(maximumGroup)} <= 1 || {nameof(maximumGroup)} >= total {nameof(coordinates)}");
            }

            // Step 1: random first center Coordinate

            var centerCoordinates = InitialCenterCoordinates(coordinates, maximumGroup);

            bool modified;

            do
            {
                modified = false;

                // Step 2: put Coordinates to group based on closest distance

                foreach (var coordinate in coordinates)
                {
                    var newGroup = GetCloseCoordinateIndex(centerCoordinates, coordinate);

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

            var clusters = new List<Cluster>(maximumGroup);

            for (int i = 0; i < maximumGroup; i++)
            {
                clusters.Add(new Cluster
                {
                    CenterCoordinate = centerCoordinates[i],

                    Coordinates = coordinates.Where(p => p.GroupNo == i).ToList()
                });
            }

            return clusters;
        }

        private List<Coordinate> InitialCenterCoordinates(IList<Coordinate> coordinates, int maximumGroup)
        {
            if (maximumGroup <= 1 || maximumGroup >= coordinates.Count)
            {
                throw new NotSupportedException($"{nameof(maximumGroup)} <= 1 || {nameof(maximumGroup)} >= total Coordinate");
            }

            Random random = new Random();

            for (var i = 0; i < maximumGroup; i++)
            {
                var j = random.Next(coordinates.Count);

                Swap(coordinates, i, j);
            }

            List<Coordinate> centerCoordinates = new List<Coordinate>();

            var selectedCoordinates = coordinates.Take(maximumGroup).ToList();

            foreach (var selectedCoordinate in selectedCoordinates)
            {
                var centerCoordinate = selectedCoordinate.Clone();

                centerCoordinates.Add(centerCoordinate);
            }

            return centerCoordinates;
        }

        private static void RecalculateCenterCoordinates(IReadOnlyList<Coordinate> centerCoordinates, IEnumerable<Coordinate> coordinates)
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

        private static int GetCloseCoordinateIndex(IReadOnlyList<Coordinate> coordinates, Coordinate coordinate)
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