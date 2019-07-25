using System;

namespace Sop.Spider
{
	/// <summary>
	/// SpiderFormatException异常
	/// </summary>
	public class SpiderFormatException : FormatException
	{
		public SpiderFormatException(string msg) : base(msg)
		{
		}

	}


	/// <summary>
	/// 
	/// </summary>
	public class SpiderArgumentException : ArgumentException
	{
		public SpiderArgumentException(string msg) : base(msg)
		{
		}
		public SpiderArgumentException(string msg, string ad) : base(msg, ad)
		{
		}

	}
	/// <summary>
	/// 表示应用程序执行期间发生的错误。
	/// </summary>
	public class SpiderException : Exception
	{
		public SpiderException(string msg) : base(msg)
		{
		}

	}
	/// <summary>
	/// 不支持调用的方法时，或者尝试读取，搜索或写入不支持调用的功能的流时引发的异常
	/// </summary>
	public class SpiderNotSupportedException : NotSupportedException
	{
		public SpiderNotSupportedException(string msg) : base(msg)
		{
		}

	}
	public class SpiderInvalidOperationException : InvalidOperationException
	{
		public SpiderInvalidOperationException(string msg) : base(msg)
		{
		}

	}



}