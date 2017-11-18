![Logo](logo%20onesignal.png)
# Puppy.OneSignal
> Project Created by **Top Nguyen** (http://topnguyen.net)
- A General purpose rest ApiClient write in C# language for the OneSignal API
- Project focus on wrapping 3rd Party service
[OneSignal Server API](https://documentation.onesignal.com/reference)

# How to use
```csharp
var client = new OneSignalClient("<api key>");

var options = new NotificationCreateOptions
{
    AppId = new Guid("<app id>"),
    IncludedSegments = new List<string> {"All"}
};


options.Contents.Add(LanguageCodes.English, "Hello World");

var result = await client.Notifications.CreateAsync(options).ConfigureAwait(true);
```
