using System;
using Sop.Spider.Common;
using Sop.Spider.EventBus;
using Sop.Spider.Network;
using Microsoft.Extensions.Logging;

namespace Sop.Spider.DownloadAgent
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