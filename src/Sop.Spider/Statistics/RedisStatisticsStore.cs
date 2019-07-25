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
	/// TODO:1、默认不设置库，可以指定请在构造函数指定库或者表，也可以不指定
	/// </summary>
	public class RedisStatisticsStore : IStatisticsStore
	{
		public readonly string Statistics = "statistics";
		public readonly string Download = "downloadCount";







		private readonly SpiderOptions _options;



		public RedisStatisticsStore(SpiderOptions options)
		{
			_options = options;

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="countType"></param>
		/// <param name="ownerId"></param>
		/// <returns></returns>
		private string CounterKey(string countType, string ownerId = null)
		{
			if (string.IsNullOrWhiteSpace(ownerId))
			{
				return $"Sop:Counter:{countType}";
			}
			return $"Sop:Counter:{countType}:{ownerId}";
		}
		/// <summary>
		/// 创建库 （redis 不用创建，保证链接就OK）
		/// </summary>
		/// <returns></returns>
		public Task EnsureDatabaseAndTableCreatedAsync()
		{
			return Task.CompletedTask;
		}

		#region SpiderStatistics
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownerId"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public Task IncrementTotalAsync(string ownerId, int count = 1)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			var total = redisQueue.GetCount("Total", Statistics, ownerId);
			total = total < 1 ? 1 : total;
			total = redisQueue.ChangeCount("Total", Statistics, ownerId, total + 1);

			return Task.FromResult(total);
		}


		public Task IncrementSuccessAsync(string ownerId)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			var total = redisQueue.GetCount("Total", Statistics, ownerId);
			redisQueue.ChangeCount("Total", Statistics, ownerId, total + 1);

			var success = redisQueue.GetCount("Success", Statistics, ownerId);
			success = success < 1 ? 1 : success;
			success = redisQueue.ChangeCount("Success", Statistics, ownerId, success + 1);




			//var spiderStatistics = redisQueue.Get<SpiderStatistics>(CounterKey(Statistics, ownerId));
			//redisQueue.Set(CounterKey(Statistics, ownerId), new SpiderStatistics()
			//{
			//	OwnerId = ownerId,
			//	Success = spiderStatistics.Success + 1,
			//	LastModificationTime = DateTime.Now

			//}, TimeSpan.FromDays(365));
			return Task.CompletedTask;

		}

		public Task IncrementFailedAsync(string ownerId, int count = 1)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			var total = redisQueue.GetCount("Total", Statistics, ownerId);
			redisQueue.ChangeCount("Total", Statistics, ownerId, total + 1);

			var failed = redisQueue.GetCount("Failed", Statistics, ownerId);
			failed = failed < 1 ? 1 : failed;
			failed = redisQueue.ChangeCount("Failed", Statistics, ownerId, failed + count);

			return Task.CompletedTask;

		}

		public Task StartAsync(string ownerId)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			var Start = redisQueue.GetCount("Start", Statistics, ownerId);
			Start = Start < 1 ? 1 : Start;
			Start = redisQueue.ChangeCount("Start", Statistics, ownerId, (long)DateTimeHelper.GetCurrentUnixTimeNumber());

			return Task.CompletedTask;

		}

		public Task ExitAsync(string ownerId)
		{

			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			var Exit = redisQueue.GetCount("Exit", Statistics, ownerId);
			Exit = Exit < 1 ? 1 : Exit;
			Exit = redisQueue.ChangeCount("Exit", Statistics, ownerId, (long)DateTimeHelper.GetCurrentUnixTimeNumber());

			return Task.CompletedTask;


		}

		public Task<SpiderStatistics> GetSpiderStatisticsAsync(string ownerId)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			var total = redisQueue.GetCount("Total", Statistics, ownerId);
			var info = new SpiderStatistics()
			{
				OwnerId = ownerId,
				Total = total
			};
			//var info = redisQueue.Get<SpiderStatistics>(CounterKey(Statistics, ownerId));
			return Task.FromResult<SpiderStatistics>(info);
		}

		public Task<IEnumerable<SpiderStatistics>> GetSpiderStatisticsListAsync(int page, int size)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);

			var list = redisQueue.Gets<SpiderStatistics>("Total");

			list = list.Skip((page - 1) * size).Take(size).ToList();

			return Task.FromResult<IEnumerable<SpiderStatistics>>(list);
		}
		#endregion



		#region Download
		/// <summary>
		/// 
		/// </summary>
		/// <param name="agentId"></param>
		/// <param name="count"></param>
		/// <param name="elapsedMilliseconds"></param>
		/// <returns></returns>
		public Task IncrementDownloadSuccessAsync(string agentId, int count, long elapsedMilliseconds)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			var value = redisQueue.GetCount("Download#Success", Statistics, agentId);
			value = value < 1 ? 1 : value;
			value = redisQueue.ChangeCount("Download#Success", Statistics, agentId, value + 1);



			return Task.CompletedTask;
		}

		public Task IncrementDownloadFailedAsync(string agentId, int count, long elapsedMilliseconds)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			var value = redisQueue.GetCount("Download#Failed", Statistics, agentId);
			value = value < 1 ? 1 : value;
			value = redisQueue.ChangeCount("Failed", Statistics, agentId, value + 1);

			return Task.CompletedTask;

		}

		public Task<IEnumerable<DownloadStatistics>> GetDownloadStatisticsListAsync(int page, int size)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			//队列
			var list = redisQueue.Gets<DownloadStatistics>("Download");
			list = list.Skip((page - 1) * size).Take(size).ToList();

			return Task.FromResult<IEnumerable<DownloadStatistics>>(list);

		}

		public Task<DownloadStatistics> GetDownloadStatisticsAsync(string agentId)
		{
			var redisQueue = new RedisQueue(_options.GetRedisOptions);
			//队列
			var countFailed = redisQueue.GetCount("Failed", "Download", agentId);
			var countSuccess = redisQueue.GetCount("Success", "Download", agentId);

			var info = new DownloadStatistics()
			{
				AgentId = agentId,
				Success = countSuccess,
				Failed = countFailed,
			}; 

			return Task.FromResult<DownloadStatistics>(info);

		}

		#endregion

	}
}