using System.Collections;
using System.Collections.Generic;
using Sop.Spider.DataFlow.Storage.Model;

namespace Sop.Spider.DataFlow.Parser
{
	/// <summary>
	/// 实体解析结果
	/// </summary>
    public interface IParseResult : IEnumerable
    {
    }

	/// <summary>
	/// 实体解析结果
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class ParseResult<T> : List<T>, IParseResult where T : EntityBase<T>, new()
    {
    }
}