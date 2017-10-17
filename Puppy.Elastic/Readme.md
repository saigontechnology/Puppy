![Logo](favicon.ico)
# Puppy.Elastic
> Project Created by [**Top Nguyen**](http://topnguyen.net)

- Add Elastic Search to Service Collection
```csharp
// [Elastic Search]
services.AddElastic(ConfigurationRoot);
```

- Use Elastic Search for ApplicationBuidler
```csharp
// [Elastic Search]
app.UseElastic();
```

- Add "Elastic" section in appsettings.json to config the Connection String
```javascript
// [Auto Reload]
"Elastic": {
    // Elastic connection string. Default is "http://127.0.0.1:9200" 
    "ConnectionString": "http://127.0.0.1:9200",
}
```

# Use
- Inject `IElasticContext` and call method insite interface.

# Note
- **Security** issue (Elastic Search Engine) not provice anything to protect data.
    > So be careful: just index and "elastic" what is publish for end-user.
- **No Transaction**
- **Nearly realtime, not realtime**. => need call elastic **refresh to make it real time**
- Elasticsearch is a great tool, but it is designed for search not to serve as a database