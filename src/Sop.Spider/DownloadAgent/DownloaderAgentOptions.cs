using System;
using Microsoft.Extensions.Configuration;

namespace Sop.Spider.DownloadAgent
{
	/// <summary>
	/// 下载器代理选项
	/// </summary>
	public class DownloaderAgentOptions
	{
		private readonly IConfiguration _configuration;
		private readonly string _defaultAgentId = Guid.NewGuid().ToString("N");

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="configuration">配置接口</param>
		public DownloaderAgentOptions(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// 是否支持 ADSL 拨号
		/// </summary>
		[Obsolete("网络拨号器废除，爬虫之判断网路给出通知即可")]
		public bool SupportAdsl => !string.IsNullOrWhiteSpace(AdslAccount);

		/// <summary>
		/// 是否忽略拨号，用于测试
		/// </summary>
		[Obsolete("网络拨号器废除，爬虫之判断网路给出通知即可")]
		public bool IgnoreRedialForTest => !string.IsNullOrWhiteSpace(_configuration["IgnoreRedialForTest"]) &&
										   bool.Parse(_configuration["IgnoreRedialForTest"]);

		/// <summary>
		/// 拨号间隔限制
		/// </summary>
		[Obsolete("网络拨号器废除，爬虫之判断网路给出通知即可")]
		public int RedialIntervalLimit => string.IsNullOrWhiteSpace(_configuration["RedialIntervalLimit"])
			? 2
			: int.Parse(_configuration["RedialIntervalLimit"]);

		/// <summary>
		/// 下载器代理标识
		/// </summary>
		public string AgentId => string.IsNullOrWhiteSpace(_configuration["AgentId"])
			? _defaultAgentId
			: _configuration["AgentId"];

		/// <summary>
		/// 下载器代理名称
		/// </summary>
		public string Name => string.IsNullOrWhiteSpace(_configuration["AgentName"])
			? "DownloadAgent"
			: _configuration["AgentName"];

		/// <summary>
		/// ADSL 网络接口
		/// </summary>
		[Obsolete("网络拨号器废除，爬虫之判断网路给出通知即可")]
		public string AdslInterface => _configuration["AdslInterface"];

		/// <summary>
		/// ADSL 帐号
		/// </summary>
		[Obsolete("网络拨号器废除，爬虫之判断网路给出通知即可")]
		public string AdslAccount => _configuration["AdslAccount"];

		/// <summary>
		/// ADSL 密码
		/// </summary>
		[Obsolete("网络拨号器废除，爬虫之判断网路给出通知即可")]
		public string AdslPassword => _configuration["AdslPassword"];

		/// <summary>
		/// 代理供应接口
		/// </summary>
		public string ProxySupplyUrl => _configuration["ProxySupplyUrl"];

		/// <summary>
		/// 请求结果插入队列尝试次数
		/// </summary>
		public int MessageAttempts => string.IsNullOrWhiteSpace(_configuration["MessageAttempts"])
			? 1
			: int.Parse(_configuration["MessageAttempts"]);
	}
}