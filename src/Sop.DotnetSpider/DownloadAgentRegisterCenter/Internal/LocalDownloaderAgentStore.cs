using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sop.DotnetSpider.DownloadAgentRegisterCenter.Entity;

namespace Sop.DotnetSpider.DownloadAgentRegisterCenter.Internal
{
	/// <summary>
	/// 本地下载器代理存储
	/// </summary>
	internal class LocalDownloadAgentStore : IDownloadAgentStore
	{
		private readonly ConcurrentDictionary<string, DotnetSpider.Entity.DownloadAgent> _agents =
			new ConcurrentDictionary<string, DotnetSpider.Entity.DownloadAgent>();

		public Task EnsureDatabaseAndTableCreatedAsync()
		{
			return Task.CompletedTask;
		}

		public Task<IEnumerable<DotnetSpider.Entity.DownloadAgent>> GetAllListAsync()
		{
			return Task.FromResult((IEnumerable<DotnetSpider.Entity.DownloadAgent>)_agents.Values);
		}

		public Task RegisterAsync(DotnetSpider.Entity.DownloadAgent agent)
		{
			_agents.AddOrUpdate(agent.Id, x => agent, (s, a) =>
			{
				a.CreationTime = DateTime.Now;
				a.LastModificationTime = DateTime.Now;
				return a;
			});
			return Task.CompletedTask;
		}

		/// <summary>
		/// 本地代理不需要留存心跳
		/// </summary>
		/// <param name="heartbeat"></param>
		/// <returns></returns>
		public Task HeartbeatAsync(DownloadAgentHeartbeat heartbeat)
		{
			if (_agents.TryGetValue(heartbeat.AgentId, out var agent))
			{
				agent.LastModificationTime = DateTime.Now;
			}

			return Task.CompletedTask;
		}
	}
}