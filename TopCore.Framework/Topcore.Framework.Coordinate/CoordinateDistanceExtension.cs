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

using System;

namespace Topcore.Framework.Coordinate
{
    public static class CoordinateDistanceExtension
    {
        /// <summary>
        ///     Distance to targetCoordinate 
        /// </summary>
        /// <param name="src"> </param>
        /// <param name="dest"></param>
        /// <returns> UnitOfLength.Miles </returns>
        public static double DistanceTo(this Coordinate src, Coordinate dest)
        {
            return DistanceTo(src, dest, UnitOfLength.Miles);
        }

        public static double DistanceTo(this Coordinate src, Coordinate dest, UnitOfLength unitOfLength)
        {
            var baseRad = Math.PI * src.Latitude / 180;
            var targetRad = Math.PI * dest.Latitude / 180;
            var theta = src.Longitude - dest.Longitude;
            var thetaRad = Math.PI * theta / 180;

            var dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);

            dist = Math.Acos(dist);

            // calculate to earth radius by meter
            dist = dist * CoordinateConst.RadiansToDegrees * 60 * 1.1515;

            return unitOfLength.ConvertFromMiles(dist);
        }

        public class UnitOfLength
        {
            public static UnitOfLength Kilometers = new UnitOfLength(CoordinateConst.KilometersToMiles);
            public static UnitOfLength NauticalMiles = new UnitOfLength(CoordinateConst.NauticalMilesToMiles);
            public static UnitOfLength Miles = new UnitOfLength(1);

            private readonly double _fromMilesFactor;

            private UnitOfLength(double fromMilesFactor)
            {
                _fromMilesFactor = fromMilesFactor;
            }

            public double ConvertFromMiles(double input)
            {
                return input * _fromMilesFactor;
            }
        }
    }
}