# Puppy.Logger
> Created by **Top Nguyen** (http://topnguyen.net)

- Puppy Logger, log error with HTTP request information and exception as detail as possible and structure by `LogEntity` object.

- Puppy Logger log into 3 places: **Sqlite** (Force), **Rolling File** (Optional) and **Console** with Color (Only in Development Environment).

- Logger write Log with `message queue` so when create a log, it **near real-time log**.

## Config
- Add config section to `appsettings.json`
- If you do not have custom setting in `appsettings.json`, **default setting will be apply**.
- Puppy Logger auto add a watcher to `appsettings.json`, when you change the config in `appsettings.json` new logger config applies immediately.

```json
  "Logger": {
    //------------------ Rolling File ------------------
    // Default Puppy Logger always log in SQLite file and also in Rolling File with config, so you can enable or disable rolling file option
    "IsEnableRollingFileLog": true,
    
    // PathFormat
    // {Date} Creates a file per day. Filenames use the DateFormat format.
    // {Hour} Creates a file per hour. Filenames use the yyyy-MM-dd HH format.
    // {HalfHour} Creates a file per half hour. Filenames use the yyyy-MM-dd HH_mm format.
    // {Level} use run time level when you call Write Log method: Verbose, Debug, Information, Warning, Error, Fatal
    "PathFormat": "Logs\\{Level}\\LOG_{Level}_{Date}.json",
    
    // Format Date Time config for PathFormat
    // {Date} date format, default is "yyyy-MM-dd"
    "DateFormat": "yyyy-MM-dd",
    // {Hour} time format, default is "yyyy-MM-dd HH"
    "HourFormat": "yyyy-MM-dd HH",
    // {HalfHour} time format, default is "yyyy-MM-dd HH_mm"
    "HalfHourFormat": "yyyy-MM-dd HH_mm",

    "RetainedFileCountLimit": 365,
    "FileSizeLimitBytes": 1048576,
    "FileLogMinimumLevel": "Warning", // Verbose, Debug, Information, Warning, Error, Fatal.
    
    //------------------ Console ------------------

    // Puppy Logger do log in Console only in Development Environment
    "ConsoleLogMinimumLevel": "Information" // Verbose, Debug, Information, Warning, Error, Fatal.
    
    //------------------ Database ------------------
    "SQLiteConnectionString": "Logs\\Puppy.Logger.db",
    "SQLiteLogMinimumLevel": "Warning" // Verbose, Debug, Information, Warning, Error, Fatal.
    
    // Access Key read from URI, empty is allow anonymous, default is empty.
    "AccessKey" : "",

    // Query parameter via http request, empty is allow anonymous, default is "key"
    "AccessKeyQueryParam": "key"
  }
```

## Add Service
```csharp
// [Logger]
services.AddLogger(ConfigurationRoot)
```

## Use Application Builder
- In Configure of Application Builder, you need inject `IApplicationLifetime appLifetime` to use Puppy Logger
```csharp
// [Logger]
app.UseLogger(loggerFactory, appLifetime)
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
public class LogInfoFilter : ExceptionFilterAttribute
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

## Get Log and View Log Via URL
- Get Log: Use `Log.Where()` and `Log.Get()` methods to get log data.
```csharp
var logs = Log.Get(out long total, predicate: predicate, orders: x => x.CreatedTime, isOrderByDescending: true, skip: skip, take: take);
```

- View Log Via URL: The sample below use Puppy.Web.Models.Api to create http collection response.

```csharp
#region Log

public const string LogsEndpointPattern = "logs/{skip:int}/{take:int}";

/// <summary>
///     Logs 
/// </summary>
/// <param name="skip"> </param>
/// <param name="take"> </param>
/// <param name="terms">
///     Search for `Id`, `Message`, `Level`, `CreatedTime` (with format
///     **"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"**, ex: "2017-08-24T00:56:29.6271125+07:00")
/// </param>
/// <returns></returns>
/// <remarks>
///     <para>
///         Logger write Log with `message queue` so when create a log, it **near real-time log**
///     </para>
/// </remarks>
[ServiceFilter(typeof(ViewLogViaUrlAccessFilter))]
[HttpGet]
[Route(LogsEndpointPattern)]
[Produces(ContentType.Json, ContentType.Xml)]
[SwaggerResponse((int)HttpStatusCode.OK, typeof(ICollection<LogEntity>))]
public IActionResult Logs([FromRoute] int skip, [FromRoute] int take, [FromQuery] string terms) => Log.GetLogsContentResult(HttpContext, LogsEndpointPattern, skip, take, terms);

/// <summary>
///     Log 
/// </summary>
/// <param name="id"> Id should be a `guid string` with format [**"N"**](https://goo.gl/pYVXKd) </param>
/// <returns></returns>
/// <remarks>
///     <para>
///         Logger write Log with `message queue` so when create a log, it **near real-time log**
///     </para>
/// </remarks>
[ServiceFilter(typeof(ViewLogViaUrlAccessFilter))]
[HttpGet]
[Route("logs/{id}")]
[Produces(ContentType.Json, ContentType.Xml)]
[SwaggerResponse((int)HttpStatusCode.OK, typeof(LogEntity))]
public IActionResult SingleLog([FromRoute]string id) => Log.GetLogContentResult(HttpContext, id);

#endregion Log
```

- OR manual implement with Log.Get()
```csharp
public const string LogsEndpointPattern = "logs/{skip:int}/{take:int}";

/// <summary>
///     Logs 
/// </summary>
/// <param name="skip"> </param>
/// <param name="take"> </param>
/// <param name="terms">
///     Search for `Id`, `Message`, `Level`, `CreatedTime` (with format
///     **"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"**, ex: "2017-08-24T00:56:29.6271125+07:00")
/// </param>
/// <returns></returns>
/// <remarks>
///     <para>
///         Logger write Log with `message queue` so when create a log, it **near real-time log**
///     </para>
/// </remarks>
[ServiceFilter(typeof(ViewLogViaUrlAccessFilter))]
[HttpGet]
[Route(LogsEndpointPattern)]
[Produces(ContentType.Json, ContentType.Xml)]
[SwaggerResponse((int)HttpStatusCode.OK, typeof(ICollection<LogEntity>))]
public IActionResult GetLogs([FromRoute]int skip, [FromRoute]int take, [FromQuery]string terms)
{
    Expression<Func<LogEntity, bool>> predicate = null;

    var termsNormalize = StringHelper.Normalize(terms);

    if (!string.IsNullOrWhiteSpace(termsNormalize))
    {
        predicate = x => x.Id.ToUpperInvariant().Contains(termsNormalize)
        || x.Message.ToUpperInvariant().Contains(termsNormalize)
        || x.Level.ToString().ToUpperInvariant().Contains(termsNormalize)
        || x.CreatedTime.ToString(Core.Constant.DateTimeOffSetFormat).Contains(termsNormalize);
    }

    var logs = Get(out long total, predicate: predicate, orders: x => x.CreatedTime, isOrderByDescending: true, skip: skip, take: take);

    ContentResult contentResult;

    if (total <= 0)
    {
        return NoContent();
    }

    var placeholderLinkView = PlaceholderLinkViewModel.ToCollection(logEndpointPattern, HttpMethod.Get.Method, new { skip, take, terms });
    var collectionFactoryViewModel = new PagedCollectionFactoryViewModel<LogEntity>(placeholderLinkView, logEndpointPattern);
    var collectionViewModel = collectionFactoryViewModel.CreateFrom(logs, skip, take, total);

    return Ok(collectionViewModel);
}
```

- Use can write your **custom filter** with check `AccessKey` query param with name `AccessKeyQueryParam` from a request by method `Log.IsCanAccessLogViaUrl(HttpContext httpContext)`

## Sample Log File

```json
{
  "Id": "7f0634e4677d4fb49b6f63b6f410a01d",
  "CallerMemberName": "OnException",
  "CallerFilePath": "D:\\Dropbox\\Github\\Monkey\\Monkey\\Monkey\\Filters\\ApiExceptionFilter.cs",
  "CallerRelativePath": "Monkey\\Filters\\ApiExceptionFilter.cs",
  "CallerLineNumber": 50,
  "CreatedTime": "2017-08-23T18:38:49.2024055+07:00",
  "Level": "Fatal",
  "Exception": {
    "Id": "a83b117553614e4f94a3c6eec542a54f",
    "Message": "Input string was not in a correct format.",
    "Source": "System.Private.CoreLib",
    "StackTrace": "   at System.Number.StringToNumber(String str, NumberStyles options, NumberBuffer& number, NumberFormatInfo info, Boolean parseDecimal)\r\n   at System.Number.ParseInt32(String s, NumberStyles style, NumberFormatInfo info)\r\n   at Monkey.Controllers.Api.TestApiController.TestException() in D:\\Dropbox\\Github\\Monkey\\Monkey\\Monkey\\Controllers\\Api\\TestApiController.cs:line 21\r\n   at lambda_method(Closure , Object , Object[] )\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.<InvokeActionMethodAsync>d__27.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()\r\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.<InvokeNextActionFilterAsync>d__25.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Rethrow(ActionExecutedContext context)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)\r\n   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.<InvokeNextExceptionFilterAsync>d__24.MoveNext()",
    "TypeName": "System.FormatException",
    "BaseTypeName": "System.FormatException"
  },
  "HttpContext": {
    "Id": "2273ad4258f84d4e800bcb1ff9c6ebb4",
    "RequestTime": "2017-08-23T06:38:43.643+07:00",
    "Headers": {
      "Connection": [
        "keep-alive"
      ],
      "Accept": [
        "*/*"
      ],
      "Accept-Encoding": [
        "gzip, deflate, br"
      ],
      "Accept-Language": [
        "en-US,en;q=0.8"
      ],
      "Cookie": [
        ".AspNetCore.Antiforgery.5Vkc_VEHWDw=CfDJ8G1e7Qna5WlPnbPKLeQdxZBbyaoWo6yWb_fIy9BB9V_kj_H6rrgFFnrSh982Mosq6XbNEKBjOg1SMoi_7Glc9axfaF5J79oBONAXGqdtxK5TPUWabiSq5caI6LLAGxTdrzkguTXvtFWbe7Y4gBKNUyM; .AspNetCore.Session=CfDJ8G1e7Qna5WlPnbPKLeQdxZBKU8sWxocCOA3LgqXU43Wu0Phe15s4TSfnN5%2BsFSAu9My3Z8XFsZr%2FkDdlXyM2gy0xo%2B9h6B41p0MgY2fJYUzQKTYYEVJaJqwFaISb7hfh%2BDG9UlVim9zBKWQHZP6zWzEvxS9LLFsCazzRHX3kwMxX; .AspNetCore.Antiforgery.qOkU0vtAckQ=CfDJ8G1e7Qna5WlPnbPKLeQdxZDHIs_VOvaTin9_8M8GdE2XuPd8HZzgeCH03l54bMjR3kBj4o7AlWLNiFhsDrewQQ_VaBjaS23OOoAlxb3JXxVUilEegBilX-t7ZlIQ4v5OL-ZJqIB7cUIeMkPyjV9hK2U; driftt_aid=fe218537-7b71-4f2f-aec7-270df2bc46d0; driftt_wmd=true; .AspNetCore.educator_cookie=CfDJ8G1e7Qna5WlPnbPKLeQdxZC81_UkcP4xD5dLK5Zvia8UsIjk0sXJ7LhLs7MNllv-e0fshZwrJy0wDW59YorO3avCMDF913dtK-E2qDt7vmfQvPy_8v44dt7QT59CgJc6eqQDmTU87JMBPAo7pZcINSF0M0VFBU0ckyH1_Bb5xt7ZUGHOoovGNeO5CMBk3uW4fM32L4fTc-ZwplL_E0JaZKL5-rX8iNPtZalzlK7zx7-Hzxo7l_M-k2aPIga2b0WcoBvy3CxQEzVGEoqp9UxGUgvxDSRYKgKX7Zse-QFQ-NUO4OMLUbsSEMs8o0rQ0YNsFIys41X6Ed3NIJV2xHwgkUBxU4PTE-YVS97KyTc9iQXDwT2WCa_Gq-403CKPmevR9iEnMECQbRWDBcvdgqDNDlqMZoiKrhg5eda6ZQ70Ze81fauhC090uTFieiCmv5IaFX2yCuqansaAF5MDhMFKTLXsqDjCXL--1HLB0E5qzhfUoArU-Tk43o9Va3kPPnTUwSJaihR_wXgFQ52NJAnBtcDxoJGfT4VlxzCjWFDJK7EgPlxp9pKwB5lHZ5g7SZwtpEjWStD23Y7m1LMc_O_jg32JpTF1SiC9_7WT9qc2uM6g2tib4CsXL-vFbJR7XTcpKnXq2RJNTkAkrCf2hA1lqGAQ8EvR-RRWwRJO6qn6LGlK2ApzE2XPE3EuWnenv5eQHA; __atuvc=10%7C32%2C15%7C33%2C55%7C34; _ga=GA1.1.917835040.1502609572"
      ],
      "Host": [
        "localhost:7777"
      ],
      "Referer": [
        "http://localhost:7777/developers"
      ],
      "User-Agent": [
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.101 Safari/537.36"
      ],
      "Id": [
        "7f0634e4677d4fb49b6f63b6f410a01d"
      ],
      "RequestTime": [
        "2017/08/23 06:38:43.643 +07:00"
      ]
    },
    "Protocol": "HTTP/1.1",
    "Method": "GET",
    "Endpoint": "http://localhost:7777/api/test/exception",
    "QueryStrings": {},
    "DisplayUrl": "[HTTP/1.1](GET)http://localhost:7777/api/test/exception"
  }
}
```