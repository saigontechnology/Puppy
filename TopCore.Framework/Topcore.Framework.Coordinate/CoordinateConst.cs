#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> CoordinateConst.cs </Name>
//         <Created> 27 Apr 17 2:02:14 PM </Created>
//         <Key> d0f42562-dabf-4c3c-89d0-2a141feb8d58 </Key>
//     </File>
//     <Summary>
//         CoordinateConst.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace Topcore.Framework.Coordinate
{
	public class CoordinateConst
	{
		public const double KilometersToMiles = 1.609344;
		public const double NauticalMilesToMiles = 0.8684;

		public const double DegreesToRadians = Math.PI / 180.0;
		public const double RadiansToDegrees = 180.0 / Math.PI;
		public const double EarthRadiusKilometers = RadiansToDegrees * 60 * 1.1515;
		public const double EarthRadiusMiles = EarthRadiusKilometers * KilometersToMiles;
	}
}