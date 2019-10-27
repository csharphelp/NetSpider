using Microsoft.Extensions.DependencyInjection;

namespace Sop.DotnetSpider.Statistics
{
	/// <summary>
	/// 
	/// </summary>
	public class StatisticsBuilder
	{
		/// <summary>
		/// 
		/// </summary>
		public IServiceCollection Services { get; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		public StatisticsBuilder(IServiceCollection services)
		{
			Services = services;
		}
	}
}