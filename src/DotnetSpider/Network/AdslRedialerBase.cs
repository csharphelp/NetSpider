using Sop.Spider.DownloadAgent;
using System;

namespace Sop.Spider.Network
{
	/// <summary>
	/// 拨号器
	/// </summary>
	[Obsolete("网络拨号器废除，爬虫之判断网路给出通知即可")]
	public abstract class AdslRedialerBase : IAdslRedialer
	{
		/// <summary>
		/// 拨号
		/// </summary>
		public abstract bool Redial();

		/// <summary>
		/// 配置
		/// </summary>
		protected DownloaderAgentOptions Options { get; }


		/// <summary>
		/// 构造方法
		/// </summary>
		protected AdslRedialerBase(DownloaderAgentOptions options)
		{
			Options = options;
		}
	}
}