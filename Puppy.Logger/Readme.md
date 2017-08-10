# Redis Cache Helper
> Created by **Top Nguyen** (http://topnguyen.net)

- use appsettings.json to config redis connection and instance name
```json
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "Using": [ "Serilog.Sinks.RollingFile", "Serilog.Sinks.ColoredConsole" ],
    "Enrich": [ "WithMachineName", "WithThreadId", "WithProcessId" ],
    "WriteTo": [
      {
        "Name": "RollingFile",
        "Args": {
          "PathFormat": "Logs/Monkey_LOG_{Date}.txt",
          "RetainedFileCountLimit": 365,
          "FileSizeLimitBytes": 1048576,
          "Formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog",
          "OutputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] [{EventId}] {Message}{NewLine}{Exception}"
        }
      },
      {
        "Name": "ColoredConsole",
        "Args": {
          "OutputTemplate": "{mm:ss.fff zzz} [{Level}] [{SourceContext}] [{EventId}] {Message}{NewLine}{Exception}"
        }
      }
    ]
  },
```

- Use application builder
```csharp
services.AddRedisCache(ConfigurationRoot)
```