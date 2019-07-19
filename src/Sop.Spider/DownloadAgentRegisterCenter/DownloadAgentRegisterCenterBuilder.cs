using Microsoft.Extensions.DependencyInjection;

namespace Sop.Spider.DownloadAgentRegisterCenter
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