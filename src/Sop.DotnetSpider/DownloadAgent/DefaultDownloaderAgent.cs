using System;
using Sop.DotnetSpider.Common;
using Sop.DotnetSpider.EventBus;
using Sop.DotnetSpider.Network;
using Microsoft.Extensions.Logging;

namespace Sop.DotnetSpider.DownloadAgent
{
	/// <summary>
	/// 默认下载器代理
	/// </summary>
	public class DefaultDownloadAgent : DownloadAgentBase
	{
	

		public DefaultDownloadAgent(DownloadAgentOptions options,
			SpiderOptions spiderOptions, IEventBus eventBus, ILogger<DefaultDownloadAgent> logger) : base(options, spiderOptions, eventBus, logger)
		{
		}

	}
}