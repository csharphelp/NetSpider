using System;

namespace Sop.Spider.Analyzer
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class GlobalValueSelectAttribute : ValueSelectAttribute
	{
		/// <summary>
		/// 解析值的名称
		/// </summary>
		public new string Name { get; set; }
	}
}