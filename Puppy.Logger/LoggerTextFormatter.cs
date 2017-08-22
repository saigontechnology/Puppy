#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LoggerTextFormatter.cs </Name>
//         <Created> 10/08/17 6:40:02 PM </Created>
//         <Key> 3fd9e5d6-a135-4970-911e-e528f7a4b885 </Key>
//     </File>
//     <Summary>
//         LoggerTextFormatter.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Puppy.Logger.Core.Models;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.IO;

namespace Puppy.Logger
{
    /// <summary>
    ///     Logger Formatter for Serilog, Write only Message is LoggerException JSON String 
    /// </summary>
    /// <remarks> Auto add <c> ,Environment.NewLine </c> to the end of message </remarks>
    public class LoggerTextFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (string.IsNullOrWhiteSpace(logEvent.MessageTemplate?.Text))
            {
                return;
            }

            if (!IsCanLog(logEvent.MessageTemplate.Text)) return;
            output.Write(logEvent.MessageTemplate.Text);
            output.Write($",{Environment.NewLine}");
        }

        private static bool IsCanLog(string message)
        {
            try
            {
                // Check if message is LoggerException JSON string.
                var loggerException = JsonConvert.DeserializeObject<LoggerException>(message, Core.Constant.JsonSerializerSettings);
                return loggerException != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}