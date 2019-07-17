# Sop.Spider(请查看需求和修改，根据自己需要入手)

[![Build Status](https://dev.azure.com/zlzforever/DotnetSpider/_apis/build/status/dotnetcore.DotnetSpider?branchName=master)](https://dev.azure.com/zlzforever/DotnetSpider/_build/latest?definitionId=3&branchName=master)

[![NuGet](https://img.shields.io/nuget/vpre/DotnetSpider.svg)](https://www.nuget.org/packages/Sop.Spider)
[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/csharphelp)
[![GitHub license](https://img.shields.io/github/license/dotnetcore/DotnetSpider.svg)](https://raw.githubusercontent.com/dotnetcore/DotnetSpider/master/LICENSE)


### 介绍

Sop.Spider，一个c# .NET标准网络爬行库。 它是轻量级，高效且快速的高级Web爬行和抓取框架

### 修改

1. 此版本依据原有项目扩展，目前不添加测试用例、目前以中文案例为准，详细参考中文案例
[Sample项目]
2. 移除docker，容器化是趋势不是必须，目前支持部署在windows+centos为主，移除docker的支持
3. 

### 需求

1. 代码复用，功能模块化
2. 易扩展（下载规则、解析规则、入库规则、请求操作规则）
3. 健壮性、可维护性。报错的概率更大，例如断网、中途被防爬、爬到“脏数据”
4. 分布式。多网站抓取。Redis消息队列同步、深度优先、广度优先出重URL
5. 支持异步、多线程、支持配置、支持批量写入、支持操作网页

- User-Agent：主要用来将我们的爬虫伪装成浏览器。
- Cookie：主要用来保存爬虫的登录状态。
- 连接数：主要用来限制单台机器与服务端的连接数量。
- 代理IP：主要用来伪装请求地址，提高单机并发数量。
----
### 设计架构

![DESIGN IMAGE](https://github.com/dotnetcore/DotnetSpider/blob/master/images/%E6%95%B0%E6%8D%AE%E9%87%87%E9%9B%86%E7%B3%BB%E7%BB%9F.png?raw=true)

### 开发环境

1. Visual Studio 2017 (15.3 or later) 
2. [.NET Core 2.2 or later](https://www.microsoft.com/net/download/windows)
3. MySql
4. Redis (option)
5. SqlServer(option)
6. PostgreSQL (option)
7. MongoDb  (option)
8.  Kafka (option)
### MORE DOCUMENTS

https://github.com/dotnetcore/DotnetSpider/wiki

### SAMPLES

    Please see the Projet DotnetSpider.Sample in the solution.

### BASE USAGE

[Base usage Codes](https://github.com/zlzforever/DotnetSpider/blob/master/src/DotnetSpider.Sample/samples/BaseUsage.cs)

### ADDITIONAL USAGE: Configurable Entity Spider

[View complete Codes](https://github.com/zlzforever/DotnetSpider/blob/master/src/DotnetSpider.Sample/samples/EntitySpider.cs)

    public class EntitySpider : Spider
    {
        public static void Run()
        {
            var builder = new SpiderBuilder();
            builder.AddSerilog();
            builder.ConfigureAppConfiguration();
            builder.UseStandalone();
            builder.AddSpider<EntitySpider>();
            var provider = builder.Build();
            provider.Create<EntitySpider>().RunAsync();
        }

        protected override void Initialize()
        {
            NewGuidId();
            Scheduler = new QueueDistinctBfsScheduler();
            Speed = 1;
            Depth = 3;
            DownloaderSettings.Type = DownloaderType.HttpClient;
            AddDataFlow(new DataParser<BaiduSearchEntry>()).AddDataFlow(GetDefaultStorage());
            AddRequests(
                new Request("https://news.cnblogs.com/n/page/1/", new Dictionary<string, string> {{"网站", "博客园"}}),
                new Request("https://news.cnblogs.com/n/page/2/", new Dictionary<string, string> {{"网站", "博客园"}}));
        }

        [Schema("cnblogs", "cnblogs_entity_model")]
        [EntitySelector(Expression = ".//div[@class='news_block']", Type = SelectorType.XPath)]
        [ValueSelector(Expression = ".//a[@class='current']", Name = "类别", Type = SelectorType.XPath)]
        class BaiduSearchEntry : EntityBase<BaiduSearchEntry>
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

        public EntitySpider(IMessageQueue mq, IStatisticsService statisticsService, ISpiderOptions options, ILogger<Spider> logger, IServiceProvider services) : base(mq, statisticsService, options, logger, services)
        {
        }
    }

#### Distributed spider
     
##### start 2 agent

    $ cd src/DotnetSpider.DownloaderAgent
    $ sh build.sh
    $ mkdir -p /Users/lewis/dotnetspider/logs/agent
    $ docker run --name agent001 -d --restart always -e "DOTNET_SPIDER_AGENTID=agent001" -e "DOTNET_SPIDER_AGENTNAME=agent001" -e "DOTNET_SPIDER_KAFKACONSUMERGROUP=Agent" -v /Users/lewis/dotnetspider/agent/logs:/logs registry.zousong.com:5000/dotnetspider/downloader-agent:latest
    $ docker run --name agent002 -d --restart always -e "DOTNET_SPIDER_AGENTID=agent002" -e "DOTNET_SPIDER_AGENTNAME=agent002" -e "DOTNET_SPIDER_KAFKACONSUMERGROUP=Agent" -v /Users/lewis/dotnetspider/agent/logs:/logs registry.zousong.com:5000/dotnetspider/downloader-agent:latest
    
#### start portal

    $ mkdir -p /Users/lewis/dotnetspider/portal & cd /Users/lewis/dotnetspider/portal
    $ curl https://raw.githubusercontent.com/dotnetcore/DotnetSpider/master/src/DotnetSpider.Portal/appsettings.json -O
    $ docker run --name dotnetspider.portal -d --restart always -p 7897:7896 -v /Users/lewis/dotnetspider/portal/appsettings.json:/portal/appsettings.json -v /Users/lewis/dotnetspider/portal/logs:/logs registry.zousong.com:5000/dotnetspider/portal:latest

#### WebDriver Support

When you want to collect a page JS loaded, there is only one thing to do, set the downloader to WebDriverDownloader.

    Downloader = new WebDriverDownloader(Browser.Chrome);

[See a complete sample](https://github.com/zlzforever/DotnetSpider/)

NOTE:

1.  Make sure the ChromeDriver.exe is in bin folder when use Chrome, install it to your project from NUGET: Chromium.ChromeDriver
2.  Make sure you already add a \*.webdriver Firefox profile when use Firefox: https://support.mozilla.org/en-US/kb/profile-manager-create-and-remove-firefox-profiles
3.  Make sure the PhantomJS.exe is in bin folder when use PhantomJS, install it to your project from NUGET: PhantomJS

### NOTICE

#### when you use redis scheduler, please update your redis config:

    timeout 0
    tcp-keepalive 60

### AREAS FOR IMPROVEMENTS

QQ Group: 721420150(2人群，人员极少，不建议加入)
Email: sopcce@qq.com
