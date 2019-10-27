using System;
using System.Text.RegularExpressions;
using Sop.DotnetSpider.Analyzer.Format;

namespace Sop.DotnetSpider.Analyzer
{
	/// <summary>
	///  正则格式化属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RegexReplaceFormatAttribute : FormatBaseAttribute
	{
		/// <summary>
		/// 表达式
		/// </summary>
		public string Expression { get; set; }
		/// <summary>
		/// 忽略比较
		/// </summary>
		public RegexOptions RegexOptions { get; set; } = RegexOptions.None;

		/// <summary>
		/// 要替换的新值
		/// </summary>
		public string NewValue { get; set; } = "";



		/// <summary>
		/// 实现数值的转化
		/// </summary>
		/// <param name="value">数值</param>
		/// <returns>被格式化后的数值</returns>
		protected override string FormatValue(string value)
		{
			if (string.IsNullOrWhiteSpace(NewValue))
			{
				NewValue = "";
			}
			value = Regex.Replace(value, Expression, NewValue, RegexOptions);
			return value;
		}

		/// <summary>
		/// 校验参数是否设置正确
		/// </summary>
		protected override void CheckArguments()
		{
			if (string.IsNullOrWhiteSpace(Expression))
			{
				throw new SpiderArgumentException("RegexReplaceFormatAttribute ");
			}

		}
	}
}
