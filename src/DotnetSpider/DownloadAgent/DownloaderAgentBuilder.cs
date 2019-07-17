using Microsoft.Extensions.DependencyInjection;

namespace Sop.Spider.DownloadAgent
{
	public class DownloaderAgentBuilder
	{
		public IServiceCollection Services { get; }
		
		public DownloaderAgentBuilder(IServiceCollection services)
		{
			Services = services;
		}
	}
}