using System.Collections.Generic;
using System.Threading.Tasks;
using Sop.Spider.DownloadAgentRegisterCenter.Entity;

namespace Sop.Spider.DownloadAgentRegisterCenter
{
	/// <summary>
	/// 下载器代理存储接口
	/// </summary>
	public interface IDownloadAgentStore
	{
		/// <summary>
		/// 创建数据库和表
		/// </summary>
		/// <returns></returns>
		Task EnsureDatabaseAndTableCreatedAsync();

		/// <summary>
		/// 查询所有已经注册的下载器代理
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<Entity.DownloadAgent>> GetAllListAsync();

		/// <summary>
		/// 添加下载器代理
		/// </summary>
		/// <param name="agent">下载器代理</param>
		/// <returns></returns>
		Task RegisterAsync(Entity.DownloadAgent agent);

		/// <summary>
		/// 保存下载器代理的心跳
		/// </summary>
		/// <param name="heartbeat">下载器代理心跳</param>
		/// <returns></returns>
		Task HeartbeatAsync(DownloadAgentHeartbeat heartbeat);
	}
}