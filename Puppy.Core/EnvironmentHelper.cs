#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EnvironmentHelper.cs </Name>
//         <Created> 17/07/17 4:59:08 PM </Created>
//         <Key> ed41b5ee-5580-4c4a-b0bb-ddf1462bb787 </Key>
//     </File>
//     <Summary>
//         EnvironmentHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.Core
{
    public static class EnvironmentHelper
    {
        public const string Development = "Development";
        public const string Staging = "Staging";
        public const string Production = "Production";
        private const string EnvironmentVariable = "ASPNETCORE_ENVIRONMENT";

        public static readonly string Name = Environment.GetEnvironmentVariable(EnvironmentVariable);

        public static readonly string MachineName = Environment.MachineName;

        public static bool IsDevelopment()
        {
            return string.Equals(Name, Development, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsProduction()
        {
            return string.Equals(Name, Production, StringComparison.OrdinalIgnoreCase);
        }
    }
}