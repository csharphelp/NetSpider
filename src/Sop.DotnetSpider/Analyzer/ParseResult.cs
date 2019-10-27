using System.Collections.Generic;
using Sop.DotnetSpider.DataStorage;

namespace Sop.DotnetSpider.Analyzer
{


	/// <summary>
	/// 实体解析结果
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ParseResult<T> : List<T>, IParseResult where T : EntityBase<T>, new()
    {
    }
}