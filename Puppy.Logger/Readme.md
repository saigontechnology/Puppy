# Redis Cache Helper
> Created by **Top Nguyen** (http://topnguyen.net)

## Config
- Add config section to `appsettings.json`
- If you not have custom setting in appsettings.json, `default setting` will be apply.

```json
  "Logger": {
    "PathFormat": "Logs/LOG_{Date}.txt",
    "RetainedFileCountLimit": 365,
    "FileSizeLimitBytes": 1048576,
    "ConsoleTemplate": "{mm:ss.fff zzz} [{Level}] [{SourceContext}] [{EventId}] {Message}{NewLine}{Exception}"
  },
```

## Add Service
```csharp
// [Logger]
services.AddLogger(ConfigurationRoot)
```

## Use Application Builder
```csharp
// [Logger]
app.UseLogger(loggerFactory)
```

