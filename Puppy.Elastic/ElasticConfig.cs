#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ElasticConfigs.cs </Name>
//         <Created> 17/10/17 8:08:39 PM </Created>
//         <Key> f9a22a80-ba30-4209-af5d-c48124f96f4d </Key>
//     </File>
//     <Summary>
//         ElasticConfigs.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Elastic
{
    public static class ElasticConfig
    {
        /// <summary>
        ///     Elastic connection string. Default is "http://127.0.0.1:9200" 
        /// </summary>
        public static string ConnectionString { get; set; } = "http://127.0.0.1:9200";
    }
}