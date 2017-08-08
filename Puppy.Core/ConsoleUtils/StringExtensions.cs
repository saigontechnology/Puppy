#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> StringExtensions.cs </Name>
//         <Created> 08/08/17 9:56:01 AM </Created>
//         <Key> f4978b2e-9b79-4b91-ab2a-cb028f270bcf </Key>
//     </File>
//     <Summary>
//         StringExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Core.ConsoleUtils
{
    public static class StringExtensions
    {
        public static string ConsoleNormalize(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            value = value.Replace('{', '<').Replace('}', '>');

            return value;
        }
    }
}