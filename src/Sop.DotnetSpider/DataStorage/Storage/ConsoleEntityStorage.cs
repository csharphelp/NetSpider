using System;
using System.Threading.Tasks;
using Sop.DotnetSpider.Common;
using Newtonsoft.Json;
using Sop.DotnetSpider.DataStorage;

namespace Sop.DotnetSpider.DataStorage
{
	/// <summary>
	/// 控制台打印(实体)解析结果
	/// </summary>
	public class ConsoleEntityStorage : StorageBase
	{
		/// <summary>
		/// 根据配置返回存储器
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public static ConsoleEntityStorage CreateFromOptions(SpiderOptions options)
		{
			return new ConsoleEntityStorage();
		}

		protected override Task<DataFlowResult> Store(DataFlowContext context)
		{
			var items = context.GetItems();
			foreach (var item in items)
			{
				foreach (var data in item.Value)
				{
					Console.WriteLine(JsonConvert.SerializeObject(data));
				}
			}

			return Task.FromResult(DataFlowResult.Success);
		}
	}
}