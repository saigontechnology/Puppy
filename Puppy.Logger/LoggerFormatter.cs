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

using Serilog.Events;
using Serilog.Formatting;
using System;
using System.IO;

namespace Puppy.Logger
{
    /// <summary>
    ///     Logger Formatter for Serilog, Write only Message Template as Message 
    /// </summary>
    public class LoggerFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            // logEvent.Properties
            if (logEvent == null)
                throw new ArgumentNullException("logEvent");
            if (output == null)
                throw new ArgumentNullException("output");

            output.Write(logEvent.MessageTemplate);
            output.Write(Environment.NewLine);
        }
    }
}