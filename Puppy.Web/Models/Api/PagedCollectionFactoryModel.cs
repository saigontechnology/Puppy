using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;

namespace Puppy.Web.Models.Api
{
    public class PagedCollectionFactoryModel<T> where T : class, new()
    {
        public const string TakeParam = "take";
        public const string SkipParam = "skip";
        public const string TermsParam = "terms";

        private readonly ILinkViewModel _meta;

        public PagedCollectionFactoryModel(ILinkViewModel meta)
        {
            _meta = meta;
        }

        public PagedCollectionModel<T> Generate(ICollection<T> items, int skip, int take, string terms, long total)
        {
            if (total <= 0)
            {
                return new PagedCollectionModel<T>
                {
                    Meta = _meta,
                };
            }

            var pagedCollectionViewModel = new PagedCollectionModel<T>
            {
                Meta = _meta,
                Total = total,
                Items = items,
                First = GetFirstLink(skip, take, terms),
                Last = GetLastLink(total, skip, take, terms),
                Next = GetNextLink(total, skip, take, terms),
                Previous = GetPreviousLink(total, skip, take, terms)
            };

            return pagedCollectionViewModel;
        }

        private ILinkViewModel GetFirstLink(int skip, int take, string terms)
        {
            if (skip == 0)
                return null;

            var newLink = new PlaceholderLinkModel(_meta);
            SafeSetValue(newLink.Values, SkipParam, 0);
            SafeSetValue(newLink.Values, TakeParam, take);
            SafeSetValue(newLink.Values, TermsParam, terms);
            newLink.Href = GetNewHrefByValues(newLink.Values);
            return newLink;
        }

        private ILinkViewModel GetLastLink(long total, int skip, int take, string terms)
        {
            if (total <= take)
                return null;

            var skipToNext = skip + take;

            if (skipToNext >= total)
                return null;

            var skipToLast = Math.Ceiling((total - (double)take) / take) * take;
            var newLink = new PlaceholderLinkModel(_meta);
            SafeSetValue(newLink.Values, SkipParam, skipToLast);
            SafeSetValue(newLink.Values, TakeParam, take);
            SafeSetValue(newLink.Values, TermsParam, terms);

            newLink.Href = GetNewHrefByValues(newLink.Values);

            return newLink;
        }

        private ILinkViewModel GetNextLink(long total, int skip, int take, string terms)
        {
            var skipToNext = skip + take;

            if (skipToNext >= total)
                return null;

            var newLink = new PlaceholderLinkModel(_meta);
            SafeSetValue(newLink.Values, SkipParam, take);
            SafeSetValue(newLink.Values, TakeParam, skipToNext);
            SafeSetValue(newLink.Values, TermsParam, terms);

            newLink.Href = GetNewHrefByValues(newLink.Values);

            return newLink;
        }

        private ILinkViewModel GetPreviousLink(long total, int skip, int take, string terms)
        {
            if (skip == 0 || total <= skip)
                return null;

            var skipToPrevious = Math.Max(skip - take, 0);

            if (skipToPrevious <= 0)
                return GetFirstLink(skip, take, terms);

            var newLink = new PlaceholderLinkModel(_meta);
            SafeSetValue(newLink.Values, SkipParam, take);
            SafeSetValue(newLink.Values, TakeParam, skipToPrevious);
            SafeSetValue(newLink.Values, TermsParam, terms);

            newLink.Href = GetNewHrefByValues(newLink.Values);

            return newLink;
        }

        private static void SafeSetValue(RouteValueDictionary routeValueDictionary, string key, object value)
        {
            if (routeValueDictionary.ContainsKey(key))
                routeValueDictionary.Remove(key);
            routeValueDictionary.Add(key, value);
        }

        private string GetNewHrefByValues(RouteValueDictionary routeValueDictionary)
        {
            var href = _meta.Href;
            foreach (var newLinkValue in routeValueDictionary)
            {
                var hrefKey = "{" + newLinkValue.Key + "}";
                if (href.Contains(hrefKey))
                {
                    href = href.Replace(hrefKey, newLinkValue.Value?.ToString() ?? string.Empty);
                }
                else
                {
                    var hrefValue = newLinkValue.Value?.ToString();

                    if (string.IsNullOrWhiteSpace(hrefValue))
                        continue;

                    href += href.Contains("?") ? "&" : "?";
                    var hrefParam = $"{newLinkValue.Key}={hrefValue}";
                    href += hrefParam;
                }
            }

            return href;
        }
    }
}