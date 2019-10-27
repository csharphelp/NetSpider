using Microsoft.Extensions.DependencyInjection;

namespace Sop.DotnetSpider.DownloadAgentRegisterCenter
{
	public class DownloadAgentRegisterCenterBuilder
	{
		public IServiceCollection Services { get; }

		public DownloadAgentRegisterCenterBuilder(IServiceCollection services)
		{
			Services = services;
		}
	}
}