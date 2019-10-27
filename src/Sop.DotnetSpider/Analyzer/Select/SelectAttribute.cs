using Newtonsoft.Json;
using Sop.DotnetSpider;

namespace Sop.DotnetSpider.Analyzer.Select
{
	/// <summary>
	/// 选择器特性
	/// </summary>
	public class SelectAttribute : System.Attribute
	{
#if !NET451
		/// <summary>
		/// 避免被序列化出去
		/// </summary>
		[JsonIgnore]
		public override object TypeId => base.TypeId;
#endif

		/// <summary>
		/// 构造方法
		/// </summary>
		public SelectAttribute()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="expression">表达式</param>
		/// <param name="type">选择器类型</param>
		/// <param name="arguments">参数</param>
		public SelectAttribute(string expression, SelectorType type = SelectorType.XPath, string arguments = null)
		{
			Type = type;
			Expression = expression;
			Arguments = arguments;
		}

		/// <summary>
		/// 选择器类型
		/// </summary>
		public SelectorType Type { get; set; } = SelectorType.XPath;

		/// <summary>
		/// 表达式
		/// </summary>
		public string Expression { get; set; }

		/// <summary>
		/// 参数
		/// </summary>
		public string Arguments { get; set; }
	}
}