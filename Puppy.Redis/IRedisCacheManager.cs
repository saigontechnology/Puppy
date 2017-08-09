#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IRedisCacheManager.cs </Name>
//         <Created> 17/07/17 4:38:34 PM </Created>
//         <Key> 8d005ff7-9257-484d-aaf9-2e94c1ca9ee0 </Key>
//     </File>
//     <Summary>
//         IRedisCacheManager.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.Extensions.Caching.Distributed;
using System;

namespace Puppy.Redis
{
    public interface IRedisCacheManager
    {
        bool IsExist(string key);

        T Get<T>(string key) where T : class;

        /// <summary>
        ///     Absolute Expiration by duration 
        /// </summary>
        /// <param name="key">     </param>
        /// <param name="data">    </param>
        /// <param name="duration"></param>
        void Set(string key, object data, TimeSpan duration);

        void Set(string key, object data, DistributedCacheEntryOptions options);

        void Remove(string key);
    }
}