using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sop.Spider.Common;
using Sop.Spider.DownloadAgent;
using Sop.Spider.DownloadAgentRegisterCenter;
using Sop.Spider.DownloadAgentRegisterCenter.Internal;
using Sop.Spider.EventBus;
using Sop.Spider.Network.InternetDetector;
using Sop.Spider.Statistics;
using System;


namespace Sop.Spider
{
	/// <summary>
	/// 注入事件扩展
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IServiceCollection ConfigureAppConfiguration(this IServiceCollection services,
			string config = null)
		{
			Check.NotNull(services, nameof(services));

			var configurationBuilder = Framework.CreateConfigurationBuilder(config);
			IConfigurationRoot configurationRoot = configurationBuilder.Build();
			services.AddSingleton<IConfiguration>(configurationRoot);

			return services;
		}

		#region DownloadCenter

		public static IServiceCollection AddDownloadCenter(this IServiceCollection services,
			Action<DownloadAgentRegisterCenterBuilder> configure = null)
		{
			services.AddSingleton<IHostedService, DefaultDownloadAgentRegisterCenter>();

			DownloadAgentRegisterCenterBuilder downloadCenterBuilder = new DownloadAgentRegisterCenterBuilder(services);
			configure?.Invoke(downloadCenterBuilder);

			return services;
		}
		/// <summary>
		/// 注入本地下载事件
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddLocalDownloadCenter(this IServiceCollection services)
		{
			services.AddDownloadCenter(x => x.UseLocalDownloadAgentStore());
			return services;
		}

		public static DownloadAgentRegisterCenterBuilder UseMySqlDownloadAgentStore(
			this DownloadAgentRegisterCenterBuilder builder)
		{
			builder.Services.AddSingleton<IDownloadAgentStore, MySqlDownloadAgentStore>();
			return builder;
		}
		/// <summary>
		/// 使用注入本地事件下载代理存储
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static DownloadAgentRegisterCenterBuilder UseLocalDownloadAgentStore(
			this DownloadAgentRegisterCenterBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IDownloadAgentStore, LocalDownloadAgentStore>();
			return builder;
		}

		#endregion

		#region  EventBus
		/// <summary>
		/// 事件注入（本地事件注入）
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddLocalEventBus(this IServiceCollection services)
		{
			services.AddSingleton<IEventBus, LocalEventBus>();
			return services;
		}

		#endregion

		#region DownloadAgent
		/// <summary>
		/// 注入事件下载代理器
		/// </summary>
		/// <param name="services">注入服务</param>
		/// <param name="configure">相关配置委托事件</param>
		/// <returns></returns>
		public static IServiceCollection AddDownloadAgent(this IServiceCollection services,
			Action<DownloadAgentBuilder> configure = null)
		{
			
			services.AddSingleton<IHostedService, DefaultDownloadAgent>();
			//services.AddSingleton<NetworkCenter>();
			services.AddSingleton<DownloadAgentOptions>();

			DownloadAgentBuilder spiderAgentBuilder = new DownloadAgentBuilder(services);
			configure?.Invoke(spiderAgentBuilder);

			return services;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static DownloadAgentBuilder UseFileLocker(this DownloadAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<ILockerFactory, FileLockerFactory>();

			return builder;
		}
		/// <summary>
		/// 检查网络
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static DownloadAgentBuilder UseDefaultInternetDetector(this DownloadAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<IInternetDetector, DefaultInternetDetector>();

			return builder;
		}
 
		#endregion

		#region  Statistics
		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configure"></param>
		/// <returns></returns>
		public static IServiceCollection AddStatisticsCenter(this IServiceCollection services,
			Action<StatisticsBuilder> configure)
		{
			services.AddSingleton<IHostedService, StatisticsCenter>();

			var spiderStatisticsBuilder = new StatisticsBuilder(services);
			configure?.Invoke(spiderStatisticsBuilder);

			return services;
		}
		/// <summary>
		/// 使用内存类
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static StatisticsBuilder UseMemory(this StatisticsBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IStatisticsStore, MemoryStatisticsStore>();
			return builder;
		}
		/// <summary>
		/// 使用mysql
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static StatisticsBuilder UseMySql(this StatisticsBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IStatisticsStore, MySqlStatisticsStore>();
			return builder;
		}
		/// <summary>
		/// 使用Redis
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static StatisticsBuilder UseRedis(this StatisticsBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			
			builder.Services.AddSingleton<IStatisticsStore, RedisStatisticsStore>();
			return builder;
		}

		#endregion

		#region Sop.Spider

		public static IServiceCollection AddSpider(this IServiceCollection services)
		{
			
			return services;
		}
		
		#endregion
	}
}