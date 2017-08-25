# Redis Cache Helper
> Created by **Top Nguyen** (http://topnguyen.net)

- use appsettings.json to config redis connection and instance name
```json
  "Hangfire": {
    // Dashboard URL, Set empty to disable dashboard feature
    // Start with "/" but end with "<empty>", default is "/developers/job"
    "DashboardUrl": "/developers/job",

    // Access Key Query Param: name of parameter to check can access api document or not
    // Default is "key"
    "AccessKeyQueryParam": "key",

    // Access Key: access key to check with AccessKeyQueryParam - empty is allow annonymous
    // Default is empty
    "AccessKey": "",

    // The path for the Back To Site link. Set to <see langword="null" /> in order to hide the Back To Site link. Default is "/"
    "BackToSiteUrl": "/developers",

    // The interval the /stats endpoint should be polled with (milliseconds). Default is 2000
    "StatsPollingInterval": 2000
  },
```

- Add Service
```csharp
// [Background Job] Store Job in Memory. Add param
// databaseConnectionString to store job in Sql Server
.AddHangfire(ConfigurationRoot)
```

- Use in Application Builder 
```csharp
// [Background Job] Hangfire
.UseHangfire()
```

- How to Use: Inject `IRedisCacheManager` (refer) or `IDistributedCache` and call methods.
