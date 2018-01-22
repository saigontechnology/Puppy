#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2018 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> Cluster.cs </Name>
//         <Created> 22/01/2018 10:30:52 AM </Created>
//         <Key> a3150474-0446-4026-96d7-441ab85ee274 </Key>
//     </File>
//     <Summary>
//         Cluster.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Collections.Generic;

namespace Puppy.Coordinate.Models
{
    public class Cluster
    {
        public Coordinate CenterCoordinate { get; set; }

        public List<Coordinate> Coordinates { get; set; } = new List<Coordinate>();
    }
}