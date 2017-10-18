![Logo](favicon.ico)
# Puppy.Elastic
> Project Created by [**Top Nguyen**](http://topnguyen.net)

# Install Elastic

- Support Elastic Search version 2.4.6

- [Download Elastic 2.4.6](https://www.elastic.co/downloads/past-releases/elasticsearch-2-4-6)

- After download elastic search - unzup the zip file

- Go to `config` > `elasticsearch.yml` to edit config if you want, the config below is sample.
	+ cluster.name puppy.elastic
	+ node.name: puppy.elastic-primary
	+ network.host: 127.0.0.1
	+ http.port: 9200
	> [Setup Guide](https://www.elastic.co/guide/en/elasticsearch/reference/2.4/setup.html)

- [Download Java Machine](https://java.com/en/download/manual.jsp)
- [Setting JAVA_HOME variable](https://confluence.atlassian.com/doc/setting-the-java_home-variable-in-windows-8895.html)
- After setup JAVA_HOME, you can restart to update environment variables or use cmd with administrator permission `taskkill /f /im explorer.exe`.
- Then open `explorere` again by `Task Manager` (access via alt + ctrl + delete then File > Run New Task > explorer)
- Go to `bin` and open cmd at the directory
- Use cmd `service install`
- Then cmd `service manager` then setting `start` service and `start mode` is `automatic`
	> [Run as Service](https://www.elastic.co/guide/en/elasticsearch/reference/2.4/setup-service-win.html)

# Use Elastic

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

- Inject `IElasticContext` and call method insite interface.

# Note
- **Security** issue (Elastic Search Engine) not provice anything to protect data.
    > So be careful: just index and "elastic" what is publish for end-user.
- **No Transaction**
- **Nearly realtime, not realtime**. => need call elastic **refresh to make it real time**
- Elasticsearch is a great tool, but it is designed for search not to serve as a database