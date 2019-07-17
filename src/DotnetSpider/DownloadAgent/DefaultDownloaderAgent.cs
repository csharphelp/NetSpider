using Sop.Spider.Common;
using Sop.Spider.EventBus;
using Sop.Spider.Network;
using Microsoft.Extensions.Logging;

namespace Sop.Spider.DownloadAgent
{
	/// <summary>
	/// 默认下载器代理
	/// </summary>
	public class DefaultDownloaderAgent : DownloaderAgentBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="options">下载器代理选项</param>
		/// <param name="spiderOptions">任务选项</param>
		/// <param name="eventBus">消息队列</param>
		/// <param name="networkCenter">网络中心</param>
		/// <param name="logger">日志接口</param>
		public DefaultDownloaderAgent(DownloaderAgentOptions options,
			SpiderOptions spiderOptions, IEventBus eventBus, NetworkCenter networkCenter,
			ILogger<DefaultDownloaderAgent> logger) : base(options, spiderOptions, eventBus, networkCenter, logger)
		{
		}

		public DefaultDownloaderAgent(DownloaderAgentOptions options,
			SpiderOptions spiderOptions, IEventBus eventBus, ILogger<DefaultDownloaderAgent> logger) : base(options, spiderOptions, eventBus, logger)
		{
		}

	}
}