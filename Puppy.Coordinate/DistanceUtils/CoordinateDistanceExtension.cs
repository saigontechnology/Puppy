#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> CoordinateDistanceExtension.cs </Name>
//         <Created> 27 Apr 17 1:28:26 PM </Created>
//         <Key> 935ee223-1480-4d8b-8990-3901d4b419ee </Key>
//     </File>
//     <Summary>
//         CoordinateDistanceExtension.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Puppy.Coordinate.Models;
using System;

namespace Puppy.Coordinate.DistanceUtils
{
    public static class CoordinateDistanceExtension
    {
        /// <summary>
        ///     Distance to Destination Coordinate in Flat (2D) Map 
        /// </summary>
        /// <param name="src"> </param>
        /// <param name="dest"></param>
        /// <returns> Miles </returns>
        public static double FlatDistanceTo(this Models.Coordinate src, Models.Coordinate dest)
        {
            return Math.Abs(src.Latitude - dest.Latitude) + Math.Abs(src.Longitude - dest.Longitude);
        }

        /// <summary>
        ///     Distance to Destination Coordinate By Spherical law of cosines 
        /// </summary>
        /// <param name="src"> </param>
        /// <param name="dest"></param>
        /// <returns> UnitOfLength.Kilometer </returns>
        public static double DistanceTo(this Models.Coordinate src, Models.Coordinate dest)
        {
            return DistanceTo(src, dest, UnitOfLength.Kilometer);
        }

        /// <summary>
        ///     By Spherical law of cosines http://en.wikipedia.org/wiki/Spherical_law_of_cosines 
        /// </summary>
        public static double DistanceTo(this Models.Coordinate src, Models.Coordinate dest, UnitOfLength unitOfLength)
        {
            var theta = src.Longitude - dest.Longitude;
            var thetaRad = theta * CoordinateConst.DegreesToRadians;

            var targetRad = dest.Latitude * CoordinateConst.DegreesToRadians;
            var baseRad = src.Latitude * CoordinateConst.DegreesToRadians;

            var dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);

            dist = Math.Acos(dist);

            // calculate to earth radius by miles
            dist = dist * CoordinateConst.EarthRadiusMile;

            return unitOfLength.ConvertFromMiles(dist);
        }

        /// <summary>
        ///     By Haversine https://en.wikipedia.org/wiki/Haversine_formula 
        /// </summary>
        /// <returns></returns>
        public static double DistanceToByHaversine(this Models.Coordinate src, Models.Coordinate dest, UnitOfLength unitOfLength)
        {
            var dLat = (dest.Latitude - src.Latitude) * CoordinateConst.DegreesToRadians;
            var dLon = (dest.Longitude - src.Longitude) * CoordinateConst.DegreesToRadians;

            var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                    Math.Cos(src.Latitude * CoordinateConst.DegreesToRadians) *
                    Math.Cos(dest.Latitude * CoordinateConst.DegreesToRadians) *
                    Math.Pow(Math.Sin(dLon / 2), 2);

            // central angle, aka arc segment angular distance
            var centralAngle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var dist = CoordinateConst.EarthRadiusMile * centralAngle;

            return unitOfLength.ConvertFromMiles(dist);
        }

        /// <summary>
        ///     By Geographical distance http://en.wikipedia.org/wiki/Geographical_distance 
        /// </summary>
        public static double DistanceToByGeo(this Models.Coordinate src, Models.Coordinate dest, UnitOfLength unitOfLength)
        {
            var radLatSrc = src.Latitude * CoordinateConst.DegreesToRadians;
            var radLatDest = dest.Latitude * CoordinateConst.DegreesToRadians;
            var dLat = radLatDest - radLatSrc;
            var dLon = (dest.Longitude - src.Longitude) * CoordinateConst.DegreesToRadians;

            var a = dLon * Math.Cos((radLatSrc + radLatDest) / 2);

            // central angle, aka arc segment angular distance
            var centralAngle = Math.Sqrt(a * a + dLat * dLat);

            // great-circle (orthodromic) distance on Earth between 2 points
            var dist = CoordinateConst.EarthRadiusMile * centralAngle;
            return unitOfLength.ConvertFromMiles(dist);
        }
    }
}