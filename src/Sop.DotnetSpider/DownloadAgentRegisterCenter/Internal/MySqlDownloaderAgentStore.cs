using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Sop.Spider.Common;
using Sop.Spider.DownloadAgentRegisterCenter.Entity;
using MySql.Data.MySqlClient;

namespace Sop.Spider.DownloadAgentRegisterCenter.Internal
{
	public class MySqlDownloadAgentStore : IDownloadAgentStore
	{
		private readonly SpiderOptions _options;

		public MySqlDownloadAgentStore(SpiderOptions options)
		{
			_options = options;
		}

		public async Task EnsureDatabaseAndTableCreatedAsync()
		{
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				await conn.ExecuteAsync("CREATE SCHEMA IF NOT EXISTS SopSpider DEFAULT CHARACTER SET utf8mb4;");
				var sql1 =
					$"create table if not exists SopSpider.downloader_agent(id nvarchar(40) primary key, `name` nvarchar(255) null, processor_count int null, total_memory int null, is_deleted tinyint(1) default 0 null, creation_time timestamp default CURRENT_TIMESTAMP not null, last_modification_time timestamp default CURRENT_TIMESTAMP not null, key NAME_INDEX (`name`));";
				var sql2 =
					$"create table if not exists SopSpider.downloader_agent_heartbeat(id bigint AUTO_INCREMENT primary key, agent_id nvarchar(40) not null, `agent_name` nvarchar(255) null, free_memory int null, downloader_count int null, creation_time timestamp default CURRENT_TIMESTAMP not null, key NAME_INDEX (`agent_name`), key ID_INDEX (`agent_id`));";
				await conn.ExecuteAsync(sql1);
				await conn.ExecuteAsync(sql2);
			}
		}

		public async Task<IEnumerable<Entity.DownloadAgent>> GetAllListAsync()
		{
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				return (await conn.QueryAsync<Entity.DownloadAgent>(
						$"SELECT * FROM SopSpider.downloader_agent"))
					.ToList();
			}
		}

		public async Task RegisterAsync(Entity.DownloadAgent agent)
		{
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				await conn.ExecuteAsync(
					$"INSERT IGNORE INTO SopSpider.downloader_agent (id, `name`, processor_count, total_memory, creation_time, last_modification_time) VALUES (@Id, @Name, @ProcessorCount, @TotalMemory, @CreationTime, @LastModificationTime); UPDATE SopSpider.downloader_agent SET is_deleted = false WHERE id = @Id",
					agent);
			}
		}

		public async Task HeartbeatAsync(DownloadAgentHeartbeat agent)
		{
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				var obj = await conn.QueryFirstOrDefaultAsync<dynamic>(
					$"SELECT id FROM SopSpider.downloader_agent WHERE id = @Id && is_deleted = false LIMIT 1;",
					new
					{
						agent.Id
					});
				if (obj != null)
				{
					await conn.ExecuteAsync(
						$"INSERT IGNORE INTO SopSpider.downloader_agent_heartbeat (agent_id, agent_name, free_memory, downloader_count, creation_time) VALUES (@AgentId, @AgentName, @FreeMemory, @DownloaderCount, @CreationTime);",
						agent);
					await conn.ExecuteAsync(
						$"UPDATE SopSpider.downloader_agent SET last_modification_time = @LastModificationTime WHERE id = @AgentId",
						new {agent.AgentId, LastModificationTime = agent.CreationTime});
				}
			}
		}
	}
}