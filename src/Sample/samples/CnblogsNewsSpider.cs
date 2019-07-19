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
	/// </summary>
	public class CnblogsNewsSpider : Spider
	{
		/*
		 * 0 此类爬虫存储抓取网页存储数据库
		 * 1 Depth 有什么影响 
		 *  2 Speed 有什么作用 
		 *  3 FollowSelectAttribute 这个属性为啥有时候有效 有时候无效在哪里使用
	    */


		/// <summary>
		/// 
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
		/// 
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
					services.AddDownloaderAgent(x =>
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
			//DownloadFile = DownloadFile.ImgAgeDownLoad;
			AddDataFlow(new DataParser<CnblogsEntry>()).
			AddDataFlow(new MySqlEntityStorage(StorageType.InsertIgnoreDuplicate, "Database='Sop.Spider';Data Source=127.0.0.1;password=123456;User ID=root;Port=3306;"));
			//AddRequests(
			//	new Request("https://news.cnblogs.com/n/page/5/", new Dictionary<string, string> {{"网站", "博客园"}}),
			//	new Request("https://news.cnblogs.com/n/page/2/", new Dictionary<string, string> {{"网站", "博客园"}}));

			AddRequests(
				new Request("https://news.cnblogs.com/n/page/22/", new Dictionary<string, string> { { "网站sop", "博客园sop" } })
				 );
		}

		[Schema("cnblogs", "cnblogs_entity_model_1")]
		[EntitySelect(Expression = ".//div[@class='news_block']",Type = SelectorType.XPath)]
		[ValueSelect(Expression = ".//a[@class='current']", Name = "内容", Type = SelectorType.XPath)]
		//[FollowSelect(XPaths = new[] { "//div[@class='pager']" })]
		public class CnblogsEntry : EntityBase<CnblogsEntry>
		{
			protected override void Configure()
			{

				HasIndex(x => x.Title);
				HasIndex(x => new { x.WebSite, x.Guid }, true);
			}

			public int Id { get; set; }

			[Required]
			[StringLength(200)]
			[ValueSelect(Expression = "类别", Type = SelectorType.Enviroment)]
			public string Category { get; set; }

			[Required]
			[StringLength(200)]
			[ValueSelect(Expression = "网站sop", Type = SelectorType.Enviroment)]

			public string WebSite { get; set; }

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



			[ValueSelect(Expression = ".//div[@class='entry_footer']", ValueOption = ValueOption.OuterHtml)]
			public string FooterHtml { get; set; }




         
			[ValueSelect(Expression = ".//div[@class='entry_footer']/span[@class='comment']",
				ValueOption = ValueOption.InnerText)]
			public string Comment { get; set; }




			[ValueSelect(Expression = ".//div[@class='entry_footer']/span[@class='view']", ValueOption = ValueOption.InnerText)]
			public string View { get; set; }


			[ValueSelect(Expression = ".//div[@class='entry_summary']", ValueOption = ValueOption.InnerText)]
			public string PlainText { get; set; }

			[ValueSelect(Expression = "DATETIME", Type = SelectorType.Enviroment)]
			public DateTime CreationTime { get; set; }
		}


	}


}