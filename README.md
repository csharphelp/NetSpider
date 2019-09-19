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


### 更多文档

**完成度：0%**

https://github.com/csharphelp/DotnetSpider/wiki

### 案例
最好的说明文件就是看demo,
[请查看 Sample 详细案例 ](https://github.com/csharphelp/DotnetSpider/tree/master/src/Sample)
  
 
### 其他

QQ Group: 721420150(10个人的垃圾小群，人员极少)

Email: sopcce@qq.com
