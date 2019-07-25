# 1设计介绍

>  Sop.Spider，一个c# .NET标准网络爬行库。 它是轻量级，高效且快速的高级Web爬行和抓取框架,此项目是[DotnetSpider](https://github.com/dotnetcore/DotnetSpider)Fork的，因原始项目无法满足现有需求，随创建本新项目。并且修改默认命名空间、用于项目统一

# 使用介绍

# 一 框架介绍
 1. 基类为 Spider.cs[  没有改变，还是使用作者的- ]，默认所有爬虫继承强制此类
 2. 提供数据解析接口Analyzer(原DotnetSpider中的请对应名称，文件名称为改变升级）推荐使用实体处理器，选选择后格式的操作
  - 选择处理器：Select（提供了内容选择器、目标链接 **原作者应该为了实现跟随循环链接采集,暂时这么叫，原则我想对这块实现js 操作，但是能力有限**、实体选择器）
  - 选择处理器：Selector（提供了XPath、正则Regex、Css、JsonPath、Enviroment **环境实现就是对本地环境字符串的替换，详细见DataParser`.cs文件中的环境替换**）
  - 格式化处理器：Format（提供了诸多格式化处理方法）
  - 数据解析器：DataParser（提供了数据解析存储操作的处理器。主要是对实体解析、建库**Storage相关的文件**、建表、添加数据的解析）
  - 下载处理器：Download（** 对下载中心等模块目前还没有研究 ** 提供下载服务，主要使用IDownloaded.cs 接口文件等）
  - 网络中心:Network 只保留网络判断，其他移除
  - 计数统计、URL去重等：Statistics、Scheduler 暂时保留原来，有改动需求。目前保持原来
  ** 其他非主要的文件暂时不做介绍，这里没有使用或者 **
# 二 使用介绍
```
namespace Sample.samples
{
	/// <summary>
	/// 实体信息采集器
	/// </summary>
	public class CnblogsNewsSpider : Spider //必须继承此接口
	{
		 
		/// <summary>
		/// 构造函数 //提供事件注入
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
		    //注入服务
			var builder = new SpiderHostBuilder()
				.ConfigureLogging(x => x.AddSerilog()) //注入日志服务
				.ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.json"))
				.ConfigureServices(services =>
				{
				    //注册本地事件、下载器、文件锁、默认网络检查、计数服务（使用内存或者MYSQL）后期可能扩展redis
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
		//初始化 源框架必须对OwnerId 在本初始化是
			 
			AddDataFlow(new DataParser<CnblogsEntry>()).
			AddDataFlow(new MySqlEntityStorage(StorageType.InsertIgnoreDuplicate, "Database='Sop.Spider';Data Source=127.0.0.1;password=123456;User ID=root;Port=3306;"));
			//AddRequests(
			//	new Request("https://news.cnblogs.com/n/page/5/", new Dictionary<string, string> {{"网站", "博客园"}}),
			//	new Request("https://news.cnblogs.com/n/page/2/", new Dictionary<string, string> {{"网站", "博客园"}}));

			AddRequests(
				new Request("https://news.cnblogs.com/n/page/22/", new Dictionary<string, string> { { "网站key", "博客园value " } })
				 );
		}

		[Schema("cnblogs", "cnblogs_entity_model_2", TablePostfix.Today)]
		[EntitySelect(Expression = ".//div[@class='news_block']", Type = SelectorType.XPath)]
		[ValueSelect(Expression = ".//a[@class='current']", Name = "内容key", Type = SelectorType.XPath)]
		[FollowSelect(XPaths = new[] { "//div[@class='pager']" })]
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
			[RegexReplaceFormat(Expression = @"^[1 - 9]\d *$")]
			public string View { get; set; }


			[ValueSelect(Expression = ".//div[@class='entry_summary']", ValueOption = ValueOption.InnerText)]
			public string PlainText { get; set; }

			[ValueSelect(Expression = "DATETIME", Type = SelectorType.Enviroment)]
			public DateTime CreationTime { get; set; }
		}


	}


}


```


 