#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DateTimeFormatMode.cs </Name>
//         <Created> 29/09/17 12:05:05 AM </Created>
//         <Key> ac9a0940-b621-44a3-bde5-baafaf7cc5c5 </Key>
//     </File>
//     <Summary>
//         DateTimeFormatMode.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.DataTable.Constants
{
    public enum DateTimeFormatMode
    {
        /// <summary>
        ///     Try parse DateTime from any string format 
        /// </summary>
        Auto,

        /// <summary>
        ///     Parse DateTime by specific/exactly format. 
        /// </summary>
        Specific
    }
}