using Microsoft.Extensions.Configuration;
using System;
using Sop.Spider.DataStorage;
using StackExchange.Redis;

namespace Sop.Spider.Common
{
	/// <summary>
	/// 任务选项
	/// </summary>
	public class SpiderOptions
	{
		private readonly IConfiguration _configuration;

		#region 统计计数类型
		public const string Success = "Success";
		public const string Failed = "Failed";
		public const string Start = "Start";
		public const string Exit = "Exit";
		public const string Total = "Total";		
		public const string DownloadSuccess = "DownloadSuccess";
		public const string DownloadFailed ="DownloadFailed";
		public const string Print = "Print";

		 

		public const string TenantType = "Spider";
	 
		#endregion


		/// <summary>
		/// 租户类型
		/// </summary>


		public ConfigurationOptions GetRedisOptions
		{
			get
			{
				ConfigurationOptions option = new ConfigurationOptions
				{
					AllowAdmin = true,
					AbortOnConnectFail = false,
					SyncTimeout = 6000,
					Password = "sopcce.com.cc2018"
				};
				option.EndPoints.Add("127.0.0.1", 6379);
				var test = ConnectionMultiplexer.Connect(option, null);
				if (test.IsConnected == true)
				{
					return option;
				}
				#region 尝试启动本机服务 windows服务

				//var serviceControllers = ServiceController.GetServices();
				//var listDictionary = new Dictionary<string, ServiceController>();
				//foreach (var service in serviceControllers)
				//{
				//	if (service.ServiceName.ToLower().Contains("redis"))
				//	{
				//		listDictionary.Add(service.ServiceName.ToLower(), service);
				//	}
				//}
				//if (listDictionary.ContainsKey("redis"))
				//{
				//	throw new System.Exception("不存在redis服务");
				//}
				//foreach (var info in listDictionary)
				//{
				//	if (info.Value.Status != ServiceControllerStatus.Running)
				//	{
				//		info.Value.Start();
				//	}
				//}
				#endregion
				return null;
			}
		}

		public SpiderOptions(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		/// <summary>
		/// 获取配置文件中当前键值对应的值，并转换为相应的类型
		/// </summary>
		/// <typeparam name="T">想要转换的类型</typeparam>
		/// <param name="key">键值</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>配置项值</returns>
		public T AppSettings<T>(string key, T defaultValue)
		{
			var v = _configuration[key];
			return string.IsNullOrEmpty(v) ? defaultValue : (T)Convert.ChangeType(v, typeof(T));
		}

		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		public string ConnectionString => _configuration["ConnectionString"];





		public int RequestsTime => this.AppSettings<int>("requestsTime", 60);

		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		public string StorageConnectionString => _configuration["StorageConnectionString"];

		/// <summary>
		/// 存储器类型: FullTypeName, AssemblyName
		/// </summary>
		public string Storage => _configuration["Storage"];

		/// <summary>
		/// 是否忽略数据库相关的大写小
		/// </summary>
		public bool StorageIgnoreCase => string.IsNullOrWhiteSpace(_configuration["IgnoreCase"]) ||
										 bool.Parse(_configuration["StorageIgnoreCase"]);

		/// <summary>
		/// 存储器失败重试时间限制
		/// </summary>
		public int StorageRetryTimes => string.IsNullOrWhiteSpace(_configuration["StorageRetryTimes"])
			? 600
			: int.Parse(_configuration["StorageRetryTimes"]);

		/// <summary>
		/// 是否使用事务操作。默认不使用。
		/// </summary>
		public bool StorageUseTransaction => !string.IsNullOrWhiteSpace(_configuration["StorageUseTransaction"]) &&
											 bool.Parse(_configuration["StorageUseTransaction"]);

		/// <summary>
		/// 存储器类型
		/// </summary>
		public StorageType StorageType => string.IsNullOrWhiteSpace(_configuration["StorageType"])
			? StorageType.InsertIgnoreDuplicate
			: (StorageType)Enum.Parse(typeof(StorageType), _configuration["StorageType"]);

		/// <summary>
		/// MySql 文件类型
		/// </summary>
		public string MySqlFileType => _configuration["MySqlFileType"];

		/// <summary>
		/// 邮件服务地址
		/// </summary>
		public string EmailHost => _configuration["EmailHost"];

		/// <summary>
		/// 邮件用户名
		/// </summary>
		public string EmailAccount => _configuration["EmailAccount"];

		/// <summary>
		/// 邮件密码
		/// </summary>
		public string EmailPassword => _configuration["EmailPassword"];

		/// <summary>
		/// 邮件显示名称
		/// </summary>
		public string EmailDisplayName => _configuration["EmailDisplayName"];

		/// <summary>
		/// 邮件服务端口
		/// </summary>
		public string EmailPort => _configuration["EmailPort"];

		/// <summary>
		/// Kafka 服务地址
		/// </summary>
		public string KafkaBootstrapServers => string.IsNullOrWhiteSpace(_configuration["KafkaBootstrapServers"])
			? "localhost:9092"
			: _configuration["KafkaBootstrapServers"];

		/// <summary>
		/// Kafka 消费组
		/// </summary>
		public string KafkaConsumerGroup => string.IsNullOrWhiteSpace(_configuration["KafkaConsumerGroup"])
			? "Sop.Spider"
			: _configuration["KafkaConsumerGroup"];

		public int KafkaTopicPartitionCount => string.IsNullOrWhiteSpace(_configuration["KafkaTopicPartitionCount"])
			? 50
			: int.Parse(_configuration["KafkaTopicPartitionCount"]);

		public string ResponseHandlerTopic => "ResponseHandler-";

		/// <summary>
		/// 下载代理注入订阅消息队列
		/// </summary>
		public string DownloadedAgentRegisterCenterTopic =>
			string.IsNullOrWhiteSpace(_configuration["DownloadedAgentRegisterCenterTopic"])
				? "DownloaderAgentRegisterCenter"
				: _configuration["DownloadedAgentRegisterCenterTopic"];

		public string StatisticsServiceTopic => string.IsNullOrWhiteSpace(_configuration["StatisticsServiceTopic"])
			? "StatisticsService"
			: _configuration["StatisticsServiceTopic"];

		public string DownloadQueueTopic => string.IsNullOrWhiteSpace(_configuration["DownloadQueueTopic"])
			? "DownloadQueue"
			: _configuration["DownloadQueueTopic"];

		public string AdslDownloadQueueTopic => string.IsNullOrWhiteSpace(_configuration["AdslDownloadQueueTopic"])
			? "AdslDownloadQueue"
			: _configuration["AdslDownloadQueueTopic"];

		public string[] PartitionTopics => _configuration.GetSection("PartitionTopics").Get<string[]>();

		/// <summary>
		/// 消息队列推送消息、文章话题、获取消息失败重试的次数
		/// 默认是 28800 次即 8 小时
		/// </summary>
		public int MessageQueueRetryTimes => string.IsNullOrWhiteSpace(_configuration["MessageQueueRetryTimes"])
			? 28800
			: int.Parse(_configuration["MessageQueueRetryTimes"]);

		/// <summary>
		/// 设置消息过期时间，每个消息发送应该带上时间，超时的消息不作处理
		/// 默认值 60 秒
		/// </summary>
		public int MessageExpiredTime => string.IsNullOrWhiteSpace(_configuration["MessageExpiredTime"])
			? 60
			: int.Parse(_configuration["MessageExpiredTime"]);











	}
}