# 项目介绍

# 设计介绍

>  Sop.Spider，一个c# .NET标准网络爬行库。 它是轻量级，高效且快速的高级Web爬行和抓取框架,此项目是[DotnetSpider](https://github.com/dotnetcore/DotnetSpider)Fork的，因原始项目无法满足现有需求，随创建本新项目。并且修改默认命名空间、用于项目统一，这里可以根据你项目的命名空间随意替换[Sop]

# 使用介绍

## 一 框架介绍
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

|**序号**|**文件、类名称**|**介绍说明**|**测试用例** | **备注**     |
|:--:  |:-----    | :---|:----  |-------  |
|**1** |Spider.cs      |主要爬虫基类 |  10%   |无 |   
|**2** |Analyzer     |30%   |  50%   |待测试   |    
|**3** |ImgManager  |50%   |  50%   |待测试   |   
|**4** |ImgOcr      |5%    |  50%   |待测试   |   
|**5** |ImgExif     |100%   |  50%   |基本完成   |   
|**6** |ImgWaterMark|80%   |  70%   | 文字平铺水印、混合水印待测试开发|
|**7** |ImageAve    |100%  |  90%   |基本完成 |   
|**8** |ImgAnimate  |100%  |  90%   |基本完成 |   