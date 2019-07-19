using System;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Sop.Spider.Analyzer
{
	/// <summary>
	/// 数据格式化属性的基类
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public abstract class FormatBaseAttribute : System.Attribute
	{
		private readonly ILogger _logger;
		/// <summary>
		/// 构造方法
		/// </summary>
		protected FormatBaseAttribute()
		{
			Name = GetType().Name;
		}

		/// <summary>
		/// 格式化的名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 默认属性，如果被格式化的值为空的返回值
		/// </summary>
		public string ValueWhenNull { get; set; }

		/// <summary>
		/// 实现数值的转化
		/// </summary>
		/// <param name="value">数值</param>
		/// <returns>被格式化后的数值</returns>
		protected abstract string FormatValue(string value);

		/// <summary>
		/// 校验参数是否设置正确
		/// </summary>
		protected abstract void CheckArguments();

		/// <summary>
		/// 格式化数据
		/// </summary>
		/// <param name="value">数值</param>
		/// <returns>被格式化后的数值</returns>
		public string Format(string value)
		{
			CheckArguments();

			return value == null ? ValueWhenNull : FormatValue(value);
		}
	}
}
