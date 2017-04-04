# 1. Setup Program.cs
	public class Program
	{
		public static void Main(string[] args)
		{
			IWebHostBuilder hostBuilder =
				new WebHostBuilder()
					.UseKestrel()
					.UseContentRoot(Directory.GetCurrentDirectory())
					.UseIISIntegration()
					.UseStartup<Startup>();

			hostBuilder.RunWithBrowser();
		}
	}
