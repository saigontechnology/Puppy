#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DescriptionFor.cs </Name>
//         <Created> 06/10/17 8:33:11 PM </Created>
//         <Key> e16d7277-bc83-4a28-a576-f44a0d7efeec </Key>
//     </File>
//     <Summary>
//         DescriptionFor.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Puppy.Web.HtmlHelperUtils
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent MetaDataFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, Func<ModelMetadata, string> property)
        {
            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var modelExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);

            if (modelExplorer == null)
            {
                throw new InvalidOperationException($"Failed to get model explorer for {ExpressionHelper.GetExpressionText(expression)}");
            }

            return new HtmlString(property(modelExplorer.Metadata));
        }

        /// <summary>
        ///     Get ShortName in <see cref="DisplayAttribute" /> 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html">      </param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IHtmlContent ShortNameFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return html.MetaDataFor(expression, m =>
            {
                if (!(m is DefaultModelMetadata defaultMetadata)) return m.DisplayName;

                var displayAttribute = defaultMetadata.Attributes.Attributes.OfType<DisplayAttribute>().FirstOrDefault();

                //Return a default value if the property doesn't have a DisplayAttribute
                return displayAttribute != null ? displayAttribute.ShortName : m.DisplayName;
            });
        }

        /// <summary>
        ///     Get Description in <see cref="DisplayAttribute" /> 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html">      </param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IHtmlContent DescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return html.MetaDataFor(expression, m =>
            {
                if (!(m is DefaultModelMetadata defaultMetadata)) return m.DisplayName;

                var displayAttribute = defaultMetadata.Attributes.Attributes.OfType<DisplayAttribute>().FirstOrDefault();

                //Return a default value if the property doesn't have a DisplayAttribute
                return displayAttribute != null ? displayAttribute.Description : m.DisplayName;
            });
        }
    }
}