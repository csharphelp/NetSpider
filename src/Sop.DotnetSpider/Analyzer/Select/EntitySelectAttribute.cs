using System;
using Sop.DotnetSpider.Analyzer;

namespace Sop.DotnetSpider.Analyzer.Select
{
	/// <summary>
	/// 实体选择器
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class EntitySelectAttribute : SelectAttribute
	{
		/// <summary>
		/// 从最终解析到的结果中取前 Take 个实体
		/// </summary>
		public int Take { get; set; } = -1;

		/// <summary>
		/// 设置 Take 的方向, 默认是从头部取
		/// </summary>
		public bool TakeFromHead { get; set; } = true;
	}
}