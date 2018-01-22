#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2018 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> GMapHelper.cs </Name>
//         <Created> 22/01/2018 6:48:33 PM </Created>
//         <Key> 188f1baf-0e1a-480c-8079-96b1a679cbd0 </Key>
//     </File>
//     <Summary>
//         GMapHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Coordinate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

namespace Puppy.Coordinate
{
    public class GMapHelper
    {
        public static List<DirectionSteps> GetDirections(Models.Coordinate origin, Models.Coordinate destination, List<Models.Coordinate> waypoints, bool avoidHighways = false, bool avoidTolls = false, int unitSystem = 1, string travelMode = "DRIVING")
        {
            string originFormat = $"{origin.Latitude},{origin.Longitude}";

            string destinationFormat = $"{destination.Latitude},{destination.Longitude}";

            string waypointsFormat = string.Join("|", waypoints.Select(x => $"{x.Latitude},{x.Longitude}"));

            var requestUrl = $"https://maps.googleapis.com/maps/api/directions/xml?avoidHighways={avoidHighways}&avoidTolls={avoidTolls}&unitSystem={unitSystem}&travelMode={travelMode}&origin={originFormat}&destination={destinationFormat}&waypoints={waypointsFormat}";

            try
            {
                var client = new WebClient { Encoding = Encoding.UTF8 };

                var result = client.DownloadString(requestUrl);

                return ParseDirectionResults(result, origin, destination, waypoints, avoidHighways, avoidTolls, unitSystem, travelMode);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static List<DirectionSteps> ParseDirectionResults(string result, Models.Coordinate origin, Models.Coordinate destination, List<Models.Coordinate> waypoints, bool avoidHighways, bool avoidTolls, int unitSystem, string travelMode)
        {
            var directionStepsList = new List<DirectionSteps>();

            var xmlDoc = new XmlDocument { InnerXml = result };

            if (xmlDoc.HasChildNodes)
            {
                var directionsResponseNode = xmlDoc.SelectSingleNode("DirectionsResponse");

                var statusNode = directionsResponseNode?.SelectSingleNode("status");

                if (statusNode != null && statusNode.InnerText.Equals("OK"))
                {
                    var legs = directionsResponseNode.SelectNodes("route/leg");

                    if (legs == null) return directionStepsList;

                    foreach (XmlNode leg in legs)
                    {
                        int stepCount = 1;
                        var stepNodes = leg.SelectNodes("step");
                        var steps = (from XmlNode stepNode in stepNodes
                                     select new DirectionStep
                                     {
                                         Index = stepCount++,
                                         Distance = Convert.ToDouble(stepNode.SelectSingleNode("distance/value").InnerText, System.Globalization.CultureInfo.InvariantCulture),
                                         DistanceText = stepNode.SelectSingleNode("distance/text").InnerText,
                                         Duration = Convert.ToDouble(stepNode.SelectSingleNode("duration/value").InnerText, System.Globalization.CultureInfo.InvariantCulture),
                                         DurationText = stepNode.SelectSingleNode("duration/text").InnerText,
                                         Description = WebUtility.HtmlDecode(stepNode.SelectSingleNode("html_instructions").InnerText)
                                     }).ToList();

                        var directionSteps = new DirectionSteps
                        {
                            OriginPoint = new Models.Coordinate
                            {
                                SequenceNo = -1,
                                Latitude = Convert.ToDouble(leg.SelectSingleNode("start_location/lat").InnerText, System.Globalization.CultureInfo.InvariantCulture),
                                Longitude = Convert.ToDouble(leg.SelectSingleNode("start_location/lng").InnerText, System.Globalization.CultureInfo.InvariantCulture)
                            },
                            OriginAddress = leg.SelectSingleNode("start_address").InnerText,
                            DestinationPoint = new Models.Coordinate
                            {
                                SequenceNo = -1,
                                Latitude = Convert.ToDouble(leg.SelectSingleNode("end_location/lat").InnerText, System.Globalization.CultureInfo.InvariantCulture),
                                Longitude = Convert.ToDouble(leg.SelectSingleNode("end_location/lng").InnerText, System.Globalization.CultureInfo.InvariantCulture)
                            },
                            DestinationAddress = leg.SelectSingleNode("end_address").InnerText,
                            TotalDistance = Convert.ToDouble(leg.SelectSingleNode("distance/value").InnerText, System.Globalization.CultureInfo.InvariantCulture),
                            TotalDistanceText = leg.SelectSingleNode("distance/text").InnerText,
                            TotalDuration = Convert.ToDouble(leg.SelectSingleNode("duration/value").InnerText, System.Globalization.CultureInfo.InvariantCulture),
                            TotalDurationText = leg.SelectSingleNode("duration/text").InnerText,
                            Steps = steps
                        };

                        directionStepsList.Add(directionSteps);
                    }
                }
                else if (statusNode != null && statusNode.InnerText.Equals("OVER_QUERY_LIMIT"))
                {
                    Thread.Sleep(1000);
                    directionStepsList = GetDirections(origin, destination, waypoints, avoidHighways, avoidTolls, unitSystem, travelMode);
                }
                else
                {
                    throw new NotSupportedException(statusNode?.InnerText);
                }
            }
            return directionStepsList;
        }
    }
}