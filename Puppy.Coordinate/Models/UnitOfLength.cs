#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> UnitOfLength.cs </Name>
//         <Created> 25/10/2017 9:06:30 PM </Created>
//         <Key> ac49f6dc-b164-4a31-adc5-8c5b4d243b23 </Key>
//     </File>
//     <Summary>
//         UnitOfLength.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Coordinate.Models
{
    public class UnitOfLength
    {
        public static UnitOfLength Meter = new UnitOfLength(CoordinateConst.MileToMeter);
        public static UnitOfLength Kilometer = new UnitOfLength(CoordinateConst.MileToKilometer);
        public static UnitOfLength NauticalMile = new UnitOfLength(CoordinateConst.NauticalMileToMile);
        public static UnitOfLength Mile = new UnitOfLength(1);

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