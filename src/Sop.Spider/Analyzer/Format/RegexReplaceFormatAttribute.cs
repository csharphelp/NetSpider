using System;
using System.Text.RegularExpressions;

namespace Sop.Spider.Analyzer
{
	/// <summary>
	///  正则格式化属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RegexReplaceFormatAttribute : FormatBaseAttribute
	{
		/// <summary>
		/// 正则类型
		/// </summary>
		public RegexType RegexType { get; set; } = RegexType.Customize;
		/// <summary>
		/// 表达式
		/// </summary>
		public string Expression { get; set; }

		/// <summary>
		/// 要替换的新值
		/// </summary>
		public string NewValue { get; set; }



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
			return Regex.Replace(value, Expression, NewValue);
		}

		/// <summary>
		/// 校验参数是否设置正确
		/// </summary>
		protected override void CheckArguments()
		{
			if (RegexType.Customize == RegexType && string.IsNullOrWhiteSpace(Expression))
			{
				throw new SpiderArgumentException("RegexReplaceFormatAttribute ");
			}
			 
		}
	}
}
