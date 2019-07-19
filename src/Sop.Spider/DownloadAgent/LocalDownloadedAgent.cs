using System;
using Sop.Spider.Common;
using Sop.Spider.EventBus;
using Sop.Spider.Network;
using Microsoft.Extensions.Logging;

namespace Sop.Spider.DownloadAgent
{
	/// <summary>
	/// 本地下器代理
	/// </summary>
	public class LocalDownloadedAgent : DefaultDownloaderAgent
	{
	 
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="options">下载器代理选项</param>
		/// <param name="spiderOptions"></param>
		/// <param name="eventBus">消息队列</param>
		/// <param name="logger">日志接口</param>
		public LocalDownloadedAgent(DownloaderAgentOptions options, SpiderOptions spiderOptions,
			IEventBus eventBus,
			ILogger<LocalDownloadedAgent> logger) : base(options, spiderOptions,
			eventBus, logger)
		{
			// ConfigureDownloader = downloader => downloader.Logger = null;
		}
	}
}