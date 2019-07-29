using Microsoft.Extensions.DependencyInjection;

namespace Sop.Spider.DownloadAgent
{
	public class DownloadAgentBuilder
	{
		public IServiceCollection Services { get; }
		
		public DownloadAgentBuilder(IServiceCollection services)
		{
			Services = services;
		}
	}
}