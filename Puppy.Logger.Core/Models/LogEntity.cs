#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LogEntity.cs </Name>
//         <Created> 10/08/17 11:20:21 PM </Created>
//         <Key> 141ffe67-ca45-4c4c-8234-c97f287b66aa </Key>
//     </File>
//     <Summary>
//         LogEntity.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;

namespace Puppy.Logger.Core.Models
{
    [Serializable]
    [DesignerCategory(nameof(Puppy))]
    public sealed class LogEntity : Serializable
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string CallerMemberName { get; set; }

        public string CallerFilePath { get; set; }

        public string CallerRelativePath { get; set; }

        private int? _callerLineMember;

        public int? CallerLineNumber
        {
            get => _callerLineMember;
            set
            {
                if (value == null || value <= 0)
                {
                    _callerLineMember = null;
                }
                else
                {
                    _callerLineMember = value;
                }
            }
        }

        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.Now;

        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel Level { get; set; } = LogLevel.Error;

        public string Message { get; set; }

        #region Exception

        private ExceptionInfo _exception;

        /// <summary>
        ///     Exception Info 
        /// </summary>
        /// <remarks> Set value for Exception auto update value for <see cref="ExceptionJson" /> </remarks>
        public ExceptionInfo Exception
        {
            get => _exception;
            set
            {
                _exception = value;
                ExceptionJson = _exception.ToString();
            }
        }

        private string _exceptionJson;

        /// <summary>
        ///     Json String of <see cref="Exception" /> 
        /// </summary>
        /// <remarks>
        ///     When set value for this, only accept json string can parse to <c> ExceptionInfo </c>
        /// </remarks>
        [JsonIgnore]
        public string ExceptionJson
        {
            get => _exceptionJson;
            set
            {
                var exceptionEntity = JsonConvert.DeserializeObject<ExceptionInfo>(value, Constant.JsonSerializerSettings);
                if (exceptionEntity == null) throw new NotSupportedException($"{value} is not {nameof(ExceptionInfo)} Json String");
                _exceptionJson = value;
            }
        }

        #endregion

        #region HttpContext

        private HttpContextInfo _httpContext;

        /// <summary>
        ///     Http Context Info 
        /// </summary>
        /// <remarks> Set value for HttpContext auto update value for <see cref="HttpContextJson" /> </remarks>
        public HttpContextInfo HttpContext
        {
            get => _httpContext;
            set
            {
                _httpContext = value;
                HttpContextJson = _httpContext.ToString();
            }
        }

        private string _httpContextJson;

        /// <summary>
        ///     Json String of <see cref="HttpContext" /> 
        /// </summary>
        /// <remarks>
        ///     When set value for this, only accept json string can parse to <c> HttpContextInfo </c>
        /// </remarks>
        [JsonIgnore]
        public string HttpContextJson
        {
            get => _httpContextJson;
            set
            {
                var httpContext = JsonConvert.DeserializeObject<HttpContextInfo>(value, Constant.JsonSerializerSettings);

                if (httpContext == null) throw new NotSupportedException($"{value} is not {nameof(HttpContextInfo)} Json String");

                _httpContextJson = value;
            }
        }

        #endregion

        public LogEntity()
        {
        }

        public LogEntity(string message, LogLevel level) : this()
        {
            Message = message;
            Level = level;
        }

        public LogEntity(Exception ex, LogLevel level, string message = null) : this(message, level)
        {
            Exception = new ExceptionInfo(ex);

            if (string.IsNullOrWhiteSpace(Message))
            {
                Message = Exception.RootExceptionMessage;
            }
        }

        public LogEntity(ExceptionContext context, LogLevel level, string message = null) : this(message, level)
        {
            Exception = new ExceptionInfo(context.Exception);
            HttpContext = new HttpContextInfo(context.HttpContext);

            if (string.IsNullOrWhiteSpace(Message))
            {
                Message = Exception.RootExceptionMessage;
            }
        }

        /// <summary>
        ///     Fill data for HttpContext from HttpContextJson and Exception from ExceptionJson 
        /// </summary>
        /// <returns></returns>
        public LogEntity FillInfo()
        {
            if (!string.IsNullOrWhiteSpace(ExceptionJson))
            {
                Exception = JsonConvert.DeserializeObject<ExceptionInfo>(ExceptionJson, Constant.JsonSerializerSettings);
            }

            if (!string.IsNullOrWhiteSpace(HttpContextJson))
            {
                HttpContext = JsonConvert.DeserializeObject<HttpContextInfo>(HttpContextJson, Constant.JsonSerializerSettings);
            }

            return this;
        }
    }
}