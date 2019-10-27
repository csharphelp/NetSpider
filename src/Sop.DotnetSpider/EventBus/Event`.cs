using Sop.DotnetSpider.Common;
using System;

namespace Sop.DotnetSpider.EventBus
{
	/// <summary>
	/// 命令消息
	/// </summary>
	public sealed partial class Event
	{
		public bool IsTimeout(int seconds = 30)
		{
			var timestamp = DateTimeHelper.ToUnixTime(Timestamp);
			return (DateTime.Now - timestamp).TotalSeconds <= seconds;
		}
	}
}