#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> TwitterImage.cs </Name>
//         <Created> 07/06/2017 10:17:57 PM </Created>
//         <Key> 483defca-5a1b-4ba5-a277-facb184063f3 </Key>
//     </File>
//     <Summary>
//         TwitterImage.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace Puppy.Web.SEO.Twitter
{
    /// <summary>
    ///     An image used in a Twitter card. The Image must be less than 1MB in size. 
    /// </summary>
    public class TwitterImage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TwitterImage" /> class. 
        /// </summary>
        /// <param name="imageUrl"> The image URL. The Image must be less than 1MB in size. </param>
        public TwitterImage(string imageUrl)
        {
            ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TwitterImage" /> class. 
        /// </summary>
        /// <param name="imageUrl"> The image URL. The Image must be less than 1MB in size. </param>
        /// <param name="width">   
        ///     The width of the image in pixels to help Twitter accurately preserve the aspect ratio
        ///     of the image when resizing. This property is optional.
        /// </param>
        /// <param name="height">  
        ///     The height of the image in pixels to help Twitter accurately preserve the aspect
        ///     ratio of the image when resizing. This property is optional.
        /// </param>
        public TwitterImage(string imageUrl, int width, int height)
            : this(imageUrl)
        {
            Height = height;
            Width = width;
        }

        /// <summary>
        ///     Gets or sets the height of the image in pixels to help Twitter accurately preserve
        ///     the aspect ratio of the image when resizing. This property is optional.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        ///     Gets the image URL. The Image must be less than 1MB in size. 
        /// </summary>
        public string ImageUrl { get; }

        /// <summary>
        ///     Gets or sets the width of the image in pixels to help Twitter accurately preserve the
        ///     aspect ratio of the image when resizing. This property is optional.
        /// </summary>
        public int? Width { get; set; }
    }
}