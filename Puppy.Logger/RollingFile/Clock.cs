#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Clock.cs </Name>
//         <Created> 17/08/17 10:22:24 PM </Created>
//         <Key> a297ceae-b35d-4b96-89e2-914f63fb7aa7 </Key>
//     </File>
//     <Summary>
//         Clock.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.Logger.RollingFile
{
    internal static class Clock
    {
        public static readonly Func<DateTime> DateTimeNowFunc = () => DateTime.Now;

        public static DateTime DateTimeNow => DateTimeNowFunc();
    }
}