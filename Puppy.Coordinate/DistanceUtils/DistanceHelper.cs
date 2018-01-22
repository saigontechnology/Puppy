#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DistanceHelper.cs </Name>
//         <Created> 26/10/2017 9:03:04 PM </Created>
//         <Key> 8223ebf2-46be-41f2-b65e-63a64dbab85f </Key>
//     </File>
//     <Summary>
//         DistanceHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Coordinate.Models;

namespace Puppy.Coordinate.DistanceUtils
{
    public static class DistanceHelper
    {
        /// <summary>
        ///     By Haversine https://en.wikipedia.org/wiki/Haversine_formula 
        /// </summary>
        /// <returns></returns>
        public static double GetDistance(double srcLng, double srcLat, double destLng, double destLat)
        {
            var src = new Models.Coordinate(srcLng, srcLat);
            var dest = new Models.Coordinate(destLng, destLat);
            return src.DistanceToByHaversine(dest, UnitOfLength.Kilometer);
        }

        /// <summary>
        ///     By Haversine https://en.wikipedia.org/wiki/Haversine_formula 
        /// </summary>
        /// <returns></returns>
        public static double GetDistance(double srcLng, double srcLat, double destLng, double destLat, UnitOfLength unitOfLength)
        {
            var src = new Models.Coordinate(srcLng, srcLat);
            var dest = new Models.Coordinate(destLng, destLat);
            return src.DistanceToByHaversine(dest, unitOfLength);
        }
    }
}