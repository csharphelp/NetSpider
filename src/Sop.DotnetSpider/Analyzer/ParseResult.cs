using System.Collections.Generic;
using Sop.Spider.DataStorage;

namespace Sop.Spider.Analyzer
{


	/// <summary>
	/// 实体解析结果
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ParseResult<T> : List<T>, IParseResult where T : EntityBase<T>, new()
    {
    }
}