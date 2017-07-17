![Logo](favicon.ico)
# Puppy.Web
> Project Created by [**Top Nguyen**](http://topnguyen.net)

## Setup Program.cs

```c#
public class Program
{
    public static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.Title = "Monkey Web API";

        IWebHostBuilder hostBuilder =
            new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                })
                .UseWebRoot(Core.Constants.System.WebRoot)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();

        hostBuilder.Build().Run();
    }
}
```