#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> Coordinate.cs </Name>
//         <Created> 27 Apr 17 1:53:28 PM </Created>
//         <Key> ddda2a63-fa3e-48c5-9b74-92d8f5db9b60 </Key>
//     </File>
//     <Summary>
//         Coordinate.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

namespace TopCore.Framework.Coordinate
{
    public class Coordinate
    {
        public Coordinate(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public double Longitude { get; }
        public double Latitude { get; }
    }
}