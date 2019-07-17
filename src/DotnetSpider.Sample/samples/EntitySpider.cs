using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Sop.Spider;
using Sop.Spider.Common;
using Sop.Spider.DataFlow.Parser;
using Sop.Spider.DataFlow.Parser.Attribute;
using Sop.Spider.DataFlow.Parser.Formatter;
using Sop.Spider.DataFlow.Storage;
using Sop.Spider.DataFlow.Storage.Model;
using Sop.Spider.Downloader;
using Sop.Spider.EventBus;
using Sop.Spider.Scheduler;
using Sop.Spider.Selector;
using Sop.Spider.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Sample.samples
{
	public class EntityModelSpider : Spider
	{
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
						x.UseDefaultAdslRedialer();
						x.UseDefaultInternetDetector();
					});
					services.AddStatisticsCenter(x => x.UseMemory());
				}).Register<EntityModelSpider>();
			var provider = builder.Build();
			var spider = provider.Create<EntityModelSpider>();
			return spider.RunAsync();
		}


		protected override void Initialize()
		{
			NewGuidId();
			Scheduler = new QueueDistinctBfsScheduler();
			Speed = 1;
			Depth = 3;
			AddDataFlow(new DataParser<CnblogsEntry>())
				.AddDataFlow(new MySqlEntityStorage(StorageType.InsertIgnoreDuplicate, "Database='Sop.Spider';Data Source=127.0.0.1;password=123456;User ID=root;Port=3306;"));
			//AddRequests(
			//	new Request("https://news.cnblogs.com/n/page/5/", new Dictionary<string, string> {{"网站", "博客园"}}),
			//	new Request("https://news.cnblogs.com/n/page/2/", new Dictionary<string, string> {{"网站", "博客园"}}));

			AddRequests(
				new Request("https://news.cnblogs.com/n/page/22/", new Dictionary<string, string> { { "网站sop", "博客园sop" } })
				 );
		}

		[Schema("cnblogs", "cnblogs_entity_model_2")]
		[EntitySelector(Expression = ".//div[@class='news_block']", Type = SelectorType.XPath)]
		[ValueSelector(Expression = ".//a[@class='current']", Name = "类别", Type = SelectorType.XPath)]
		[FollowSelector(XPaths = new[] {"//div[@class='pager']"})]
		public class CnblogsEntry : EntityBase<CnblogsEntry>
		{
			protected override void Configure()
			{
				HasIndex(x => x.Title);
				HasIndex(x => new {x.WebSite, x.Guid}, true);
			}

			public int Id { get; set; }

			[Required]
			[StringLength(200)]
			[ValueSelector(Expression = "类别", Type = SelectorType.Enviroment)]
			public string Category { get; set; }

			[Required]
			[StringLength(200)]
			[ValueSelector(Expression = "网站", Type = SelectorType.Enviroment)]
			public string WebSite { get; set; }

			[StringLength(200)]
			[ValueSelector(Expression = "//title")]
			[ReplaceFormatter(NewValue = "", OldValue = " - 博客园")]
			public string Title { get; set; }

			[StringLength(40)]
			[ValueSelector(Expression = "GUID", Type = SelectorType.Enviroment)]
			public string Guid { get; set; }

			[ValueSelector(Expression = ".//h2[@class='news_entry']/a")]
			public string News { get; set; }

			[ValueSelector(Expression = ".//h2[@class='news_entry']/a/@href")]
			public string Url { get; set; }

			[ValueSelector(Expression = ".//div[@class='entry_summary']", ValueOption = ValueOption.InnerText)]
			public string PlainText { get; set; }

			[ValueSelector(Expression = "DATETIME", Type = SelectorType.Enviroment)]
			public DateTime CreationTime { get; set; }
		}




		public EntityModelSpider(IEventBus mq, IStatisticsService statisticsService, SpiderOptions options,
			ILogger<Spider> logger, IServiceProvider services) : base(mq, statisticsService, options, logger, services)
		{
		}
	}

	 
}