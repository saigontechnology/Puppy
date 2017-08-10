#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LoggerConfig.cs </Name>
//         <Created> 10/08/17 12:31:28 PM </Created>
//         <Key> 645e0f00-7753-42d7-8f8d-8e2029b3e5ca </Key>
//     </File>
//     <Summary>
//         LoggerConfig.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Logger
{
    /// <summary>
    ///     Logger Configuration 
    /// </summary>
    /// <remarks>
    ///     <c> Not allow config format/template </c> of rolling file because it must use Puppy
    ///     Logger format for <c> analyze purpose </c>.
    /// </remarks>
    public static class LoggerConfig
    {
        /// <summary>
        ///     Path format to write log, default is <c> Logs/LOG_{Date}.txt </c> 
        /// </summary>
        public static string PathFormat { get; set; } = "Logs/LOG_{Date}.txt";

        /// <summary>
        ///     Maximum retained file, default is <c> 365 </c> 
        /// </summary>
        public static int RetainedFileCountLimit { get; set; } = 365;

        /// <summary>
        ///     Maximum file size, default is <c> 1048576 (bytes) ~ 1 GB </c> 
        /// </summary>
        public static long? FileSizeLimitBytes { get; set; } = 1048576;

        /// <summary>
        ///     <para> Console Template to write log </para>
        ///     <para>
        ///         Default is <c> {mm:ss.fff zzz} [{Level}] [{SourceContext}] [{EventId}]
        ///         {Message}{NewLine}{Exception} </c>
        ///     </para>
        /// </summary>
        /// <remarks> Console only enable in <c> Development Environment </c> </remarks>
        public static string ConsoleTemplate { get; set; } = "{mm:ss.fff zzz} [{Level}] [{SourceContext}] [{EventId}] {Message}{NewLine}{Exception}";
    }
}