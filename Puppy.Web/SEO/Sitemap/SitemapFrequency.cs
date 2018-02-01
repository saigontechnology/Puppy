#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SiteMapFrequency.cs </Name>
//         <Created> 07/06/2017 9:57:52 PM </Created>
//         <Key> 9a79e7e8-fd99-4741-a2d4-f9bccf658b27 </Key>
//     </File>
//     <Summary>
//         SiteMapFrequency.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

namespace Puppy.Web.SEO.SiteMap
{
    /// <summary>
    ///     How frequently the page is likely to change. This value provides general information to
    ///     search engines and may not correlate exactly to how often they crawl the page.
    /// </summary>
    /// <remarks>
    ///     The value "always" should be used to describe documents that change each time they are
    ///     accessed. The value "never" should be used to describe archived URLs.
    /// </remarks>
    public enum SiteMapFrequency
    {
        /// <summary>
        ///     Describes archived URLs that never change. 
        /// </summary>
        Never,

        /// <summary>
        ///     Describes URL's that change yearly. 
        /// </summary>
        Yearly,

        /// <summary>
        ///     Describes URL's that change monthly. 
        /// </summary>
        Monthly,

        /// <summary>
        ///     Describes URL's that change weekly. 
        /// </summary>
        Weekly,

        /// <summary>
        ///     Describes URL's that change daily. 
        /// </summary>
        Daily,

        /// <summary>
        ///     Describes URL's that change hourly. 
        /// </summary>
        Hourly,

        /// <summary>
        ///     Describes documents that change each time they are accessed. 
        /// </summary>
        Always
    }
}