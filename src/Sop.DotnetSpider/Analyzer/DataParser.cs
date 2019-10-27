using System.Threading.Tasks;
using Sop.DotnetSpider.DataStorage;
using Sop.DotnetSpider.DataStorage;

namespace Sop.DotnetSpider.Analyzer
{
	/// <summary>
	/// 默认数据解析器
	/// <code></code>
	/// </summary>
	public class DataParser : DataParserBase
	{
		protected override Task<DataFlowResult> Parse(DataFlowContext context)
		{
			if (context.Response != null)
			{
				context.AddItem("URL", context.Response.Request.Url);
				context.AddItem("Content", context.Response.RawText);
				context.AddItem("TargetUrl", context.Response.TargetUrl);
				context.AddItem("Success", context.Response.Success);
				context.AddItem("ElapsedMilliseconds", context.Response.ElapsedMilliseconds);
			}

			return Task.FromResult(DataFlowResult.Success);
		}
	}
}