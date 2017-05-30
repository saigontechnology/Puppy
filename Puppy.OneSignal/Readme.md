# Important Note
> Project Created by **Top Nguyen** (http://topnguyen.net)

Project focus on wrapping 3rd Party service: https://documentation.onesignal.com/reference

# How to use
```csharp
var client = new OneSignalClient("ZWM3YThlMmQtMzY1NC00ODI1LTlkYjMtMTk3MzI2OTQzMjVh");

var options = new NotificationCreateOptions();

options.AppId = "089e4845-9849-4157-8b29-71e174af3abf";

options.IncludedSegments = new List<string> { "All" };

options.Contents.Add(LanguageCodes.English, "Hello world!");

client.Notifications.Create(options);
```
# OneSignal Api Oficial Documentation
[OneSignal Server API] (https://documentation.onesignal.com/reference)