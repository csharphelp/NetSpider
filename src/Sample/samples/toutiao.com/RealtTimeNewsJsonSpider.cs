using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Sop.Spider;
using Sop.Spider.Analyzer;
using Sop.Spider.Common;
using Sop.Spider.DataStorage;
using Sop.Spider.Download;
using Sop.Spider.EventBus;
using Sop.Spider.Scheduler;
using Sop.Spider.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Sample.samples
{
	/// <summary>
	/// 实体信息采集器
	/// 1、https://www.toutiao.com/api/pc/realtime_news/
	/// 2、参考jsonPath https://blog.csdn.net/fwk19840301/article/details/80452258
	/// 
	/// </summary>
	public class RealtTimeNewsJsonSpider : Spider
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="mq"></param>
		/// <param name="statisticsService"></param>
		/// <param name="options"></param>
		/// <param name="logger"></param>
		/// <param name="services"></param>
		public RealtTimeNewsJsonSpider(IEventBus mq, IStatisticsService statisticsService, SpiderOptions options,
		ILogger<Spider> logger, IServiceProvider services) : base(mq, statisticsService, options, logger, services)
		{
		}
		/// <summary>
		/// 运行爬虫
		/// </summary>
		/// <returns></returns>
		public static Task Run()
		{
			var builder = new SpiderHostBuilder()
				.ConfigureLogging(x => x.AddSerilog())
				.ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.json"))
				.ConfigureServices(services =>
				{
					services.AddLocalEventBus();
					services.AddLocalDownloadCenter();
					services.AddDownloadAgent(x =>
					{
						x.UseFileLocker();
						x.UseDefaultInternetDetector();
					});
					services.AddStatisticsCenter(x => x.UseMemory());
				}).Register<RealtTimeNewsJsonSpider>();
			var provider = builder.Build();
			var spider = provider.Create<RealtTimeNewsJsonSpider>();
			return spider.RunAsync();
		}
		protected override void Initialize()
		{

			AddDataFlow(new DataParser<RealtTimeNewsJsonEntry>())
				.AddDataFlow(new MySqlEntityStorage(StorageType.InsertIgnoreDuplicate, SpiderOptions.ConnectionString1));

			AddRequests(
				new Request("https://www.toutiao.com/api/pc/realtime_news/", new Dictionary<string, string> { { "网站key", "网站value " } })
				 );
		}
		/*
		 参考jsonPath https://blog.csdn.net/fwk19840301/article/details/80452258
		 https://www.rdtoc.com/tutorial/json-jsonpath-tutorial.html
		 JSON数据如下：
	{
		"message": "success",
		"data": [
			{
				"open_url": "/group/6722220390606701067/",
				"group_id": "6722220390606701067",
				"image_url": "//p9.pstatp.com/list/240x240/pgc-image/fbf255ba8aea455fb49a23ba23c9fdec",
				"title": "台饮品店在香港挺“一国两制”，绿营跳脚"
			},
			{
				"open_url": "/group/6722074975504695819/",
				"group_id": "6722074975504695819",
				"image_url": "//p1.pstatp.com/list/240x240/pgc-image/RYNbl2SBFV1sIb",
				"title": "不管怎么施压都不会得逞，新闻联播用十分钟驳斥美方"
			}
		]
	}	 
	 */


		[Schema("SpiderStorage", "toutiao_news_1")]
		//选择实体类型 JsonPath 支持正则，选择必须在循环元素中内
		[EntitySelect(Expression = "$.data[?(@.open_url =~ /^.*group.*$/i)]", Type = SelectorType.JsonPath)]
		//内容选择，定义选择内容的区域
		[ValueSelect(Expression = "$.data", Name = "name", Type = SelectorType.JsonPath)]
		public class RealtTimeNewsJsonEntry : EntityBase<RealtTimeNewsJsonEntry>
		{
			protected override void Configure()
			{
				HasIndex(x => new { x.WebSite, x.Guid }, true);
			}
			/// <summary>
			/// 索引
			/// </summary>
			public int Id { get; set; }


			[ValueSelect(Expression = "$..title", Type = SelectorType.JsonPath)]
			public string title { get; set; }

			[ValueSelect(Expression = "$..open_url", Type = SelectorType.JsonPath)]
			public string open_url { get; set; }

			[ValueSelect(Expression = "$..group_id", Type = SelectorType.JsonPath)]
			public string group_id { get; set; }

			[ValueSelect(Expression = "$..image_url", Type = SelectorType.JsonPath)]
			[DownloadFormat(FileStorageType = FileStorageType.InternetPath)]
			public string image_url { get; set; }

			[ValueSelect(Expression = "$..image_url", Type = SelectorType.JsonPath)]
			[DownloadFormat(FileStorageType = FileStorageType.LocalFilePath)]
			public string image_local_url { get; set; }


			#region  SelectorType.Enviroment
			/// <summary>
			/// 使用环境，默认类别、限制长度，内容取 当前环境
			/// 去设置ValueSelect中key是{内容key}的Expression的数值
			/// </summary>
			[Required]
			[StringLength(200)]
			[ValueSelect(Expression = "内容key", Type = SelectorType.Enviroment)]
			[TrimFormat(Type = TrimType.All)]
			public string Category { get; set; }

			/// <summary>
			/// 输出的是网站key 的value 在AddRequests定义的key-value
			/// </summary>
			[Required]
			[StringLength(16)]
			[ValueSelect(Expression = "网站key", Type = SelectorType.Enviroment)]
			[TrimFormat(Type = TrimType.RightLeft)]
			public string WebSite { get; set; }

			[StringLength(32)]
			[ValueSelect(Expression = "GUID", Type = SelectorType.Enviroment)]
			public string Guid { get; set; }

			[ValueSelect(Expression = "DATETIME", Type = SelectorType.Enviroment)]
			public DateTime CreationTime { get; set; } 
			#endregion

		}


	}


}