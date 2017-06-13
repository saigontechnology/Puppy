#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> CardTypeExtensions.cs </Name>
//         <Created> 07/06/2017 10:20:03 PM </Created>
//         <Key> e109f023-2591-4d74-b2a2-365cb5937ee1 </Key>
//     </File>
//     <Summary>
//         CardTypeExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

namespace Puppy.Web.SEO.Twitter.Enums
{
    /// <summary>
    ///     <see cref="CardType" /> extension methods. 
    /// </summary>
    internal static class CardTypeExtensions
    {
        /// <summary>
        ///     Returns the Twitter specific <see cref="string" /> representation of the <see cref="CardType" />. 
        /// </summary>
        /// <param name="twitterCardType"> Type of the twitter card. </param>
        /// <returns>
        ///     The Twitter specific <see cref="string" /> representation of the <see cref="CardType" />.
        /// </returns>
        public static string ToTwitterString(this CardType twitterCardType)
        {
            switch (twitterCardType)
            {
                case CardType.App:
                    return "app";

                case CardType.Player:
                    return "player";

                case CardType.Summary:
                    return "summary";

                case CardType.SummaryLargeImage:
                    return "summary_large_image";

                default:
                    return string.Empty;
            }
        }
    }
}