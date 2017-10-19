![Logo](favicon.ico)
# Puppy.Web
> Project Created by [**Top Nguyen**](http://topnguyen.net)

# Setup Tag Helper

Go to _ViewImports.cshtml then add `@addTagHelper "*, Puppy.Web"`

## Markdown Tag Helper
- Write text in html with tag `<markdown>` or `<p markdown>`
- Note: "---" (3x) in markdown syntax need use as "----" (4x)

```html
<markdown>

```csharp
	public void Main()
	{
		Console.WriteLine("Markdown by Top Nguyen");
	}
```.

</markdown>

@{
    ViewData["MarkdownContent"] = "*Italic*, **bold**, and `monospace`";
}

<markdown asp-for="@ViewData["MarkdownContent"]" />
```

It will transform to
```html
<pre><code class='language-cs'>
	public void Main()
	{
	  Console.WriteLine("Markdown by Top Nguyen");
	}
</code></pre>

<p><em>Italic</em>, <strong>bold</strong>, and <code>monospace</code></p>
```

You can use a library like [HighlightJS](http://highlightjs.org) to sytnax highlight that code block!

```html
@section Styles
{
    <link rel="stylesheet" href="http://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.12.0/styles/default.min.css">
}
@section Scripts{
    <script src="http://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.12.0/highlight.min.js"></script>
    <script>hljs.initHighlightingOnLoad();</script>
}
```

- Please view [markdown.html](Markdown/markdown.html) for sample

## Response Utils
- In your controller, you can create response file with `FileModel` from `Puppy.Core.FileUtils.FileModel` and `Puppy.Core.FileUtils.FileHttpRequestMode` - support view (image only) and download.
```csharp
return Response.CreateResponseFile(fileModel, mode);
```

- Use can save file from `Base 64` format by method `FileModel Save(string base64, string originalFileName, string savePath)` in `Puppy.Core.FileUtils.FileHelper` namespace.

- Then get `FileModel` object to save in your database, after that when client request a file, you can load from your database, map again to `FileModel` and make a response with `FileHttpRequestMode` to user can view (image only) or download by `Response.CreateResponseFile` method as above.

```csharp
using Puppy.Core.FileUtils
...
FileModel fileModel = FileHelper.Save(base64String, fileOriginalName, savePath);
```

## Html Utils
- Support Convert Docx to HTML
- Support Convert HTML to PDF
```csharp
byte[] htmlBytes = HtmlHelper.FromDocx("E:\\SampleConvert.docx");
HtmlHelper.ToPdfFromHtml(Encoding.UTF8.GetString(htmlBytes), @"E:\SampleConvert.pdf");
```

## ServiceCollections Extensions

### 1. MinResponseExtensions
Add mini response service to mini and compress HTML, XML, CSS, JS in response.
- Add Service
```csharp
// [Mini Response]
service.AddMinResponse()
```

- Use Application Builder
```csharp
// [Mini Response]
app.UseMinResponse()
```

### 2. ProcessingTimeExtensions
Use ProcessingTimeExtensions to get the server executed time in reponse header.
- Use Application Builder
```csharp
// [Response] Time Executed Information
app.UseProcessingTime()
```

### 3. RequestRewindExtensions
Use RequestRewindExtensions Allows using several time the stream in ASP.Net Core. Enable Rewind help to get Request Body content.

- Use Application Builder
```csharp
// [Rewind] Enable Rewind help to get Request Body content.
app.UseRequestRewind()
```

### 4.ServerInfoExtensions
- Use server info to let client app know about server author and server time zone.
- Please read the [Readme.md](Middlewares/ServerInfo/Readme.md) file in Middlewares/ServerInfo folder.

### 5.CorsExtensions
- Help to enbale and config Cros for API.
- Please read the [Readme.md](Middlewares/Cros/Readme.md) file in Middlewares/Cros folder.

### 6. HttpContextAccessor
- Add service and use with application builder as the code block below.
- Then just call `System.Web.HttpContext.Current`
```csharp
// [HttpContext]
service.AddHttpContextAccessor()

// [HttpContext]
app.UseHttpContextAccessor()
```

### 7. HttpDetection
- Use with application builder as the code block below.
- Then just call `HttpContext.Connection.RemoteIpAddress;` to get IpAddress even local request.
```csharp
// [Http Detection] Ip Address detection enhance for local request
app.UseHttpDetection()
```
