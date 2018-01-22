#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2018 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> FastestRoundTrip.cs </Name>
//         <Created> 22/01/2018 6:55:54 PM </Created>
//         <Key> 6261a6f4-e6f9-4ceb-aca1-615d244f3d03 </Key>
//     </File>
//     <Summary>
//         FastestRoundTrip.cs
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
    public class FastestRoundTrip
    {
        #region Property

        private List<Models.Coordinate> Ways { get; }

        private List<Models.Coordinate> Coordinates { get; }

        private List<DirectionSteps> LegsTemp { get; }

        private List<List<DirectionSteps>> Legs { get; }

        private List<double> DurationsTemp { get; }

        private List<List<double>> Durations { get; }

        private List<double> DistancesTemp { get; }

        private List<List<double>> Distances { get; }

        private List<int> BestPath { get; set; }

        private List<int> NextSet { get; set; }

        private double BestTrip { get; set; }

        private int ChunkNode { get; set; }

        private int OkChunkNode { get; set; }

        private int DistIndex { get; set; }

        private const double MaxTripSentry = 2000000000;

        #endregion Property

        /// <summary>
        ///     Concorde TSP Solver algorithm combine with Ant colony optimization algorithms to find
        ///     waypoint and best path
        /// </summary>
        /// <param name="coordinates"></param>
        public FastestRoundTrip(List<Models.Coordinate> coordinates)
        {
            if (coordinates.Count < 3) throw new NotSupportedException();

            DistIndex = 0;

            LegsTemp = new List<DirectionSteps>();

            DurationsTemp = new List<double>();

            DistancesTemp = new List<double>();

            Legs = new List<List<DirectionSteps>>();

            Distances = new List<List<double>>();

            Durations = new List<List<double>>();

            Ways = new List<Models.Coordinate>();

            Coordinates = new List<Models.Coordinate>();

            Coordinates.AddRange(coordinates);

            Ways.Add(coordinates[0]);

            GetWays(0);

            NextChunk();
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

        private void GetWays(int curr)
        {
            var nextAbove = -1;

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

        private void NextChunk()
        {
            var maxSize = 10;

            ChunkNode = OkChunkNode;

            if (ChunkNode < Ways.Count)
            {
                var wayArrChunk = new List<Models.Coordinate>();

                for (var i = 0; i < maxSize && i + ChunkNode < Ways.Count; ++i)
                {
                    wayArrChunk.Add(Ways[ChunkNode + i]);
                }

                var origin = wayArrChunk[0];

                var destination = wayArrChunk[wayArrChunk.Count - 1];

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

                var directionSteps = GMapHelper.GetDirections(origin, destination, wayArrChunk2);

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
            for (var i = 0; i < Coordinates.Count; i++)
            {
                Legs.Add(new List<DirectionSteps>());

                Distances.Add(new List<double>());

                Durations.Add(new List<double>());

                for (var j = 0; j < Coordinates.Count; j++)
                {
                    Legs[i].Add(new DirectionSteps());

                    Distances[i].Add(0);

                    Durations[i].Add(0);
                }
            }

            GetDistTable(0, 0);

            BestPath = new List<int>();

            NextSet = new List<int>();

            BestTrip = MaxTripSentry;

            var numActive = Coordinates.Count;

            var numCombos = 1 << Coordinates.Count;

            var c = new List<List<double>>();

            var parent = new List<List<int>>();

            for (var i = 0; i < numCombos; i++)
            {
                c.Add(new List<double>());

                parent.Add(new List<int>());

                for (var j = 0; j < numActive; ++j)
                {
                    c[i].Add(0.0);

                    parent[i].Add(0);
                }
            }

            int index;

            for (var k = 1; k < numActive; ++k)
            {
                index = 1 + (1 << k);

                c[index][k] = Durations[0][k];
            }

            for (var s = 3; s <= numActive; ++s)
            {
                for (var i = 0; i < numActive; ++i)
                {
                    NextSet.Add(0);
                }

                index = NextSetOf(s);

                while (index >= 0)
                {
                    for (var k = 1; k < numActive; ++k)
                    {
                        if (NextSet[k] != 0)
                        {
                            var previousIndex = index - (1 << k);

                            c[index][k] = MaxTripSentry;

                            for (var m = 1; m < numActive; ++m)
                            {
                                if (NextSet[m] != 0 && m != k)
                                {
                                    if (c[previousIndex][m] + Durations[m][k] < c[index][k])
                                    {
                                        c[index][k] = c[previousIndex][m] + Durations[m][k];
                                        parent[index][k] = m;
                                    }
                                }
                            }
                        }
                    }
                    index = NextSetOf(s);
                }
            }

            for (var i = 0; i < numActive; ++i)
            {
                BestPath.Add(0);
            }

            index = (1 << numActive) - 1;

            var currentNode = -1;

            BestPath.Add(0);

            for (var i = 1; i < numActive; ++i)
            {
                if (c[index][i] + Durations[i][0] < BestTrip)
                {
                    BestTrip = c[index][i] + Durations[i][0];

                    currentNode = i;
                }
            }

            BestPath[numActive - 1] = currentNode;

            for (var i = numActive - 1; i > 0; --i)
            {
                currentNode = parent[index][currentNode];

                index -= 1 << BestPath[i];

                BestPath[i - 1] = currentNode;
            }
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

        private int NextSetOf(int num)
        {
            var numActive = Coordinates.Count;

            var count = 0;

            var ret = 0;

            for (var i = 0; i < numActive; ++i) count += NextSet[i];
            {
                if (count < num)
                {
                    for (var i = 0; i < num; ++i)
                    {
                        NextSet[i] = 1;
                    }

                    for (var i = num; i < numActive; ++i)
                    {
                        NextSet[i] = 0;
                    }
                }
                else
                {
                    // Find first 1
                    var firstOne = -1;

                    for (var i = 0; i < numActive; ++i)
                    {
                        if (NextSet[i] != 0)
                        {
                            firstOne = i;
                            break;
                        }
                    }

                    // Find first 0 greater than firstOne
                    var firstZero = -1;

                    for (var i = firstOne + 1; i < numActive; ++i)
                    {
                        if (NextSet[i] == 0)
                        {
                            firstZero = i;
                            break;
                        }
                    }

                    if (firstZero < 0)
                    {
                        return -1;
                    }

                    // Increment the first zero with ones behind it

                    NextSet[firstZero] = 1;

                    // Set the part behind that one to its lowest possible value
                    for (var i = 0; i < firstZero - firstOne - 1; ++i)
                    {
                        NextSet[i] = 1;
                    }

                    for (var i = firstZero - firstOne - 1; i < firstZero; ++i)
                    {
                        NextSet[i] = 0;
                    }
                }
            }

            // Return the index for this set
            for (var i = 0; i < numActive; ++i)
            {
                ret += NextSet[i] << i;
            }

            return ret;
        }

        #endregion Helper
    }
}