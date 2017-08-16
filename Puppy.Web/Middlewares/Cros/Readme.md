# Puppy.Web.Middleware.Cros
> Created by **Top Nguyen** (http://topnguyen.net)

Help to enbale and config Cros for API.

## Config
- Add config section to `appsettings.json`
- If you not have custom setting in appsettings.json, `default setting` will be apply.

```json
"Cros": {
    "PolicyAllowAllName": "PolicyAllowAll",
    "AccessControlAllowOrigin": "*",
    "AccessControlAllowHeaders": "Authorization, Content-Type",
    "AccessControlAllowMethods": "GET, POST, PUT, HEAD"
}
```

## Add Service

```csharp
// [API Cros]
services.AddCros(ConfigurationRoot, "Cros");
```

---

## Use Application Builder

```csharp
// [API Cros]
app.UseCros();
```