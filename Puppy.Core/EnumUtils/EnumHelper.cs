#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EnumHelper.cs </Name>
//         <Created> 12/09/17 5:09:19 PM </Created>
//         <Key> dd553a3a-a1b6-4013-b524-8168099531cf </Key>
//     </File>
//     <Summary>
//         EnumHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.Core.EnumUtils
{
    public static class EnumHelper
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}