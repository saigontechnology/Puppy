![OneSignal Logo](https://onesignal.com/assets/common/logo_onesignal_color.png)
# Puppy.OneSignal
A General purpose rest ApiClient write in C# language for the OneSignal API

# Important Note
> Project Created by **Top Nguyen** (http://topnguyen.net)

## OneSignal Api Oficial Documentation
Project focus on wrapping 3rd Party service
[OneSignal Server API] (https://documentation.onesignal.com/reference)

# How to use
```csharp
var client = new OneSignalClient("ZWM3YThlMmQtMzY1NC00ODI1LTlkYjMtMTk3MzI2OTQzMjVh");

var options = new NotificationCreateOptions();

options.AppId = "089e4845-9849-4157-8b29-71e174af3abf";

options.IncludedSegments = new List<string> { "All" };

options.Contents.Add(LanguageCodes.English, "Hello world!");

client.Notifications.Create(options);
```
