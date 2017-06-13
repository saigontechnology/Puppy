#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> UserAgentHttpHeaderAttribute.cs </Name>
//         <Created> 07/06/2017 11:06:13 PM </Created>
//         <Key> e3b9048c-47ed-448f-89ce-4ca7f03a6264 </Key>
//     </File>
//     <Summary>
//         UserAgentHttpHeaderAttribute.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Puppy.Web.Attributes
{
    /// <summary>
    ///     Require the User-Agent HTTP header to be specified in a request. 
    /// </summary>
    /// <seealso cref="HttpHeaderAttribute" />
    public class UserAgentHttpHeaderAttribute : HttpHeaderAttribute
    {
        private const string UserAgentHttpHeaderName = "User-Agent";

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserAgentHttpHeaderAttribute" /> class. 
        /// </summary>
        public UserAgentHttpHeaderAttribute()
            : base(UserAgentHttpHeaderName)
        {
        }

        /// <summary>
        ///     Returns <c> true </c> if the User-Agent HTTP header value is valid, otherwise returns
        ///     <c> false </c>.
        /// </summary>
        /// <param name="headerValues"> The header values. </param>
        /// <returns>
        ///     <c> true </c> if the User-Agent HTTP header values are valid; otherwise, <c> false </c>.
        /// </returns>
        public override bool IsValid(StringValues headerValues)
        {
            var isValid = false;

            if (!StringValues.IsNullOrEmpty(headerValues))
            {
                var values = headerValues
                    .SelectMany(x => Split(x))
                    .Select(
                        x =>
                        {
                            var parsed = ProductInfoHeaderValue.TryParse(x, out ProductInfoHeaderValue productInfo);
                            return new
                            {
                                Parsed = parsed,
                                ProductInfo = productInfo
                            };
                        })
                    .ToArray();

                isValid = values.All(x => x.Parsed) &&
                          values
                              .Where(x => x.ProductInfo.Product != null)
                              .All(x => !string.IsNullOrWhiteSpace(x.ProductInfo.Product.Name) &&
                                        !string.IsNullOrWhiteSpace(x.ProductInfo.Product.Version));
            }

            return isValid;
        }

        private static IEnumerable<string> Split(string value)
        {
            var inComment = false;
            var startIndex = 0;
            for (var i = 0; i < value.Length; ++i)
            {
                var c = value[i];

                switch (c)
                {
                    case '(':
                        inComment = true;
                        break;

                    case ')':
                        inComment = false;
                        break;

                    case ' ':
                        if (!inComment)
                        {
                            var subValue = value.Substring(startIndex, i - startIndex);
                            startIndex = i;
                            yield return subValue;
                        }

                        break;

                    default:
                        break;
                }

                if (i == value.Length - 1)
                    yield return value.Substring(startIndex, i - startIndex + 1);
            }
        }
    }
}