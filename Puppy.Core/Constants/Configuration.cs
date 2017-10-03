#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Configuration.cs </Name>
//         <Created> 03/10/17 5:50:42 PM </Created>
//         <Key> 09cbbf60-fce7-4361-96fe-a13bb74c8379 </Key>
//     </File>
//     <Summary>
//         Configuration.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Core.Constants
{
    public static class Configuration
    {
        // Config File Name
        public const string AppSettingsJsonFileName = "appsettings.json";

        public const string BundleConfigJsonFileName = "bundleconfig.json";
        public const string CompilerConfigJsonFileName = "compilerconfig.json";

        // Config Section
        public const string ConnectionStringsConfigSection = "ConnectionStrings";
    }
}