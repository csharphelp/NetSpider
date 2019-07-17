using System;
using Sop.Spider.Common;
using Sop.Spider.DataFlow;
using Sop.Spider.DownloadAgent;
using Sop.Spider.DownloadAgentRegisterCenter;
using Sop.Spider.DownloadAgentRegisterCenter.Internal;
using Sop.Spider.EventBus;
using Sop.Spider.Network;
using Sop.Spider.Network.InternetDetector;
using Sop.Spider.Statistics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sop.Spider
{
	/// <summary>
	/// 注入事件扩展
	/// </summary>
	public static class ServiceCollectionExtensions
	{
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
			services.AddDownloadCenter(x => x.UseLocalDownloaderAgentStore());
			return services;
		}

		public static DownloadAgentRegisterCenterBuilder UseMySqlDownloaderAgentStore(
			this DownloadAgentRegisterCenterBuilder builder)
		{
			builder.Services.AddSingleton<IDownloaderAgentStore, MySqlDownloaderAgentStore>();
			return builder;
		}
		/// <summary>
		/// 使用注入本地事件下载代理存储
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static DownloadAgentRegisterCenterBuilder UseLocalDownloaderAgentStore(
			this DownloadAgentRegisterCenterBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IDownloaderAgentStore, LocalDownloaderAgentStore>();
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

		#region DownloaderAgent
		/// <summary>
		/// 注入事件下载代理器
		/// </summary>
		/// <param name="services">注入服务</param>
		/// <param name="configure">相关配置委托事件</param>
		/// <returns></returns>
		public static IServiceCollection AddDownloaderAgent(this IServiceCollection services,
			Action<DownloaderAgentBuilder> configure = null)
		{
			
			services.AddSingleton<IHostedService, DefaultDownloaderAgent>();
			services.AddSingleton<NetworkCenter>();
			services.AddSingleton<DownloaderAgentOptions>();

			DownloaderAgentBuilder spiderAgentBuilder = new DownloaderAgentBuilder(services);
			configure?.Invoke(spiderAgentBuilder);

			return services;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		
		public static DownloaderAgentBuilder UseFileLocker(this DownloaderAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<ILockerFactory, FileLockerFactory>();

			return builder;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		[Obsolete("网络拨号器废除，爬虫之判断网路给出通知即可")]
		public static DownloaderAgentBuilder UseDefaultAdslRedialer(this DownloaderAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<IAdslRedialer, DefaultAdslRedialer>();

			return builder;
		}

		public static DownloaderAgentBuilder UseDefaultInternetDetector(this DownloaderAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<IInternetDetector, DefaultInternetDetector>();

			return builder;
		}

		public static DownloaderAgentBuilder UseVpsInternetDetector(this DownloaderAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<IInternetDetector, VpsInternetDetector>();

			return builder;
		}

		#endregion

		#region  Statistics

		public static IServiceCollection AddStatisticsCenter(this IServiceCollection services,
			Action<StatisticsBuilder> configure)
		{
			services.AddSingleton<IHostedService, StatisticsCenter>();

			var spiderStatisticsBuilder = new StatisticsBuilder(services);
			configure?.Invoke(spiderStatisticsBuilder);

			return services;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static StatisticsBuilder UseMemory(this StatisticsBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IStatisticsStore, MemoryStatisticsStore>();
			return builder;
		}

		public static StatisticsBuilder UseMySql(this StatisticsBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IStatisticsStore, MySqlStatisticsStore>();
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