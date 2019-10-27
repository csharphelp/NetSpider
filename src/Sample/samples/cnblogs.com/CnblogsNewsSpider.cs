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
using Sop.DotnetSpider.Analyzer;
using Sop.DotnetSpider.Analyzer.Format;
using Sop.DotnetSpider.Analyzer.Select;

namespace Sample.samples
{
	/// <summary>
	/// 实体信息采集器
	/// 1、https://news.cnblogs.com/n/page/1/
	/// </summary>
	public class CnblogsNewsSpider : Spider
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="mq"></param>
		/// <param name="statisticsService"></param>
		/// <param name="options"></param>
		/// <param name="logger"></param>
		/// <param name="services"></param>
		public CnblogsNewsSpider(IEventBus mq, IStatisticsService statisticsService, SpiderOptions options,
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
				}).Register<CnblogsNewsSpider>();
			var provider = builder.Build();
			var spider = provider.Create<CnblogsNewsSpider>();
			return spider.RunAsync();
		}
		protected override void Initialize()
		{

			AddDataFlow(new DataParser<CnblogsEntry>())
				.AddDataFlow(new MySqlEntityStorage(StorageType.InsertIgnoreDuplicate, SpiderOptions.ConnectionString1));
			#region 注释掉
			//AddRequests(
			//	new Request("https://news.cnblogs.com/n/page/5/", new Dictionary<string, string> {{"网站", "博客园"}}),
			//	new Request("https://news.cnblogs.com/n/page/2/", new Dictionary<string, string> {{"网站", "博客园"}})); 
			#endregion

			AddRequests(
				new Request("https://news.cnblogs.com/n/page/44/", new Dictionary<string, string> { { "网站key", "博客园value " } })
				 );
		}

		[Schema("SpiderStorage", "cnblogs_news_1")]
		[EntitySelect(Expression = ".//div[@class='news_block']", Type = SelectorType.XPath)]
		[ValueSelect(Expression = ".//a[@class='current']", Name = "内容key", Type = SelectorType.XPath)]
		//[FollowSelect(XPaths = new[] { "//div[@class='pager']" })]
		public class CnblogsEntry : EntityBase<CnblogsEntry>
		{
			protected override void Configure()
			{
				HasIndex(x => x.Title);
				HasIndex(x => new { x.WebSite, x.Guid }, true);
			}
			/// <summary>
			/// 索引
			/// </summary>
			public int Id { get; set; }

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
			/// 
			/// </summary>
			[Required]
			[StringLength(200)]
			[ValueSelect(Expression = "网站key", Type = SelectorType.Enviroment)]
			[TrimFormat(Type = TrimType.RightLeft)]
			public string WebSite { get; set; }

			/// <summary>
			/// 抓去标题（先调用选择器，在调用格式发处理器）
			/// </summary>
			[StringLength(200)]
			[ValueSelect(Expression = "//title")]
			[ReplaceFormat(NewValue = "", OldValue = " - 博客园")]
			public string Title { get; set; }

			[StringLength(40)]
			[ValueSelect(Expression = "GUID", Type = SelectorType.Enviroment)]
			public string Guid { get; set; }

			[ValueSelect(Expression = ".//h2[@class='news_entry']/a")]
			public string News { get; set; }

			[ValueSelect(Expression = ".//h2[@class='news_entry']/a/@href")]
			public string Url { get; set; }


			[ValueSelect(Expression = ".//div[@class='entry_summary']/a/img/@src")]
			[DownloadFormat()]
			public string ImgUrl { get; set; }

			[ValueSelect(Expression = ".//div[@class='entry_footer']", ValueOption = ValueOption.OuterHtml)]
			public string FooterHtml { get; set; }

			[ValueSelect(Expression = ".//div[@class='entry_footer']/span[@class='comment']",
				ValueOption = ValueOption.InnerText)]
			public string Comment { get; set; }

			[ValueSelect(Expression = ".//div[@class='entry_footer']/span[@class='view']", ValueOption = ValueOption.InnerText)]
			public string View { get; set; }

			[ValueSelect(Expression = ".//div[@class='entry_footer']/span[@class='view']", ValueOption = ValueOption.InnerText)]
			[RegexReplaceFormat(Expression = @"[^0-9]")]
			public string ViewCount { get; set; }



			[ValueSelect(Expression = ".//div[@class='entry_summary']", ValueOption = ValueOption.OuterHtml)]
			[RegexReplaceFormat(Expression = @"[^\u4E00-\u9FA5]")]
			public string PlainText { get; set; }

			[ValueSelect(Expression = "DATETIME", Type = SelectorType.Enviroment)]
			public DateTime CreationTime { get; set; }
		}


	}


}