using Microsoft.VisualStudio.TestTools.UnitTesting;
using Puppy.OneSignal;
using Puppy.OneSignal.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puppy.Testing
{
    [TestClass]
    public class OneSignalUnitTest
    {
        [TestMethod]
        public async Task SendUnitTest()
        {
            var client = new OneSignalClient("<api key>");

            var options = new NotificationCreateOptions
            {
                AppId = new Guid("<app id>"),
                IncludedSegments = new List<string> { "All" }
            };

            options.Contents.Add(LanguageCodes.English, "Hello World");

            var result = await client.Notifications.CreateAsync(options).ConfigureAwait(true);
        }
    }
}