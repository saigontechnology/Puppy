#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DataTableConfig.cs </Name>
//         <Created> 28/09/17 11:34:11 PM </Created>
//         <Key> 4de8f04a-11dd-456a-b430-6f235934995d </Key>
//     </File>
//     <Summary>
//         DataTableConfig.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Puppy.DataTable.Constants;

namespace Puppy.DataTable
{
    /// <summary>
    ///     [Auto Reload] DataTable Global Config 
    /// </summary>
    public static class DataTableGlobalConfig
    {
        /// <summary>
        ///     Config use datetime with TimeZone. Default is "UTC", See more: https://msdn.microsoft.com/en-us/library/gg154758.aspx 
        /// </summary>
        public static string DateTimeTimeZone { get; set; } = "UTC";

        public static string DateFormat { get; set; } = "dd/MM/yyyy";

        /// <summary>
        ///     All response will apply the format by default. If
        ///     <see cref="RequestDateTimeFormatMode" /> is
        ///     <see cref="DateTimeFormatMode.Specific" />, every request will use the format to
        ///     parse string to DateTime. Else will try parse string to DateTime by any format.
        /// </summary>
        /// <remarks> Format "dd/MM/yyyy hh:mm tt" by default </remarks>
        public static string DateTimeFormat { get; set; } = "dd/MM/yyyy hh:mm:ss tt";

        /// <summary>
        ///     Control the way to parse string to DateTime every request. If value is
        ///     <see cref="DateTimeFormatMode.Specific" />, every request will use the
        ///     <see cref="DateTimeFormat" /> to parse string to DateTime. Else, will try parse
        ///     string to DateTime by any format.
        /// </summary>
        /// <remarks> Value is "Auto" by default </remarks>
        [JsonConverter(typeof(StringEnumConverter))]
        public static DateTimeFormatMode RequestDateTimeFormatMode { get; set; } = DateTimeFormatMode.Auto;
    }
}