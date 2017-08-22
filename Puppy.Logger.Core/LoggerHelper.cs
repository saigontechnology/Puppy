#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LoggerHelper.cs </Name>
//         <Created> 22/08/17 11:50:05 PM </Created>
//         <Key> a77b08f0-b591-413e-a4ac-b82761ccef9b </Key>
//     </File>
//     <Summary>
//         LoggerHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Puppy.Logger.Core.Models;
using System;

namespace Puppy.Logger.Core
{
    public class LoggerHelper
    {
        public static bool TryParseLogInfo(string value, out LogInfo logInfo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    logInfo = null;
                    return false;
                }
                logInfo = JsonConvert.DeserializeObject<LogInfo>(value, Constant.JsonSerializerSettings);
                return logInfo != null;
            }
            catch (Exception)
            {
                logInfo = null;
                return false;
            }
        }
    }
}