using Sample.samples;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Sample
{
	class Program
	{
		
		static async Task Main(string[] args)
		{
			var configure = new LoggerConfiguration()

#if DEBUG
				.MinimumLevel.Verbose()
#else
				.MinimumLevel.Information()
#endif
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.WriteTo
				.RollingFile("dotnet-spider.log");
			Log.Logger = configure.CreateLogger();

			//await BaseUsage.Run();

			await EntityModelSpider.Run();
			Console.Read();
		}
	}
}