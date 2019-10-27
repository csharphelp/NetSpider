using System;
using System.Collections.Generic;
using System.Text;

namespace Sop.DotnetSpider
{
	/// <summary>
	/// 元素取值方式
	/// </summary>
	public enum ValueOption
	{
		/// <summary>
		/// For json content
		/// </summary>
		None,

		/// <summary>
		///获取或设置HTML中的对象及其内容。
		/// </summary>
		OuterHtml,

		/// <summary>
		/// HTML内容
		/// </summary>
		InnerHtml,

		/// <summary>
		/// 文字
		/// </summary>
		InnerText,
		/// <summary>
		/// 
		/// </summary>
		Count
	}
}
