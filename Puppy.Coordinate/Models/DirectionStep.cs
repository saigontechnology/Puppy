#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2018 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> DirectionStep.cs </Name>
//         <Created> 22/01/2018 6:43:57 PM </Created>
//         <Key> 5e315838-f901-4b00-858a-9bab6bbb5685 </Key>
//     </File>
//     <Summary>
//         DirectionStep.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Collections.Generic;

namespace Puppy.Coordinate.Models
{
    public class DirectionStep
    {
        public int Index { get; set; }

        public string Description { get; set; }

        public double Distance { get; set; }

        public string DistanceText { get; set; }

        public double Duration { get; set; }

        public string DurationText { get; set; }
    }

    public class DirectionSteps
    {
        public Models.Coordinate OriginPoint { get; set; }

        public Models.Coordinate DestinationPoint { get; set; }

        public double TotalDuration { get; set; }

        public string TotalDurationText { get; set; }

        public double TotalDistance { get; set; }

        public string TotalDistanceText { get; set; }

        public string OriginAddress { get; set; }

        public string DestinationAddress { get; set; }

        public List<DirectionStep> Steps { get; set; }
    }
}