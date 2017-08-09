# Redis Cache Helper
> Created by **Top Nguyen** (http://topnguyen.net)

- use appsettings.json to config redis connection and instance name
```json
"Redis": {
"ConnectionString": "localhost:<redis server port>", // default port is 6379
"InstanceName": "Puppy"
}
```

- In Startup.cs add Service
```csharp
services.AddRedisCache(ConfigurationRoot)
```

- How to Use: Inject `IRedisCacheManager` (refer) or `IDistributedCache` and call methods.
