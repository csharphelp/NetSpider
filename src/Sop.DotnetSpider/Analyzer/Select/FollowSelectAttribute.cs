using System;
using Newtonsoft.Json;

namespace Sop.Spider.Analyzer
{
	/// <summary>
	/// 目标链接选择器（用于批量采集时，选择目标）
	/// Todo 待完善 需求如下
	/// 1、要求支持JQ操作（期望，不一定实现，实现难度太大）
	/// 2、支持循环操作
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class FollowSelectAttribute : System.Attribute
	{
#if !NET451
		/// <summary>
		/// 避免被序列化出去
		/// </summary>
		[JsonIgnore]
		public override object TypeId => base.TypeId;
#endif
		public FollowSelectAttribute()
		{
		}

		public FollowSelectAttribute(string[] xPaths, string[] patterns = null)
		{
			XPaths = xPaths;
			Patterns = patterns;
		}

		public FollowSelectAttribute(string xpath)
		{
			XPaths = new[] {xpath};
		}

		public FollowSelectAttribute(string xpath, string pattern)
		{
			XPaths = new[] {xpath};
			Patterns = new[] {pattern};
		}

		/// <summary>
		/// 目标链接所在区域
		/// </summary>
		public string[] XPaths { get; set; }

		/// <summary>
		/// 匹配目标链接的正则表达式
		/// </summary>
		public string[] Patterns { get; set; }

		/// <summary>
		/// 需要排除链接的正则表达式
		/// </summary>
		public string[] ExcludePatterns { get; set; }



	}
}