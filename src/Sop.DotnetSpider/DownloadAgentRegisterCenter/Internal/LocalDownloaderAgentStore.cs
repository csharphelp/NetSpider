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
		private readonly ConcurrentDictionary<string, Entity.DownloadAgent> _agents =
			new ConcurrentDictionary<string, Entity.DownloadAgent>();

		public Task EnsureDatabaseAndTableCreatedAsync()
		{
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Entity.DownloadAgent>> GetAllListAsync()
		{
			return Task.FromResult((IEnumerable<Entity.DownloadAgent>)_agents.Values);
		}

		public Task RegisterAsync(Entity.DownloadAgent agent)
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