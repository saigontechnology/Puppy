#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Constants.cs </Name>
//         <Created> 10/08/17 10:57:36 AM </Created>
//         <Key> 03615d25-3125-478e-93c1-b6fa83bedf76 </Key>
//     </File>
//     <Summary>
//         Constants.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Serilog.Events;

namespace Puppy.Logger
{
    public static class Constant
    {
        public const string DefaultConfigSection = "Logger";

        /// <summary>
        ///     <para> Console Template to write log </para>
        ///     <para>
        ///         Default is <c> [{Timestamp:mm:ss.fff} {Level:u3}] {Message}{NewLine} </c>
        ///     </para>
        /// </summary>
        /// <remarks> Console only enable in <c> Development Environment </c> </remarks>
        public const string ConsoleTemplate = "[{Timestamp:mm:ss.fff} {Level:u3}] {Message}{NewLine}";

        public static string[] LogEventLevels = {
            LogEventLevel.Verbose.ToString(),
            LogEventLevel.Debug.ToString(),
            LogEventLevel.Information.ToString(),
            LogEventLevel.Warning.ToString(),
            LogEventLevel.Error.ToString(),
            LogEventLevel.Fatal.ToString()
        };
    }
}