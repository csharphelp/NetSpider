using System;

namespace Sop.Spider.Network
{
	/// <summary>
	/// 拨号器
	/// </summary>
	[Obsolete("网络拨号器废除，爬虫之判断网路给出通知即可")]
	public interface IAdslRedialer
	{
		/// <summary>
		/// 拨号
		/// </summary>
		bool Redial();
	}
}