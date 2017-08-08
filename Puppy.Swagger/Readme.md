![Logo](favicon.ico)
# Puppy.Swagger
> Project Created by [**Top Nguyen**](http://topnguyen.net)

# How to Use

### 1. Add config section to `appsettings.json`

```json
"ApiDocument": {
    // Api Document Html Title: Name of API Document in HTML
    "ApiDocumentHtmlTitle": "API Document",

    // Access Key Query Param: name of parameter to check can access api document or not
    "AccessKeyQueryParam": "key",

    // Access Key: access key to check with AccessKeyQueryParam - empty is allow annonymous
    "AccessKey": "",

    // Authenticate Token Prefix
    "AuthTokenKeyPrefix": "Bearer",

    // Api Document Url: relative URL with start and end by "/", be careful this may replace or be replace by MVC route
    "ApiDocumentUrl": "/.well-known/api-configuration/",

    // Api Document Name/Version: the path of document URL
    "ApiDocumentName": "latest",

    // Api Document Json File: the file name of json define api.
    "ApiDocumentJsonFile": "all.json",

    // => SO API JSON DOC Endpoint is "/.well-known/api-configuration/latest/all.json"

    // Api Contact Information
    "Contact": {
        "Name": "Top Nguyen",
        "Url": "http://topnguyen.net",
        "Email": "topnguyen92@gmail.com"
    },

    // Enable describe all enums as string - default is true
    "IsDescribeAllEnumsAsString": true,

    // Enable discribe all parameter in camel case - default is true
    "IsDescribeAllParametersInCamelCase": true
}
```

---

### 2. Add Service

```csharp
// [API Document] Swagger
services.AddApiDocument(Path.Combine(Directory.GetCurrentDirectory(), "Documentation.xml"), ConfigurationRoot, "ApiDocument")
```

---

### 3. Use in App Builder

```csharp
// [API Document] Swagger
app.UseApiDocument()
```

---

### 4. Enable Swagger Custom UI

Now you have Swagger Service, let add route for swagger.
This sample below is use `Developers` area as route for swagger.

```csharp
[Route("")]
[HttpGet]
[ServiceFilter(typeof(ApiDocAccessFilter))]
public IActionResult Index() => Helper.GetApiDocHtml(Url, Url.AbsoluteAction("Viewer", "Developers", new { area = "Developers" }));

[Route("Viewer")]
[HttpGet]
[ServiceFilter(typeof(ApiDocAccessFilter))]
public IActionResult Viewer() => Helper.GetApiViewerHtml(Url);
```

`GetApiDocHtml()` and `GetApiJsonViewerHtml()` already have css, js and html inside configuration depend on `Config` object.
