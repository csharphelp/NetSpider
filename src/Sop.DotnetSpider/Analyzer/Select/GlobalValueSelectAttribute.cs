using System;
using Sop.DotnetSpider.Analyzer;

namespace Sop.DotnetSpider.Analyzer.Select
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class GlobalValueSelectAttribute : ValueSelectAttribute
	{
		/// <summary>
		/// 解析值的名称
		/// </summary>
		public new string Name { get; set; }
	}
}