#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> CoordinateModel.cs </Name>
//         <Created> 27/05/2017 12:12:22 AM </Created>
//         <Key> 26363db5-96c6-4fc2-8c78-40f2fa8cced7 </Key>
//     </File>
//     <Summary>
//         CoordinateModel.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace TopCore.Framework.Core.GoogleMapUtils
{
    public class CoordinateModel
    {
        public CoordinateModel(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}