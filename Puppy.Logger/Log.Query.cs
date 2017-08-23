﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Puppy.Core.XmlUtils;
using Puppy.Logger.Core.Models;
using Puppy.Web;
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

        public static bool IsCanAccessLogViaUrl(HttpContext httpContext)
        {
            if (string.IsNullOrWhiteSpace(LoggerConfig.AccessKeyQueryParam))
            {
                return true;
            }

            string requestKey = httpContext.Request.Query[LoggerConfig.AccessKeyQueryParam];
            var isCanAccess = string.IsNullOrWhiteSpace(LoggerConfig.AccessKey) || LoggerConfig.AccessKey == requestKey;
            return isCanAccess;
        }

        /// <summary>
        ///     Create Content Result with response type is <see cref="PagedCollectionViewModel{LogEntity}" /> 
        /// </summary>
        /// <param name="httpContext">       </param>
        /// <param name="logEndpointPattern"></param>
        /// <param name="skip">              </param>
        /// <param name="take">              </param>
        /// <param name="terms">             </param>
        /// <returns></returns>
        /// <remarks>
        ///     Base on <paramref name="httpContext"> </paramref> will return <c> ContentType XML
        ///     </c> when Request Header Accept or ContentType is XML, else return <c> ContentType
        ///     Json </c>
        /// </remarks>
        public static ContentResult GetLogsContentResult(HttpContext httpContext, string logEndpointPattern, int skip, int take, string terms)
        {
            Expression<Func<LogEntity, bool>> predicate = null;

            if (!string.IsNullOrWhiteSpace(terms))
            {
                predicate = x => x.Message.Contains(terms);
            }

            var logs = Get(out long total, predicate: predicate, orders: x => x.CreatedTime, isOrderByDescending: true, skip: skip, take: take);

            ContentResult contentResult;

            if (total <= 0)
            {
                // Return 204 for No Data Case
                contentResult = new ContentResult
                {
                    ContentType =
                        (httpContext.Request.Headers[HeaderKey.Accept] == ContentType.Xml || httpContext.Request.Headers[HeaderKey.ContentType] == ContentType.Xml)
                        ? ContentType.Xml
                        : ContentType.Json,
                    StatusCode = (int)HttpStatusCode.NoContent,
                    Content = null
                };

                return contentResult;
            }

            var placeholderLinkView = PlaceholderLinkViewModel.ToCollection(logEndpointPattern, HttpMethod.Get.Method, new { skip, take, terms });
            var collectionFactoryViewModel = new PagedCollectionFactoryViewModel<LogEntity>(placeholderLinkView, logEndpointPattern);
            var collectionViewModel = collectionFactoryViewModel.CreateFrom(logs, skip, take, total);

            if (httpContext.Request.Headers[HeaderKey.Accept] == ContentType.Xml || httpContext.Request.Headers[HeaderKey.ContentType] == ContentType.Xml)
            {
                contentResult = new ContentResult
                {
                    ContentType = ContentType.Xml,
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = XmlHelper.ToXmlStringViaJson(collectionViewModel, "Logs")
                };
            }
            else
            {
                contentResult = new ContentResult
                {
                    ContentType = ContentType.Json,
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = JsonConvert.SerializeObject(collectionViewModel, Core.Constant.JsonSerializerSettings)
                };
            }

            return contentResult;
        }

        /// <summary>
        ///     Create Content Result with response type is <see cref="LogEntity" /> 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="id">         </param>
        /// <returns></returns>
        /// <remarks>
        ///     Base on <paramref name="httpContext"> </paramref> will return <c> ContentType XML
        ///     </c> when Request Header Accept or ContentType is XML, else return <c> ContentType
        ///     Json </c>
        /// </remarks>
        public static ContentResult GetLogContentResult(HttpContext httpContext, string id)
        {
            var log = Get(out long _, x => x.Id == id).FirstOrDefault();

            ContentResult contentResult;

            if (log == null)
            {
                // Return 204 for No Data Case
                contentResult = new ContentResult
                {
                    ContentType =
                        (httpContext.Request.Headers[HeaderKey.Accept] == ContentType.Xml || httpContext.Request.Headers[HeaderKey.ContentType] == ContentType.Xml)
                            ? ContentType.Xml
                            : ContentType.Json,
                    StatusCode = (int)HttpStatusCode.NoContent,
                    Content = null
                };

                return contentResult;
            }

            if (httpContext.Request.Headers[HeaderKey.Accept] == ContentType.Xml || httpContext.Request.Headers[HeaderKey.ContentType] == ContentType.Xml)
            {
                contentResult = new ContentResult
                {
                    ContentType = ContentType.Xml,
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = XmlHelper.ToXmlStringViaJson(log, "Log")
                };
            }
            else
            {
                contentResult = new ContentResult
                {
                    ContentType = ContentType.Json,
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = JsonConvert.SerializeObject(log, Core.Constant.JsonSerializerSettings)
                };
            }

            return contentResult;
        }
    }
}