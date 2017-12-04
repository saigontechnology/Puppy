using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Puppy.Core.StringUtils;
using Puppy.Core.XmlUtils;
using Puppy.Logger.Core.Models;
using Puppy.Web.Constants;
using Puppy.Web.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;

namespace Puppy.Logger
{
    public partial class Log
    {
        public static IQueryable<LogEntity> Where(Expression<Func<LogEntity, bool>> predicate = null)
        {
            return LogQueryExtensions.Where(predicate);
        }

        /// <summary>
        ///     Get result with fill info for HttpContext and Exception from Json. Ex call: 
        ///     <code>
        /// Log.Get(out long total, orders: x =&gt; x.CreatedTime, isOrderByDescending: true, skip: skip, take: take)
        ///     </code>
        /// </summary>
        /// <returns></returns>
        public static List<LogEntity> Get(
            out long total,
            Expression<Func<LogEntity, bool>> predicate = null,
            Expression<Func<LogEntity, object>> orders = null,
            bool isOrderByDescending = true,
            int? skip = null,
            int? take = null)
        {
            // Get result with fill info for HttpContext and Exception from Json
            var query = Where(predicate);

            // get total records in advance
            total = query.LongCount();

            // orders
            if (orders != null)
            {
                query = isOrderByDescending ? query.OrderByDescending(orders) : query.OrderBy(orders);
            }

            // skip
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            // take
            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            var result = query.Get();

            return result;
        }

        /// <summary>
        ///     <para> Create Content Result with response type is <see cref="PagedCollectionModel{LogEntity}" /> </para>
        ///     <para>
        ///         Search for <see cref="LogEntity.Id" />, <see cref="LogEntity.Message" />,
        ///         <see cref="LogEntity.Level" />, <see cref="LogEntity.CreatedTime" /> (with string
        ///         format is <c> "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK" </c>, ex: "2017-08-24T00:56:29.6271125+07:00")
        ///     </para>
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     <para>
        ///         Logger write Log with **message queue** so when create a log it *near real-time log*
        ///     </para>
        ///     <para>
        ///         Base on Http Request will return <c> ContentType XML </c> when Request Header
        ///         Accept or ContentType is XML, else return <c> ContentType Json </c>
        ///     </para>
        /// </remarks>
        public static ContentResult GetLogsContentResult(HttpContext context)
        {
            int skip = 0;
            if (context.Request.Query.TryGetValue("skip", out var skipStr))
            {
                if (int.TryParse(skipStr, out var skipInt))
                {
                    skip = skipInt;
                }
            }

            int take = 1000;
            if (context.Request.Query.TryGetValue("take", out var takeStr))
            {
                if (int.TryParse(takeStr, out var takeInt))
                {
                    take = takeInt;
                }
            }

            context.Request.Query.TryGetValue("terms", out var terms);

            Expression<Func<LogEntity, bool>> predicate = null;

            var termsNormalize = StringHelper.Normalize(terms);

            if (!string.IsNullOrWhiteSpace(termsNormalize))
            {
                predicate = x => x.Id.ToUpperInvariant().Contains(termsNormalize)
                || x.Message.ToUpperInvariant().Contains(termsNormalize)
                || x.Level.ToString().ToUpperInvariant().Contains(termsNormalize)
                || x.CreatedTime.ToString(Puppy.Core.Constants.StandardFormat.DateTimeOffSetFormat).Contains(termsNormalize);
            }

            var logs = Get(out long total, predicate: predicate, orders: x => x.CreatedTime, isOrderByDescending: true, skip: skip, take: take);

            ContentResult contentResult;

            string endpoint = context.Request.Host.Value + LoggerConfig.ViewLogUrl;

            var collectionModel = new PagedCollectionFactoryModel<LogEntity>(endpoint, skip, take, terms, total, logs, null, HttpMethod.Get.Method).Generate();

            if (context.Request.Headers[HeaderKey.Accept] == ContentType.Xml ||
                context.Request.Headers[HeaderKey.ContentType] == ContentType.Xml)
            {
                contentResult = new ContentResult
                {
                    ContentType = ContentType.Xml,
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = XmlHelper.ToXmlStringViaJson(collectionModel)
                };
            }
            else
            {
                contentResult = new ContentResult
                {
                    ContentType = ContentType.Json,
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = JsonConvert.SerializeObject(collectionModel, Puppy.Core.Constants.StandardFormat.JsonSerializerSettings)
                };
            }

            return contentResult;
        }
    }
}