# Redis Cache Helper
> Created by **Top Nguyen** (http://topnguyen.net)

- use appsettings.json to config redis connection and instance name
```json
  "Hangfire": {
    // Dashboard URL, Set empty to disable dashboard feature
    "DashboardUrl": "/developers/job",

    // Access Key Query Param: name of parameter to check can access api document or not
    "AccessKeyQueryParam": "key",

    // Access Key: access key to check with AccessKeyQueryParam - empty is allow annonymous
    "AccessKey": ""
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
