#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> XDocumentExtensions.cs </Name>
//         <Created> 07/06/2017 9:56:00 PM </Created>
//         <Key> 6f85b7fa-0436-49e8-8f00-227d83be3ef5 </Key>
//     </File>
//     <Summary>
//         XDocumentExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Core.StringUtils;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Puppy.Core
{
    /// <summary>
    ///     <see cref="XDocument" /> extension methods 
    /// </summary>
    public static class XDocumentExtensions
    {
        /// <summary>
        ///     Returns a <see cref="string" /> that represents the XML document in the specified encoding. 
        /// </summary>
        /// <param name="document"> The document. </param>
        /// <param name="encoding"> The encoding. </param>
        /// <returns> A <see cref="string" /> that represents the XML document. </returns>
        public static string ToString(this XDocument document, Encoding encoding)
        {
            var stringBuilder = new StringBuilder();

            using (StringWriter stringWriter = new StringWriterWithEncoding(stringBuilder, encoding))
            {
                document.Save(stringWriter);
            }

            return stringBuilder.ToString();
        }
    }
}