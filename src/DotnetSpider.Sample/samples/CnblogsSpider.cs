using System;
using System.Threading.Tasks;
using Sop.Spider.Common;
using Sop.Spider.DataFlow;
using Sop.Spider.DataFlow.Parser;
using Sop.Spider.DataFlow.Storage;
using Sop.Spider.EventBus;
using Sop.Spider.Scheduler;
using Sop.Spider.Statistics;
using Microsoft.Extensions.Logging;

namespace Spider.Sample.samples
{
    public class CnblogsSpider : Sop.Spider.Spider
	{
        protected override void Initialize()
        {		
            NewGuidId();
            Scheduler = new QueueDistinctBfsScheduler();
            Speed = 1;
            Depth = 3;
            AddDataFlow(new CnblogsDataParser()).AddDataFlow(new JsonFileStorage());
            AddRequests("http://www.cnblogs.com/");
        }

        class CnblogsDataParser : DataParser
        {
            public CnblogsDataParser()
            {
                Required = DataParserHelper.CheckIfRequiredByRegex("cnblogs\\.com");
                GetFollowRequests = DataParserHelper.QueryFollowRequestsByXPath(".");
            }

            protected override Task<DataFlowResult> Parse(DataFlowContext context)
            {
                context.AddItem("URL", context.Response.Request.Url);
                context.AddItem("Title", context.GetSelectable().XPath(".//title").GetValue());
                return Task.FromResult(DataFlowResult.Success);
            }
        }

        public CnblogsSpider(IEventBus mq, IStatisticsService statisticsService, SpiderOptions options, ILogger<Sop.Spider.Spider> logger, IServiceProvider services) : base(mq, statisticsService, options, logger, services)
        {
        }
    }
}