# Sop.Spider

### (请查看需求和修改，根据自己需要入手)

[![Build Status](https://dev.azure.com/zlzforever/DotnetSpider/_apis/build/status/dotnetcore.DotnetSpider?branchName=master)](https://dev.azure.com/zlzforever/DotnetSpider/_build/latest?definitionId=3&branchName=master)

[![NuGet](https://img.shields.io/nuget/vpre/Sop.Spider.svg)](https://www.nuget.org/packages/Sop.Spider)
[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/csharphelp)

[![GitHub license](https://img.shields.io/github/license/dotnetcore/DotnetSpider.svg)](https://raw.githubusercontent.com/dotnetcore/DotnetSpider/master/LICENSE)


### 介绍

Sop.Spider，一个c# .NET标准网络爬行库。 它是轻量级，高效且快速的高级Web爬行和抓取框架,
此项目是[DotnetSpider](https://github.com/dotnetcore/DotnetSpider)Fork的，因原始项目
无法满足现有需求，随创建本新项目。
### 现阶段任务

1. 增加redis 计数器
2. 移除网络中心ADSL拨号功能
3. 增加了Sellenium 支持AJAX.（设想）
4. 优化下载器，对文件url解析处理（设想）。


### 修改

1. 此版本依据原有项目扩展，目前不添加测试用例、目前以中文案例为准，详细参考中文案例[Sample项目](https://github.com/csharphelp/DotnetSpider/tree/master/src/Sample)
2. 移除docker强依赖配置，容器化是趋势不是必须，目前支持部署在windows+centos为主，移除docker的支持
3. 同步更新源：https://github.com/dotnetcore/DotnetSpider 根据变动适当调整更新，原始项目设计思路、设计方式等同步更新

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

![DESIGN IMAGE](https://raw.githubusercontent.com/csharphelp/DotnetSpider/master/images/data-info-sys.png)

### 开发环境

1. 开发工具：Visual Studio 2017 (15.3 or later) 
2. [.NET Core 2.2 or later](https://www.microsoft.com/net/download/windows) 必要条件 支持NET FRAMEWORK
3. MySql (非必需)
4. Redis (非必需)(推荐配置使用)
5. SqlServer(非必需)
6. PostgreSQL (非必需)
7. MongoDb  (非必需)
8. Kafka   (非必需)


## 一 框架介绍(后期修改)
 1. 基类为 Spider.cs[  没有改变，还是使用作者的- ]，默认所有爬虫继承强制此类
 2. 提供数据解析接口Analyzer(原DotnetSpider中的请对应名称，文件名称为改变升级）推荐使用实体处理器，选选择后格式的操作
  - 选择处理器：Select（提供了内容选择器、目标链接 **原作者应该为了实现跟随循环链接采集,暂时这么叫，原则我想对这块实现js 操作，但是能力有限**、实体选择器）
  - 选择处理器：Selector（提供了XPath、正则Regex、Css、JsonPath、Enviroment **环境实现就是对本地环境字符串的替换，详细见DataParser`.cs文件中的环境替换**）
  - 格式化处理器：Format（提供了诸多格式化处理方法）
  - 数据解析器：DataParser（提供了数据解析存储操作的处理器。主要是对实体解析、建库**Storage相关的文件**、建表、添加数据的解析）
  - 下载处理器：Download（** 对下载中心等模块目前还没有研究 ** 提供下载服务，主要使用IDownloaded.cs 接口文件等）
  - 网络中心:Network 只保留网络判断，其他移除
  - 计数统计、URL去重等：Statistics、Scheduler 暂时保留原来，有改动需求。目前保持原来

  ** 其他非主要的文件暂时不做介绍，这里没有使用 **
## 结构介绍


```text
Sop.Spider/
├── Analyzer/[数据解析器]
    ├── Format/[数据格式化解析器]
	├── HtmlAgilityPack.Css/[HtmlAgilityPack选择处理器]
	├── Select/[爬虫实体解析器]
	└── Selector/[实体选择筛选器]
        ├── DataParser.cs 
        ├── DataParser`.cs
        ├── DataParserBase.cs
        ├── DataParserHelper.cs
        ├── IParseResult.cs
        ├── Model.cs
        ├── ParseResult.cs
        └── SelectorExtensions.cs
├── Common/
├── DataStorage/
├── Download/
├── DownloadAgent/
├── DownloadAgentRegisterCenter/
├── EventBus/
├── Extensions/
├── Network/
├── DataStorage/
├── RequestSupplier/
├── DataStorage/
├── Scheduler/
├── Statistics/
	├── HtmlAgilityPack.Css/[HtmlAgilityPack选择处理器]
	├── Select/[爬虫实体解析器]
	├── Selector/[实体选择筛选器]
    └── js/
        ├── bootstrap.bundle.js
        ├── bootstrap.bundle.js.map
        └── bootstrap.min.js.map
├── Spider.cs
├── readme.md
├── readme.md
├── readme.md
├── readme.md
├── readme.md
├── readme.md
├── readme.md
├── readme.md
├── readme.md
├── readme.md
└── Status.cs


```

### 更多文档

**完成度：0%**

https://github.com/csharphelp/DotnetSpider/wiki

### 案例
最好的说明文件就是看demo,
[请查看 Sample 详细案例 ](https://github.com/csharphelp/DotnetSpider/tree/master/src/Sample)
  
 
### 其他

QQ Group: 721420150(10个人的垃圾小群，人员极少)

Email: sopcce@qq.com
