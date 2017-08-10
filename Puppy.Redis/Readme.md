# Redis Cache Helper
> Created by **Top Nguyen** (http://topnguyen.net)

## Config
- Add config section to `appsettings.json`
- If you not have custom setting in appsettings.json, `default setting` will be apply.

```json
"Redis": {
"ConnectionString": "localhost:<redis server port>", // default port is 6379
"InstanceName": "Puppy"
}
```

## Add Service
```csharp
services.AddRedisCache(ConfigurationRoot)
```
- How to Use: Inject `IRedisCacheManager` (refer) or `IDistributedCache` and call methods.

## Check Redis Setup on Application Start

At the End of Application Builder in `Startup.cs` call the method below to verify the `IRedisCacheManager` is work well.
```csharp
// Verify Redis Setting is Fine
IRedisCacheManager redisCacheManager = app.Resolve<IRedisCacheManager>();
redisCacheManager.VerifySetup();
```