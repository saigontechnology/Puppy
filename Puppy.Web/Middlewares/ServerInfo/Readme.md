# Puppy.Web.Middleware.ServerInfo
> Created by **Top Nguyen** (http://topnguyen.net)

Use server info to let client app know about server author and server time zone 

## Config
- Add config section to `appsettings.json`
- If you not have custom setting in appsettings.json, `default setting` will be apply.

```json
  "ServerInfo": {
    "AuthorName": "Top Nguyen",
    "AuthorWebsite": "http://topnguyen.net",
    "AuthorEmail": "topnguyen92@gmail.com",
    "PoweredBy": "PHP/5.6.30",
    "Name": "cloudflare-nginx",
    "TimeZoneId": "Co-ordinated Universal Time", // "SE Asia Standard Time" for VietNam
    "CookieSchemaName": "Puppy_Cookie"
  },
```

## Use Application Builder
```csharp
// [Server Info]
app.UseServerInfo(ConfigurationRoot)
```