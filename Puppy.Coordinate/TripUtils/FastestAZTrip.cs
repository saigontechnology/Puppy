#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2018 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> FastestAZTrip.cs </Name>
//         <Created> 22/01/2018 6:45:45 PM </Created>
//         <Key> a60fa923-2852-42c6-9ba9-17e6a6ed84ff </Key>
//     </File>
//     <Summary>
//         FastestAZTrip.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Coordinate.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.Coordinate.TripUtils
{
    public class FastestAzTrip
    {
        #region Property

        private List<Models.Coordinate> Ways { get; }

        private List<Models.Coordinate> Coordinates { get; }

        private List<DirectionSteps> LegsTemp { get; }

        private List<List<DirectionSteps>> Legs { get; }

        private List<int> BestPath { get; set; }

        private List<List<double>> Durations { get; }

        private List<List<double>> Distances { get; }

        private List<double> DurationsTemp { get; }

        private List<double> DistancesTemp { get; }

        private int DistIndex { get; set; }

        private int ChunkNode { get; set; }

        private int OkChunkNode { get; set; }

        #endregion Property

        public FastestAzTrip(List<Models.Coordinate> coordinates)
        {
            if (coordinates.Count < 3)
            {
                throw new NotSupportedException();
            }

            DistIndex = 0;

            Legs = new List<List<DirectionSteps>>();

            Distances = new List<List<double>>();

            Durations = new List<List<double>>();

            LegsTemp = new List<DirectionSteps>();

            DurationsTemp = new List<double>();

            DistancesTemp = new List<double>();

            Ways = new List<Models.Coordinate>();

            Coordinates = new List<Models.Coordinate>();

            Coordinates.AddRange(coordinates);

            Ways.Add(Coordinates[0]);

            GetWays(0);

            NextChunk();
        }

        private void GetWays(int curr)
        {
            int nextAbove = -1;

            for (var i = curr + 1; i < Coordinates.Count; ++i)
            {
                if (nextAbove == -1)
                {
                    nextAbove = i;
                }
                else
                {
                    Ways.Add(Coordinates[i]);
                    Ways.Add(Coordinates[curr]);
                }
            }

            if (nextAbove != -1)
            {
                Ways.Add(Coordinates[nextAbove]);
                GetWays(nextAbove);
                Ways.Add(Coordinates[curr]);
            }
        }

        private static List<List<T>> GetPermutations<T>(IReadOnlyCollection<T> list, int length)
        {
            if (length == 1) return list.Select(t => new[] { t }).Select(x => x.ToList()).ToList();

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new[] { t2 }))
                .Select(x => x.ToList()).ToList();
        }

        public List<Models.Coordinate> GetTrip()
        {
            return BestPath.Select((t, i) => new Models.Coordinate
            {
                Latitude = Coordinates[t].Latitude,
                Longitude = Coordinates[t].Longitude,
                GroupNo = Coordinates[t].GroupNo,
                SequenceNo = i + 1
            }).ToList();
        }

        #region Helper

        private void NextChunk()
        {
            int maxSize = 10;

            ChunkNode = OkChunkNode;

            if (ChunkNode < Ways.Count)
            {
                var wayArrChunk = new List<Models.Coordinate>();

                for (var i = 0; i < maxSize && i + ChunkNode < Ways.Count; ++i)
                {
                    wayArrChunk.Add(Ways[ChunkNode + i]);
                }

                Models.Coordinate origin = wayArrChunk[0];

                Models.Coordinate destination = wayArrChunk[wayArrChunk.Count - 1];

                var wayArrChunk2 = new List<Models.Coordinate>();

                for (var i = 1; i < wayArrChunk.Count - 1; i++)
                {
                    wayArrChunk2.Add(wayArrChunk[i]);
                }

                ChunkNode += maxSize;

                if (ChunkNode < Ways.Count - 1)
                {
                    ChunkNode--;
                }

                List<DirectionSteps> directionSteps = GMapHelper.GetDirections(origin, destination, wayArrChunk2);

                LegsTemp.AddRange(directionSteps);

                OkChunkNode = ChunkNode;

                NextChunk();
            }
            else
            {
                DurationsTemp.AddRange(LegsTemp.Select(x => x.TotalDuration));

                DistancesTemp.AddRange(LegsTemp.Select(x => x.TotalDistance));

                CalculateTrip();
            }
        }

        private void CalculateTrip()
        {
            for (int i = 0; i < Coordinates.Count; i++)
            {
                Legs.Add(new List<DirectionSteps>());

                Distances.Add(new List<double>());

                Durations.Add(new List<double>());

                for (int j = 0; j < Coordinates.Count; j++)
                {
                    Legs[i].Add(new DirectionSteps());

                    Distances[i].Add(0);

                    Durations[i].Add(0);
                }
            }

            GetDistTable(0, 0);

            BestPath = new List<int>();

            double minDistance = -1;

            for (int i = 1; i < Coordinates.Count; i++)
            {
                List<int> coordinatesIndex = new List<int>();

                for (int j = 1; (j < Coordinates.Count); j++)
                {
                    if (j == i) continue;
                    coordinatesIndex.Add(j);
                }
                var coordinatesIndexPermutation = GetPermutations(coordinatesIndex, coordinatesIndex.Count);

                List<List<int>> listCoordinatesIndexPermutationComplete = new List<List<int>>();

                foreach (var coordinateIndexPermutation in coordinatesIndexPermutation)
                {
                    var coordinateIndexPermutationTemp = new List<int> { 0 };

                    coordinateIndexPermutationTemp.AddRange(coordinateIndexPermutation);

                    coordinateIndexPermutationTemp.Add(i);

                    listCoordinatesIndexPermutationComplete.Add(coordinateIndexPermutationTemp);
                }

                // get min
                foreach (var coordinatesIndexPermutationComplete in listCoordinatesIndexPermutationComplete)
                {
                    double pathDistance = 0;

                    for (int j = 0; j < coordinatesIndexPermutationComplete.Count - 1; j++)
                    {
                        pathDistance += Distances[coordinatesIndexPermutationComplete[j]][coordinatesIndexPermutationComplete[j + 1]];
                    }

                    if (minDistance < 0 || pathDistance < minDistance)
                    {
                        BestPath = coordinatesIndexPermutationComplete;
                        minDistance = pathDistance;
                    }
                }
            }
            // get min of results is best path
        }

        private void GetDistTable(int curr, int currInd)
        {
            var nextAbove = -1;

            var index = currInd;

            for (var i = curr + 1; i < Coordinates.Count; ++i)
            {
                index++;

                if (nextAbove == -1)
                {
                    nextAbove = i;
                }
                else
                {
                    Legs[currInd][index] = LegsTemp[DistIndex];

                    Distances[currInd][index] = DistancesTemp[DistIndex];

                    Durations[currInd][index] = DurationsTemp[DistIndex++];

                    Legs[index][currInd] = LegsTemp[DistIndex];

                    Distances[index][currInd] = DistancesTemp[DistIndex];

                    Durations[index][currInd] = DurationsTemp[DistIndex++];
                }
            }
            if (nextAbove != -1)
            {
                Legs[currInd][currInd + 1] = LegsTemp[DistIndex];

                Distances[currInd][currInd + 1] = DistancesTemp[DistIndex];

                Durations[currInd][currInd + 1] = DurationsTemp[DistIndex++];

                GetDistTable(nextAbove, currInd + 1);

                Legs[currInd + 1][currInd] = LegsTemp[DistIndex];

                Distances[currInd + 1][currInd] = DistancesTemp[DistIndex];

                Durations[currInd + 1][currInd] = DurationsTemp[DistIndex++];
            }
        }

        #endregion Helper
    }
}