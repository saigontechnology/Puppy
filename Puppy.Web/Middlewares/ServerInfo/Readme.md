# Puppy.Web.Middleware.ServerInfo
> Created by **Top Nguyen** (http://topnguyen.net)

Use server info to let client app know about server author and server time zone 

## Config
- Add config section to `appsettings.json`
- If you not have custom setting in appsettings.json, `default setting` will be apply.

```json
"ServerInfo": {
    // Default is "Top Nguyen"
    "AuthorName": "Top Nguyen",

    // Default is "http://topnguyen.net"
    "AuthorWebsite": "http://topnguyen.net",

    // Default is "topnguyen92@gmail.com"
    "AuthorEmail": "topnguyen92@gmail.com",
    
    // Default is "PHP/5.6.30"
    // Security Purpose
    "PoweredBy": "PHP/5.6.30",
    
    // Default is "cloudflare-nginx"
    // Security Purpose
    "Name": "cloudflare-nginx"
},
```

## Use Application Builder
```csharp
// [Server Info]
app.UseServerInfo(ConfigurationRoot)
```