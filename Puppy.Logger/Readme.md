# Puppy.Logger
> Created by **Top Nguyen** (http://topnguyen.net)

## Config
- Add config section to `appsettings.json`
- If you not have custom setting in appsettings.json, `default setting` will be apply.

```json
  "Logger": {
    "PathFormat": "Logs/LOG_{Date}.txt",
    "RetainedFileCountLimit": 365,
    "FileSizeLimitBytes": 1048576,
    "FileLogMinimumLevel": "Warning", // Verbose, Debug, Information, Warning, Error, Fatal.
    "ConsoleLogMinimumLevel": "Warning" // Verbose, Debug, Information, Warning, Error, Fatal.
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

## Enhance Logger
- Puppy Logger use `Serilog` to implement Logger Client.
- Please refer `Serilog` official document to enhance config.
  + [Wiki](https://github.com/serilog/serilog/wiki)
  + [Output Template](https://github.com/serilog/serilog/wiki/Formatting-Output)