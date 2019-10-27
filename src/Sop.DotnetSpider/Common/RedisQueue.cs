using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Sop.Spider.Common
{


	/// <summary>
	/// 使用Redis的队列实现
	/// </summary>
	public class RedisQueue
	{

		#region private
		private static ConfigurationOptions _connection = null;
		private static volatile ConnectionMultiplexer _instance;
		private static readonly object Lock = new object();
		private static IDatabase _db;

		public static ConnectionMultiplexer Instance
		{
			get
			{
				if (_instance != null && _instance.IsConnected)
					return _instance;
				lock (Lock)
				{
					try
					{
						if (_instance != null && _instance.IsConnected)
							return _instance;

						if (_connection == null)
							throw new Exception("Redis connection string is empty");

						_instance?.Dispose();
						_instance = ConnectionMultiplexer.Connect(_connection);
					}
					catch (Exception ex)
					{
						throw new Exception("Redis service is not started " + ex.Message);
					}

				}
				return _instance;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		public RedisQueue(ConfigurationOptions connection = null)
		{
		
			_connection = connection;
			_db = Instance.GetDatabase();
		}


		public IServer Server(EndPoint endPoint)
		{
			return Instance.GetServer(endPoint);
		}

		public EndPoint[] GetEndpoints()
		{
			return Instance.GetEndPoints();
		}

		public void FlushDb(int? db = null)
		{
			var endPoints = GetEndpoints();

			foreach (var endPoint in endPoints)
			{
				Server(endPoint).FlushDatabase(db ?? -1);
			}
		}
		/// <summary>
		/// 生成计数器的CacheKey
		/// </summary>
		/// <param name="countType">计数类型</param>
		/// <param name="tenantType">租户类型</param>
		/// <returns>CacheKey</returns>
		private string CounterKey(string countType, string tenantType)
		{
			return $"Sop:Counter:{countType}:{tenantType}";
		}

		/// <summary>
		/// 生成排行榜的CacheKey
		/// </summary>
		/// <param name="countType">计数类型</param>
		/// <param name="ownerType">所有者类型</param>
		/// <param name="ownerId">所有者Id</param>
		/// <returns>CacheKey</returns>
		private string RankingKey(string countType, string ownerType, string ownerId)
		{
			return $"Sop:Ranking:{countType}:{ownerType}#{ownerId}";
		}
		#endregion


		#region Methods


		public virtual T Get<T>(string key)
		{
			if (_db != null)
			{
				var value = _db?.StringGetAsync(key);
				var obj = Instance.Wait(value);
				if (obj.IsNull)
					return default(T);
				return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(obj));
			}
			return default(T);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public virtual List<T> Gets<T>(string pattern)
		{
			var list = new List<T>();
			foreach (var ep in GetEndpoints())
			{
				var server = Server(ep);
				var keys = server.Keys(pattern: "*" + pattern + "*");
				foreach (var key in keys)
				{
					list.Add(this.Get<T>(key));
				} 
			}
			return list;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <param name="cacheTime"></param>
		public virtual void Set(string key, object data, int cacheTime)
		{
			if (data == null)
				return;

			var entryBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
			var expiresIn = TimeSpan.FromMinutes(cacheTime);

			//_db.StringSet(key, entryBytes, expiresIn);
			_db.StringSetAsync(key, entryBytes, expiresIn, flags: CommandFlags.FireAndForget);
		}
		/// <summary>
		/// 添加
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="timeSpan"></param>
		public void Set(string key, object value, TimeSpan timeSpan)
		{
			if (value == null)
				return;
			var entryBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
			//_db.StringSet(key, entryBytes, timeSpan);
			_db.StringSetAsync(key, entryBytes, timeSpan, flags: CommandFlags.FireAndForget);
		}
		/// <summary>
		/// 是否存在
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual bool IsSet(string key)
		{
			return _db.KeyExists(key);
		}

		/// <summary>
		/// 删除 
		/// </summary>
		/// <param name="key"></param>
		public virtual void Remove(string key)
		{
			_db.KeyDeleteAsync(key, CommandFlags.FireAndForget);
		}
		/// <summary>
		/// 删除父类
		/// </summary>
		/// <param name="pattern"></param>
		public virtual void RemoveByPattern(string pattern)
		{
			foreach (var ep in GetEndpoints())
			{
				var server = Server(ep);
				var keys = server.Keys(pattern: "*" + pattern + "*");
				foreach (var key in keys)
					_db.KeyDelete(key);
			}
		}
		/// <summary>
		/// 清空
		/// </summary>
		public virtual void Clear()
		{
			foreach (var ep in GetEndpoints())
			{
				var server = Server(ep);

				//we can use the code below (commented)
				//but it requires administration permission - ",allowAdmin=true"
				//server.FlushDatabase();
				//that's why we simply interate through all elements now
				var keys = server.Keys();
				foreach (var key in keys)
					_db.KeyDelete(key);
			}
		}


		#endregion

		#region Queue
		/// <summary>
		/// 将一个对象加入队列
		/// </summary>
		/// <param name="cacheKey">队列名称</param>
		/// <param name="value">加入队列的对象</param>
		public void Push(string cacheKey, RedisValue value)
		{
			_db.ListLeftPushAsync(cacheKey, value, flags: CommandFlags.FireAndForget);
		}

		/// <summary>
		/// 从队列起始位置获取一个对象
		/// </summary>
		/// <param name="cacheKey">队列名称</param>
		/// <returns>队列起始位置的对象</returns>
		public RedisValue Pop(string cacheKey)
		{
			return _db.ListRightPop(cacheKey);
		}
		#endregion

		#region Counter

		/// <summary>
		/// 改变（增加或减少）计数
		/// </summary>
		/// <param name="countType">计数类型</param>
		/// <param name="tenantType">租户类型</param>
		/// <param name="tenantId">租户Id</param>
		/// <param name="value">改变的计数值，可以为负值，即减少计数</param>
		/// <returns>改变后的计数值</returns>
		public long ChangeCount(string countType, string tenantType, string tenantId, long value = 1)
		{
			var task = _db.HashIncrementAsync(this.CounterKey(countType, tenantType), tenantId, value);
			return _db.Wait(task);
		}

		/// <summary>
		/// 删除一个对象的计数
		/// </summary>
		/// <param name="countType">计数类型</param>
		/// <param name="tenantType">租户类型</param>
		/// <param name="tenantId">租户Id</param>
		public void DeleteCount(string countType, string tenantType, string tenantId)
		{
			_db.HashDeleteAsync(this.CounterKey(countType, tenantType), tenantId, CommandFlags.FireAndForget);
		}

		/// <summary>
		/// 获取一个对象的计数，例如网页中的链接数
		/// </summary>
		/// <param name="countType">计数类型</param>
		/// <param name="tenantType">租户类型</param>
		/// <param name="tenantId">租户Id</param>
		/// <returns>计数值</returns>
		public long GetCount(string countType, string tenantType, string tenantId)
		{
			var task = _db.HashGetAsync(this.CounterKey(countType, tenantType), tenantId);
			return (long)_db.Wait(task);
		}


		/// <summary>
		/// 改变（增加或减少）排行榜的分值
		/// </summary>
		/// <param name="countType">计数类型</param>
		/// <param name="ownerType">所有者类型</param>
		/// <param name="ownerId">所有者Id</param>
		/// <param name="objectId">排行榜对象Id</param>
		/// <param name="value">改变的排行榜分值，可以为负值，即减少分值</param>
		/// <returns>改变后的排行榜分值</returns>
		public double ChangeRanking(string countType, string ownerType, string ownerId, string objectId, double value = 1)
		{

			var task = _db.SortedSetIncrementAsync(this.RankingKey(countType, ownerType, ownerId), objectId, value);
			return _db.Wait(task);
		}

		/// <summary>
		/// 删除排行榜中的一个对象
		/// </summary>
		/// <param name="countType">计数类型</param>
		/// <param name="ownerType">所有者类型</param>
		/// <param name="ownerId">所有者Id</param>
		/// <param name="objectId">排行榜对象Id</param>
		public void DeleteRanking(string countType, string ownerType, string ownerId, string objectId)
		{
			_db.SortedSetRemoveAsync(this.RankingKey(countType, ownerType, ownerId), objectId, CommandFlags.FireAndForget);
		}

		/// <summary>
		/// 获取排行榜中一个对象的分值，例如网页中链接数
		/// </summary>
		/// <param name="countType">计数类型</param>
		/// <param name="ownerType">所有者类型</param>
		/// <param name="ownerId">所有者Id</param>
		/// <param name="objectId">排行榜对象Id</param>
		/// <returns>排行榜中一个对象的分值</returns>
		public double GetRanking(string countType, string ownerType, string ownerId, string objectId)
		{

			var task = _db.SortedSetScoreAsync(this.RankingKey(countType, ownerType, ownerId), objectId);
			return _db.Wait(task) ?? 0;
		}

		/// <summary>
		/// 根据分值排序，获取排行榜中前N条记录
		/// </summary>
		/// <param name="countType">计数类型</param>
		/// <param name="ownerType">所有者类型</param>
		/// <param name="ownerId">所有者Id</param>
		/// <param name="topNumber">前N条记录</param>
		/// <returns>排行榜数据的键值对集合，Key是排行榜对象Id，Value是其对应的分值</returns>
		public IEnumerable<KeyValuePair<long, double>> TopRanking(string countType, string ownerType, string ownerId, int topNumber)
		{

			var task = _db.SortedSetRangeByRankWithScoresAsync(this.RankingKey(countType, ownerType, ownerId), 0, topNumber, Order.Descending);
			var result = _db.Wait(task);
			return result.Select(n => new KeyValuePair<long, double>((long)n.Element, n.Score));
		}
		#endregion

	}


}