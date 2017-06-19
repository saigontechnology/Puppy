#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> CardType.cs </Name>
//         <Created> 07/06/2017 10:18:44 PM </Created>
//         <Key> 64d9e413-84f2-4c17-a219-c32a47569e03 </Key>
//     </File>
//     <Summary>
//         CardType.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

namespace Puppy.Web.SEO.Twitter.Enums
{
    /// <summary>
    ///     The type of Twitter card. 
    /// </summary>
    public enum CardType
    {
        /// <summary>
        ///     The App Card is a great way to represent mobile applications on Twitter and to drive
        ///     installs. The app card is designed to allow for a name, description and icon, and
        ///     also to highlight attributes such as the rating and the price. This Card type is
        ///     currently available on the twitter.com website, as well as iOS and Android mobile
        ///     clients. It is not yet available on mobile web. See https://dev.twitter.com/cards/types/app.
        /// </summary>
        App,

        /// <summary>
        ///     Video clips and audio streams have a special place on the Twitter platform thanks to
        ///     the Player Card. By implementing a few HTML meta tags to your website and following
        ///     the Twitter Rules of the Road, you can deliver your rich media to users across the
        ///     globe. Twitter must approve the use of the player card, find out more below. See https://dev.twitter.com/cards/types/player
        /// </summary>
        Player,

        /// <summary>
        ///     The Summary Card can be used for many kinds of web content, from blog posts and news
        ///     articles, to products and restaurants. It is designed to give the reader a preview of
        ///     the content before clicking through to your website. See https://dev.twitter.com/cards/types/summary.
        /// </summary>
        Summary,

        /// <summary>
        ///     The Summary Card with Large Image features a large, full-width prominent image
        ///     alongside a tweet. It is designed to give the reader a rich photo experience, and
        ///     clicking on the image brings the user to your website. On twitter.com and the mobile
        ///     clients, the image appears below the tweet text. See https://dev.twitter.com/cards/types/summary-large-image.
        /// </summary>
        SummaryLargeImage
    }
}