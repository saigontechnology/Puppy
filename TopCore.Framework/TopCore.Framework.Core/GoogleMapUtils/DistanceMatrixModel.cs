#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> DistanceMatrixModel.cs </Name>
//         <Created> 26/05/2017 11:21:44 PM </Created>
//         <Key> 50a62710-c78d-4809-aec5-1798ee881703 </Key>
//     </File>
//     <Summary>
//         DistanceMatrixModel.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;

namespace TopCore.Framework.Core.GoogleMapUtils
{
    public class DistanceMatrixModel
    {
        [JsonProperty(PropertyName = "origin_addresses")]
        public string[] OriginAddresses { get; set; }

        [JsonProperty(PropertyName = "destination_addresses")]
        public string[] DestinationAddresses { get; set; }

        [JsonProperty(PropertyName = "rows")]
        public DistanceMatrixRowModel[] Rows { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        private int[,] _distanceMatrix;

        /// <summary>
        ///     Get distance in meters from <see cref="OriginAddresses" />[i] to <see cref="DestinationAddresses" />[j] 
        /// </summary>
        public int[,] DistanceMatrix => _distanceMatrix ?? (_distanceMatrix = GetDistanceMatrix());

        private int[,] _durationMatrix;

        /// <summary>
        ///     Get duration int second from <see cref="OriginAddresses" />[i] to <see cref="DestinationAddresses" />[j] 
        /// </summary>
        public int[,] DurationMatrix => _durationMatrix ?? (_durationMatrix = GetDurationMatrix());

        private int[,] GetDistanceMatrix()
        {
            int[,] matrix = new int[OriginAddresses.Length, DestinationAddresses.Length];

            for (int i = 0; i < Rows.Length; i++)
            {
                for (int j = 0; j < Rows[i].Elements.Length; j++)
                {
                    matrix[i, j] = Rows[i].Elements[j].Distance.Value;
                }
            }

            return matrix;
        }

        private int[,] GetDurationMatrix()
        {
            int[,] matrix = new int[OriginAddresses.Length, DestinationAddresses.Length];

            for (int i = 0; i < Rows.Length; i++)
            {
                for (int j = 0; j < Rows[i].Elements.Length; j++)
                {
                    matrix[i, j] = Rows[i].Elements[j].Duration.Value;
                }
            }

            return matrix;
        }
    }

    public class DistanceMatrixRowModel
    {
        [JsonProperty(PropertyName = "elements")]
        public DistanceMatrixRowElementModel[] Elements { get; set; }
    }

    public class DistanceMatrixRowElementModel
    {
        [JsonProperty(PropertyName = "distance")]
        public DistanceMatrixElementDistanceDataModel Distance { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public DistanceMatrixElementDurationDataModel Duration { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }

    public class DistanceMatrixElementDistanceDataModel
    {
        /// <summary>
        ///     Displace text depend on "units" and "language" params 
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        /// <summary>
        ///     Value always in Meters Unit 
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }
    }

    public class DistanceMatrixElementDurationDataModel
    {
        /// <summary>
        ///     Displace text depend on "language" params 
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        /// <summary>
        ///     Value always in Second Unit 
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }
    }
}