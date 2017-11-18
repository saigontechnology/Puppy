using Microsoft.VisualStudio.TestTools.UnitTesting;
using Puppy.OneSignal;
using Puppy.OneSignal.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puppy.Testing
{
    [TestClass]
    public class OneSignal
    {
        [TestMethod]
        public async Task Send()
        {
            var client = new OneSignalClient("ODI4M2M4NzgtZDllNC00MTliLWJjZjMtNjZjZWM0ZmY3YjY4");

            var options = new NotificationCreateOptions
            {
                AppId = new Guid("f0afca0a-7631-4176-bbb5-0faacef3e7a6"),
                IncludedSegments = new List<string> { "All" }
            };

            options.Contents.Add(LanguageCodes.English, "Hello World");

            var result = await client.Notifications.CreateAsync(options).ConfigureAwait(true);
        }
    }
}