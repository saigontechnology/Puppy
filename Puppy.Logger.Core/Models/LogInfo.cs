#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LogInfo.cs </Name>
//         <Created> 10/08/17 11:20:21 PM </Created>
//         <Key> 141ffe67-ca45-4c4c-8234-c97f287b66aa </Key>
//     </File>
//     <Summary>
//         LogInfo.cs
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
    public class LogInfo : Serializable
    {
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

        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public DateTimeOffset CreatedOn { get; } = DateTimeOffset.Now;

        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel Level { get; set; } = LogLevel.Error;

        public string Message { get; set; }

        public ExceptionInfo ExceptionInfo { get; set; }

        public HttpContextInfo HttpContextInfo { get; set; }

        public LogInfo()
        {
        }

        public LogInfo(string message, LogLevel level) : this()
        {
            Message = message;
            Level = level;
        }

        public LogInfo(Exception ex, LogLevel level, string message = null) : this(message, level)
        {
            ExceptionInfo = new ExceptionInfo(ex);
        }

        public LogInfo(ExceptionContext context, LogLevel level, string message = null) : this(message, level)
        {
            ExceptionInfo = new ExceptionInfo(context.Exception);
            HttpContextInfo = new HttpContextInfo(context.HttpContext);
        }
    }
}