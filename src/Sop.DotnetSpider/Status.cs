using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sop.DotnetSpider
{
	/// <summary>
	/// 爬虫状态
	/// </summary>
	[Flags]
	[JsonConverter(typeof(StringEnumConverter))]
	public enum Status
	{
		/// <summary>
		/// 正在运行
		/// </summary>
		Running = 1,

		/// <summary>
		/// 暂停
		/// </summary>
		Paused = 2,

		/// <summary>
		/// 退出中
		/// </summary>
		Exiting = 4,

		/// <summary>
		/// 退出完成
		/// </summary>
		Exited = 8
	}
}