using Microsoft.AspNetCore.Mvc;
using Puppy.Web.RouteUtils;
using System;
using System.Collections.Generic;

namespace Puppy.Web.Models.Api
{
    public class PagedCollectionFactoryModel<T> where T : class, new()
    {
        private readonly string _endpoint;
        private readonly ILinkViewModel _meta;
        private readonly int _skip;
        private readonly int _take;
        private readonly string _terms;
        private readonly long _total;
        private readonly IEnumerable<T> _items;

        public PagedCollectionFactoryModel(IUrlHelper urlHelper, int skip, int take, string terms, long total, IEnumerable<T> items, string method = "GET")
        {
            _endpoint = urlHelper.ActionContext.HttpContext.Request.Path.Value;
            _endpoint = urlHelper.AbsoluteContent(_endpoint);
            _meta = PlaceholderLinkModel.ToCollection(_endpoint, method, new { skip, take, terms });
            _skip = skip;
            _take = take;
            _terms = terms;
            _total = total;
            _items = items;
        }

        public PagedCollectionFactoryModel(string endpoint, int skip, int take, string terms, long total, IEnumerable<T> items, string method = "GET")
        {
            _endpoint = endpoint;
            _meta = PlaceholderLinkModel.ToCollection(_endpoint, method, new { skip, take, terms });
            _skip = skip;
            _take = take;
            _terms = terms;
            _total = total;
            _items = items;
        }

        public PagedCollectionModel<T> Generate()
        {
            if (_total <= 0)
            {
                return new PagedCollectionModel<T>
                {
                    Meta = _meta
                };
            }

            var pagedCollectionViewModel = new PagedCollectionModel<T>
            {
                Meta = _meta,
                Total = _total,
                Items = _items,
                First = GetFirstLink(),
                Last = GetLastLink(),
                Next = GetNextLink(),
                Previous = GetPreviousLink()
            };

            return pagedCollectionViewModel;
        }

        private ILinkViewModel GetFirstLink()
        {
            if (_skip == 0)
                return null;

            var firstPlaceHolderLink = GetPlaceholderLinkModel(0);
            return firstPlaceHolderLink;
        }

        private ILinkViewModel GetLastLink()
        {
            if (_total <= _take)
                return null;

            var skipToNext = _skip + _take;

            if (skipToNext >= _total)
                return null;

            var skipToLast = (int)(Math.Ceiling((_total - (double)_take) / _take) * _take);
            var nextPlaceHolderLink = GetPlaceholderLinkModel(skipToLast);
            return nextPlaceHolderLink;
        }

        private ILinkViewModel GetNextLink()
        {
            var skipToNext = _skip + _take;

            if (skipToNext >= _total)
                return null;

            var nextPlaceHolderLink = GetPlaceholderLinkModel(skipToNext);
            return nextPlaceHolderLink;
        }

        private ILinkViewModel GetPreviousLink()
        {
            if (_skip == 0 || _total <= _skip)
                return null;

            var skipToPrevious = Math.Max(_skip - _take, 0);

            if (skipToPrevious <= 0)
                return GetFirstLink();

            var previousPlaceHolderLink = GetPlaceholderLinkModel(skipToPrevious);
            return previousPlaceHolderLink;
        }

        private PlaceholderLinkModel GetPlaceholderLinkModel(int skip)
        {
            var placeHolderLink = new PlaceholderLinkModel(_meta);
            placeHolderLink.Values.SafeSetValue("skip", skip);
            placeHolderLink.Values.SafeSetValue("take", _take);
            placeHolderLink.Values.SafeSetValue("terms", _terms);

            placeHolderLink.Href = placeHolderLink.Values.GetUrlWithQueries(_endpoint);
            return placeHolderLink;
        }
    }
}