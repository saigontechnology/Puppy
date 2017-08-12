# Puppy.Logger
> Created by **Top Nguyen** (http://topnguyen.net)

Puppy Logger, log error with http request information and exception as detail as possible.

## Config
- Add config section to `appsettings.json`
- If you not have custom setting in appsettings.json, `default setting` will be apply.

```json
  "Logger": {
    "PathFormat": "Logs\\LOG_{Date}.txt",
    "RetainedFileCountLimit": 365,
    "FileSizeLimitBytes": 1048576,
    "FileLogMinimumLevel": "Error", // Verbose, Debug, Information, Warning, Error, Fatal.
    "ConsoleLogMinimumLevel": "Information" // Verbose, Debug, Information, Warning, Error, Fatal.
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

## Logging

- Call static method in class `Puppy.Logger.Log` to write log with `Log Level`.
- This below is sample to write logs.

```csharp
Puppy.Logger.Log.Verbose("<message>");
Puppy.Logger.Log.Debug("<message>");
Puppy.Logger.Log.Information("<message>");
Puppy.Logger.Log.Warning("<message>");
Puppy.Logger.Log.Error("<message>");
Puppy.Logger.Log.Fatal("<message>");

// Use Write with specific LogLevel
Puppy.Logger.Log.Write(LogLevel.Verbose, "<message>");
Puppy.Logger.Log.Write(LogLevel.Debug, "<message>");
Puppy.Logger.Log.Write(LogLevel.Information, "<message>");
Puppy.Logger.Log.Write(LogLevel.Warning, "<message>");
Puppy.Logger.Log.Write(LogLevel.Error, "<message>");
Puppy.Logger.Log.Write(LogLevel.Fatal, "<message>");
```

## Exception Global Log with Logger
If you want to global log with logger, just `throw exception` and not use any specific `try-catch` block in your code. Then add `Exception Filter` as below to handle exception and Log when exception occur.

```csharp
public class LoggerExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        // Your other logic before log...

        // Log with Logger
        string logId = Log.Error(context);

        // Your other logic after Log...

        // Keep base Exception
        base.OnException(context);
    }
}
```

## Enhance Logger
- Puppy Logger use `Serilog` to implement Logger Client.
- Please refer `Serilog` official document to enhance config.
  + [Wiki](https://github.com/serilog/serilog/wiki)
  + [Output Template](https://github.com/serilog/serilog/wiki/Formatting-Output)

## Sample Log
```json
{
  "CallerMemberName": "OnException",
  "CallerFilePath": "D:\\Dropbox\\Github\\Monkey\\Monkey\\Monkey\\Filters\\ApiExceptionFilter.cs",
  "CallerRelativePath": "Monkey\\Filters\\ApiExceptionFilter.cs",
  "CallerLineNumber": 46,
  "Id": "26b9716decbe4a819c539f1ffd3a6871",
  "CreatedOn": "2017-08-11T19:20:58.9626676+07:00",
  "Level": "Error",
  "Exception": {
    "Message": "Input string was not in a correct format.",
    "Source": "System.Private.CoreLib",
    "StackTrace": "   at System.Number.StringToNumber(String str, NumberStyles options, NumberBuffer& number, NumberFormatInfo info, Boolean parseDecimal)\r\n   at System.Number.ParseInt32(String s, NumberStyles style, NumberFormatInfo info)\r\n   at Monkey.Controllers.Api.TestApiController.Post(String data, TestData testData) in D:\\Dropbox\\Github\\Monkey\\Monkey\\Monkey\\Controllers\\Api\\TestApiController.cs:line 19\r\n   at lambda_method(Closure , Object , Object[] )\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.<InvokeActionMethodAsync>d__27.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()\r\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.<InvokeNextActionFilterAsync>d__25.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Rethrow(ActionExecutedContext context)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.<InvokeNextExceptionFilterAsync>d__24.MoveNext()",
    "TypeName": "System.FormatException",
    "BaseTypeName": "System.FormatException"
  },
  "HttpContextInfo": {
    "RequestTime": "2017-08-11T07:20:56.738+07:00",
    "Headers": {
      "Connection": [
        "keep-alive"
      ],
      "Content-Length": [
        "28"
      ],
      "Content-Type": [
        "application/json"
      ],
      "Accept": [
        "application/json"
      ],
      "Accept-Encoding": [
        "gzip, deflate, br"
      ],
      "Accept-Language": [
        "en-US,en;q=0.8"
      ],
      "Cookie": [
        ".AspNetCore.Antiforgery.5Vkc_VEHWDw=CfDJ8G1e7Qna5WlPnbPKLeQdxZBbyaoWo6yWb_fIy9BB9V_kj_H6rrgFFnrSh982Mosq6XbNEKBjOg1SMoi_7Glc9axfaF5J79oBONAXGqdtxK5TPUWabiSq5caI6LLAGxTdrzkguTXvtFWbe7Y4gBKNUyM; driftt_aid=fe218537-7b71-4f2f-aec7-270df2bc46d0; __atuvc=4%7C32"
      ],
      "Host": [
        "localhost:7777"
      ],
      "Referer": [
        "http://localhost:7777/developers/"
      ],
      "User-Agent": [
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.90 Safari/537.36"
      ],
      "Origin": [
        "http://localhost:7777"
      ],
      "Id": [
        "26b9716decbe4a819c539f1ffd3a6871"
      ],
      "RequestTime": [
        "2017/08/11 07:20:56.738 +07:00"
      ]
    },
    "Protocol": "HTTP/1.1",
    "Method": "POST",
    "Endpoint": "http://localhost:7777/api/test",
    "QueryStrings": {
      "data": [
        "puppy logger"
      ]
    },
    "DisplayUrl": "[HTTP/1.1](POST)http://localhost:7777/api/test?data=puppy%20logger",
    "RequestBody": {
      "data": "Puppy Logger"
    }
  }
},

```