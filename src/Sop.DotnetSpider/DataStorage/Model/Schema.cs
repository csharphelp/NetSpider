using System;

namespace Sop.Spider.DataStorage
{
	/// <summary>
	/// 数据库架构信息
	/// </summary>
	public class Schema : Attribute
	{
		/// <summary>
		/// 数据库名
		/// </summary>
		public string Database { get; }

		/// <summary>
		/// 表名
		/// </summary>
		public string Table { get; }

		/// <summary>
		/// 表名后缀
		/// </summary>
		public TablePostfix TablePostfix { get; set; }
		/// <summary>
		/// 自定义后缀
		/// </summary>
		public string TablePostfixFormat { get; set; }

		/// <summary>
		/// 数据库
		/// </summary>
		/// <param name="database">数据库名</param>
		/// <param name="table">表名</param>
		/// <param name="tablePostfix">表名后缀</param>
		/// <param name="tablePostfixFormat">自定义后缀</param>
		public Schema(string database, string table, TablePostfix tablePostfix = TablePostfix.None, string tablePostfixFormat = null)
		{
			Database = database;
			Table = table;
			TablePostfix = tablePostfix;
			if (tablePostfix == TablePostfix.DateFormat)
			{
				try
				{
					if (string.IsNullOrWhiteSpace(tablePostfixFormat))
					{
						throw  new SpiderArgumentException("Sop.Spider.DataStorage tablePostfixFormat为空");
					}
					else
					{
						DateTime.Now.ToString(tablePostfixFormat);
						TablePostfixFormat = tablePostfixFormat;
					}
				}
				catch (Exception ex)
				{
					throw new SpiderArgumentException("--[Sop.Spider.DataStorage ]");
				}
			}
			
		}
	}
}