#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> CoordinatePositionExtension.cs </Name>
//         <Created> 27 Apr 17 1:58:17 PM </Created>
//         <Key> 8dbb416c-7bd6-4f3c-a88e-e64d8503ed12 </Key>
//     </File>
//     <Summary>
//         CoordinatePositionExtension.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace Topcore.Framework.Coordinate
{
	public static class CoordinatePositionExtension
	{
		/// <summary>
		///     Calculates the end-point from a given source at a given range (meters) and bearing (degrees). This methods uses simple geometry equations to calculate the end-point. 
		/// </summary>
		/// <param name="src">         Point of origin </param>
		/// <param name="radiusMiles"> Radius/Range in meters </param>
		/// <param name="bearing">     Bearing in degrees from 0 to 360 </param>
		/// <returns> End-point from the source given the desired range and bearing. </returns>
		public static Coordinate GetDerivedPosition(this Coordinate src, double radiusMiles, double bearing)
		{
			var latSrc = src.Latitude * CoordinateConst.DegreesToRadians;
			var lngSrc = src.Longitude * CoordinateConst.DegreesToRadians;

			var angularDistance = radiusMiles / CoordinateConst.EarthRadiusMiles;
			var trueCourse = bearing * CoordinateConst.DegreesToRadians;

			var lat = Math.Asin(
				Math.Sin(latSrc) * Math.Cos(angularDistance) +
				Math.Cos(latSrc) * Math.Sin(angularDistance) * Math.Cos(trueCourse));

			var dlon = Math.Atan2(
				Math.Sin(trueCourse) * Math.Sin(angularDistance) * Math.Cos(latSrc),
				Math.Cos(angularDistance) - Math.Sin(latSrc) * Math.Sin(lat));

			var lon = (lngSrc + dlon + Math.PI) % (Math.PI * 2) - Math.PI;

			var result = new Coordinate(lat * CoordinateConst.RadiansToDegrees, lon * CoordinateConst.RadiansToDegrees);
			return result;
		}

		/// <summary>
		///     Get Top Left Coordinate of square (out bound of circle) corner 
		/// </summary>
		/// <param name="src">        </param>
		/// <param name="radiusMiles"></param>
		/// <returns></returns>
		public static Coordinate GetTopLeftOfSquare(this Coordinate src, double radiusMiles)
		{
			var top = GetDerivedPosition(src, radiusMiles, 0);
			var left = GetDerivedPosition(src, radiusMiles, 270);
			var topLeft = new Coordinate(left.Latitude, top.Longitude);
			return topLeft;
		}

		/// <summary>
		///     Get Bot Right Coordinate of square (out bound of circle) corner 
		/// </summary>
		/// <param name="src">        </param>
		/// <param name="radiusMiles"></param>
		/// <returns></returns>
		public static Coordinate GetBotRightOfSquare(this Coordinate src, double radiusMiles)
		{
			var bot = GetDerivedPosition(src, radiusMiles, 180);
			var right = GetDerivedPosition(src, radiusMiles, 90);
			var botRight = new Coordinate(right.Latitude, bot.Longitude);
			return botRight;
		}
	}
}