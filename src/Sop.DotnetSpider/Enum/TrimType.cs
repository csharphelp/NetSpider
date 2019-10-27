using System;
using System.Collections.Generic;
using System.Text;

namespace Sop.Spider
{
	/// <summary>
	/// Trim 类型
	/// </summary>
	public enum TrimType
	{
		/// <summary>
		/// 只Trim后边
		/// </summary>
		Right,
		/// <summary>
		/// 只Trim前边
		/// </summary>
		Left,

		/// <summary>
		/// 去除所有空格、空白字符
		/// </summary>
		All,
		/// <summary>
		/// Trim前后
		/// </summary>
		RightLeft,
	}
}
