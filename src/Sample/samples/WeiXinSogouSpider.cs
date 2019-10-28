using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Sop.DotnetSpider;
using Sop.DotnetSpider.Analyzer;
using Sop.DotnetSpider.Analyzer.Format;
using Sop.DotnetSpider.Analyzer.Select;
using Sop.DotnetSpider.Common;
using Sop.DotnetSpider.DataStorage;
using Sop.DotnetSpider.Download;
using Sop.DotnetSpider.EventBus;
using Sop.DotnetSpider.Statistics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.samples
{
	/// <summary>
	/// 实体信息采集器
	/// 1、简单的采集器，此类采集一般对新闻网站
	/// 2、采集不会出现验证码、反爬虫策略
	/// </summary>
	public class WeiXinSoGouSpider : Spider
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="mq"></param>
		/// <param name="statisticsService"></param>
		/// <param name="options"></param>
		/// <param name="logger"></param>
		/// <param name="services"></param>
		public WeiXinSoGouSpider(IEventBus mq, IStatisticsService statisticsService, SpiderOptions options,
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
					//这里也使用本地事件注入，目前不需要分布式消息队列Kafka
					services.AddLocalEventBus();
					services.AddLocalDownloadCenter();
					services.AddDownloadAgent(x =>
					{
						x.UseFileLocker();
						x.UseDefaultInternetDetector();
					});
					services.AddStatisticsCenter(x => x.UseMemory());
				}).Register<WeiXinSoGouSpider>();
			var provider = builder.Build();
			var spider = provider.Create<WeiXinSoGouSpider>();
			return spider.RunAsync();
		}
		protected override void Initialize()
		{

			 
			AddDataFlow(new DataParser<WeiXinSoGouEntry>())
				.AddDataFlow(new MySqlEntityStorage(StorageType.InsertIgnoreDuplicate, SpiderOptions.ConnectionString1));
		  
			//采集地址
			AddRequests(
				new Request("https://weixin.sogou.com/weixin?type=1&s_from=input&query=税收&ie=utf8&_sug_=n&_sug_type_=", new Dictionary<string, string> { { "网站key", "博客园value " } })
				 );
		} 
		[Schema("SpiderStorage", "weixinsogou_entity_model_5", TablePostfix.Today)]
		[EntitySelect(Expression = ".//ul[@class='news-list2']/li", Type = SelectorType.XPath)]
		//[ValueSelect(Expression = ".//a[@class='current']", Name = "内容key", Type = SelectorType.XPath)]
		//[FollowSelect(XPaths = new[] { "//div[@class='pager']" })]
		public class WeiXinSoGouEntry : EntityBase<WeiXinSoGouEntry>
		{
			protected override void Configure()
			{
				HasIndex(x => x.Id);
			}
		 


			/// <summary>
			/// 索引
			/// </summary>
			public int Id { get; set; }

			/// <summary>
			/// 微信公总号头像
			/// </summary>
			[ValueSelect(Expression = ".//div[@class='img-box']/a/img/@src")]
			public string ImgBox { get; set; }

			[ValueSelect(Expression = ".//div[@class='img-box']/a/img/@src")]
			[DownloadFormat()]
			public string ImgUrl { get; set; }

			/// <summary>
			/// 微信公总号名称
			/// </summary>
			[ValueSelect(Expression = ".//div[@class='txt-box']/p[@class='tit']/a/", ValueOption = ValueOption.InnerText)]
			public string Name { get; set; }

			/// <summary>
			/// 微信公总号名称
			/// </summary>
			[ValueSelect(Expression = ".//div[@class='txt-box']/p[@class='info']/label/", ValueOption = ValueOption.InnerText)]
			public string WeixinHao { get; set; }

			/// <summary>
			/// 功能介绍：
			/// </summary>
			[ValueSelect(Expression = ".//dl[1]", ValueOption = ValueOption.OuterHtml)]
			[RegexReplaceFormat(Expression = @"[^\u4E00-\u9FA5]")]
			public string GongNengText { get; set; }

			/// <summary>
			/// 最近文章
			/// </summary>
			[ValueSelect(Expression = ".//dl[2]", ValueOption = ValueOption.OuterHtml)]
			[RegexReplaceFormat(Expression = @"[^\u4E00-\u9FA5]")]
			public string NewPostText { get; set; }

			[ValueSelect(Expression = "DATETIME", Type = SelectorType.Enviroment)]
			public DateTime CreationTime { get; set; }
		}


	}


}