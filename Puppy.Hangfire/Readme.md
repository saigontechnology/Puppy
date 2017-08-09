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
services.AddHangfire(SystemConfigs.DatabaseConnectionString, ConfigurationRoot)
```

- Use in Application Builder 
```csharp
app.UseHangfire();
```

- How to Use: Inject `IRedisCacheManager` (refer) or `IDistributedCache` and call methods.
