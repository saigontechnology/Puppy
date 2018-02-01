#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SitemapException.cs </Name>
//         <Created> 07/06/2017 9:58:15 PM </Created>
//         <Key> 563a048a-bb9e-4c6e-b41f-30a1b4f1f528 </Key>
//     </File>
//     <Summary>
//         SitemapException.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace Puppy.Web.SEO.SiteMap
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents errors that occur during sitemap creation. 
    /// </summary>
    public class SiteMapException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:Puppy.Web.SEO.SiteMap.SiteMapException" /> class.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        public SiteMapException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:Puppy.Web.SEO.SiteMap.SiteMapException" /> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="inner">   The inner. </param>
        public SiteMapException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}