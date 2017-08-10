#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LoggerFormatter.cs </Name>
//         <Created> 10/08/17 6:40:02 PM </Created>
//         <Key> 3fd9e5d6-a135-4970-911e-e528f7a4b885 </Key>
//     </File>
//     <Summary>
//         LoggerFormatter.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;

namespace Puppy.Logger.Core
{
    /// <summary>
    ///     Logger Formatter for Serilog, Write only Message Template as Message (Message is JSON String)
    /// </summary>
    /// <remarks> Auto add <c> ,Environment.NewLine </c> to the end of message </remarks>
    public class LoggerFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            // Write Message Template as Json
            output.Write(logEvent.MessageTemplate);
            output.Write($",{Environment.NewLine}");
        }
    }
}