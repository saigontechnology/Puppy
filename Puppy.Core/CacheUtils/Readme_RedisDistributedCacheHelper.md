# Redis Cache Helper
> Created by **Top Nguyen** (http://topnguyen.net)

- use appsettings.json to config default cache duration
```json
  "Redis": {
    "ConnectionString": "localhost:<redis server port>",
    "InstanceName": "Eatup.WebAPI"
  }

```
- In Startup.cs
```csharp
	string redisConnection = Configuration["Redis:ConnectionString"];
	string redisInstance = Configuration["Redis:InstanceName"];

	services.AddSingleton<IDistributedCache>(factory =>
	{
		var cache = new RedisCache(new RedisCacheOptions
		{
			Configuration = redisConnection,
			InstanceName = redisInstance
		});

		return cache;
	});
```