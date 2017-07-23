#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> OneSignalTesting.cs </Name>
//         <Created> 30/05/2017 5:23:04 PM </Created>
//         <Key> e7204ea9-5b98-4176-b06a-a20230f5473d </Key>
//     </File>
//     <Summary>
//         OneSignalTesting.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Puppy.OneSignal;
using Puppy.OneSignal.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puppy.Testing
{
    [TestClass]
    public class OneSignalTesting
    {
        private OneSignalClient _client;
        private NotificationCreateOptions _options;

        [TestInitialize]
        public void Initial()
        {
            _client = new OneSignalClient("<api key>");
            _options = new NotificationCreateOptions
            {
                AppId = new Guid("<app id>")
            };
        }

        [TestMethod]
        public async Task Create()
        {
            _options.IncludedSegments = new List<string> { "All" };
            _options.Contents.Add(LanguageCodes.English, "Hello World");
            var result = await _client.Notifications.CreateAsync(_options).ConfigureAwait(true);
        }
    }
}