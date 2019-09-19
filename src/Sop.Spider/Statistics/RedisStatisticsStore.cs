using Dapper;
using MySql.Data.MySqlClient;
using Sop.Spider.Common;
using Sop.Spider.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sop.Spider.Statistics
{
	/// <summary>
	/// TODO： 没有充分测试 
	/// </summary>
	public class RedisStatisticsStore : 

	{
		private readonly string Count = "Count";
		private readonly string Down = "Download";

		private readonly string LastTime = "LastTime";
		private readonly string Time = "Time";

		private readonly long _time;
		private readonly SpiderOptions _options;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		public RedisStatisticsStore(SpiderOptions options)
		{
			_time = (long)DateTimeHelper.GetCurrentUnixMilliseconds();

			_options = options;
		}

		/// <summary>
		/// 创建库 （redis 不用创建，保证链接就OK）
		/// </summary>
		/// <returns></returns>
		public Task EnsureDatabaseAndTableCreatedAsync()
		{
			return Task.CompletedTask;
		}
		/// <summary>
		/// 增量缓存计数
		/// </summary>
		/// <param name="countType">计数类型</param>
		/// <param name="tenantType">租户类型</param>
		/// <param name="tenantId">租户ID</param>
		/// <param name="value">增量数</param>
		/// <returns>增量后数，如果增量数为0时，返回查询的增量数</returns>

		private long SetRedisQueue(string countType, string tenantType, string tenantId, long value = 0)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			var newValue = redisQueue.GetCount(countType, tenantType, tenantId);
			value = redisQueue.ChangeCount(countType, tenantType, tenantId, value);
			newValue = newValue == 0 ? value : newValue;
			return newValue;
		}

		#region SpiderStatistics
		/// <summary>
		/// 计数总数
		/// </summary>
		/// <param name="ownerId"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public Task IncrementTotalAsync(string ownerId, int count)
		{
			//INSERT INTO sopspider.spider_statistics (owner_id, total) VALUES (@OwnerId, @Count) ON DUPLICATE key UPDATE total = total + @Count, last_modification_time = CURRENT_TIMESTAMP;
			//记录总请求数
			SetRedisQueue(ownerId, Count, SpiderOptions.Total, count);

			return Task.CompletedTask;
		}
		/// <summary>
		/// 成功计数
		/// </summary>
		/// <param name="ownerId"></param>
		/// <returns></returns>
		public Task IncrementSuccessAsync(string ownerId)
		{
			//INSERT INTO sopspider.spider_statistics (owner_id) VALUES (@OwnerId) ON DUPLICATE key UPDATE success = success + 1, last_modification_time = CURRENT_TIMESTAMP;
			//记录成功总数、成功时间、最后更新时间
			var count = SetRedisQueue(ownerId, Count, SpiderOptions.Success);
			SetRedisQueue(ownerId, Count, SpiderOptions.Success, count + 1);
			return Task.CompletedTask;

		}
		/// <summary>
		/// 失败计数
		/// </summary>
		/// <param name="ownerId"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public Task IncrementFailedAsync(string ownerId, int count)
		{
			//INSERT INTO sopspider.spider_statistics (owner_id) VALUES (@OwnerId) ON DUPLICATE key UPDATE failed = failed + 1, last_modification_time = CURRENT_TIMESTAMP;
			//修改失败总数、成功时间、最后更新时间
			SetRedisQueue(ownerId, Count, SpiderOptions.Failed, count);
			SetRedisQueue(ownerId, Count, SpiderOptions.Failed + Time, _time);
			return Task.CompletedTask;

		}
		/// <summary>
		/// 开始计数
		/// </summary>
		/// <param name="ownerId"></param>
		/// <returns></returns>
		public Task StartAsync(string ownerId)
		{
			//INSERT INTO sopspider.spider_statistics (owner_id, start) VALUES (@OwnerId, @Start) ON DUPLICATE key UPDATE start = @Start, last_modification_time = CURRENT_TIMESTAMP;
			SetRedisQueue(ownerId, Count, SpiderOptions.Start, 1);
			SetRedisQueue(ownerId, Count, SpiderOptions.Start + Time, _time);
			return Task.CompletedTask;

		}
		/// <summary>
		/// 退出
		/// </summary>
		/// <param name="ownerId"></param>
		/// <returns></returns>
		public Task ExitAsync(string ownerId)
		{//INSERT INTO sopspider.spider_statistics (owner_id, `exit`) VALUES (@OwnerId, @Exit) ON DUPLICATE key UPDATE `exit` = @Exit, last_modification_time = CURRENT_TIMESTAMP;

			SetRedisQueue(ownerId, Count, SpiderOptions.Exit, 1);
			SetRedisQueue(ownerId, Count, SpiderOptions.Exit + Time, _time);
			return Task.CompletedTask;

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownerId"></param>
		/// <returns></returns>
		public Task<SpiderStatistics> GetSpiderStatisticsAsync(string ownerId)
		{


			var Total = SetRedisQueue(ownerId, Count, SpiderOptions.Total);
			var Failed = SetRedisQueue(ownerId, Count, SpiderOptions.Failed);
			var Success = SetRedisQueue(ownerId, Count, SpiderOptions.Success);

			var ExitTime = SetRedisQueue(ownerId, Count, SpiderOptions.Exit + Time);
			var StartTime = SetRedisQueue(ownerId, Count, SpiderOptions.Start + Time);

			var info = new SpiderStatistics()
			{
				OwnerId = ownerId,
				Total = Total,
				Failed = Failed,
				Success = Success,
				Exit = DateTimeHelper.GetCurrentUnixMilliseconds(ExitTime),
				Start = DateTimeHelper.GetCurrentUnixMilliseconds(StartTime),
				LastModificationTime = DateTimeHelper.GetCurrentUnixMilliseconds(ExitTime),
			};
			return Task.FromResult<SpiderStatistics>(info);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="page"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public Task<IEnumerable<SpiderStatistics>> GetSpiderStatisticsListAsync(int page, int size)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);

			var list = redisQueue.Gets<SpiderStatistics>("Count");

			list = list.Skip((page - 1) * size).Take(size).ToList();

			return Task.FromResult<IEnumerable<SpiderStatistics>>(list);
		}
		#endregion



		#region Download
		/// <summary>
		/// 增量下载成功
		/// </summary>
		/// <param name="agentId"></param>
		/// <param name="count"></param>
		/// <param name="elapsedMilliseconds"></param>
		/// <returns></returns>
		public Task IncrementDownloadSuccessAsync(string agentId, int count, long elapsedMilliseconds)
		{
			SetRedisQueue(agentId, Down, SpiderOptions.DownloadSuccess, count);
			SetRedisQueue(agentId, Down, SpiderOptions.DownloadSuccess + Time, elapsedMilliseconds);

			return Task.CompletedTask;

		}
		/// <summary>
		/// 增量下载失败
		/// </summary>
		/// <param name="agentId"></param>
		/// <param name="count"></param>
		/// <param name="elapsedMilliseconds"></param>
		/// <returns></returns>
		public Task IncrementDownloadFailedAsync(string agentId, int count, long elapsedMilliseconds)
		{
			SetRedisQueue(agentId, Down, SpiderOptions.DownloadFailed, count);
			SetRedisQueue(agentId, Down, SpiderOptions.DownloadFailed + Time, elapsedMilliseconds);

			return Task.CompletedTask;

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="page"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public Task<IEnumerable<DownloadStatistics>> GetDownloadStatisticsListAsync(int page, int size)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			//队列
			var list = redisQueue.Gets<DownloadStatistics>(Down);
			list = list.Skip((page - 1) * size).Take(size).ToList();

			return Task.FromResult<IEnumerable<DownloadStatistics>>(list);

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="agentId"></param>
		/// <returns></returns>
		public Task<DownloadStatistics> GetDownloadStatisticsAsync(string agentId)
		{
			var Success = SetRedisQueue(agentId, Down, SpiderOptions.DownloadSuccess);
			var Failed = SetRedisQueue(agentId, Down, SpiderOptions.DownloadFailed);
			var DownloadTime = SetRedisQueue(agentId, Down, SpiderOptions.DownloadSuccess + Time) + SetRedisQueue(agentId, Down, SpiderOptions.DownloadFailed + Time);

			var info = new DownloadStatistics()
			{
				AgentId = agentId,
				Success = Success,
				Failed = Failed,
				ElapsedMilliseconds = DownloadTime
			};

			return Task.FromResult<DownloadStatistics>(info);

		}

		#endregion

	}
}