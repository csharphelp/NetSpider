using System;
using System.Net;

namespace Sop.Spider.Analyzer
{
	/// <summary>
	/// 把数值进行HTML解码
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class DeCodeEnCodeFormatAttribute : FormatBaseAttribute
	{
		public CodeType CodeType { get; set; }
		/// <summary>
		/// 实现数值的转化
		/// </summary>
		/// <param name="value">数值</param>
		/// <returns>被格式化后的数值</returns>
		protected override string FormatValue(string value)
		{
			string tmp = value;
			switch (CodeType)
			{
				case CodeType.UrlEncode:
					tmp = WebUtility.UrlEncode(tmp);
					break;
				case CodeType.UrlDecode:
					tmp = WebUtility.UrlEncode(tmp);
					break;
				case CodeType.HtmlEncode:
					tmp = WebUtility.UrlEncode(tmp);
					break;
				case CodeType.HtmlDecode:
					tmp = WebUtility.UrlEncode(tmp);
					break;
				default:
					tmp = WebUtility.UrlEncode(tmp);
					break;
			}
			return tmp;



		}

		/// <summary>
		/// 校验参数是否设置正确
		/// </summary>
		protected override void CheckArguments()
		{
		}
	}



}
