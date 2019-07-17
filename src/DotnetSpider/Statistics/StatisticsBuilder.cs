using Microsoft.Extensions.DependencyInjection;

namespace Sop.Spider.Statistics
{
	public class StatisticsBuilder
	{
		public IServiceCollection Services { get; }

		public StatisticsBuilder(IServiceCollection services)
		{
			Services = services;
		}
	}
}