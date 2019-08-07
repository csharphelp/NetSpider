using Serilog.Core;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Sop.Spider.Analyzer
{
	/// <summary>
	/// 下载内容
	/// </summary>
	public class DownloadFormatAttribute : FormatBaseAttribute
	{


		/// <summary>
		/// 下载文件类型
		/// </summary>
		public DownloadFile DownloadFile { get; set; } = DownloadFile.SortDownLoad;

		/// <summary>
		/// 
		/// </summary>
		public string FileExtension { get; set; } = ".png";

		public string SavePath { get; set; } = null;
		public string FileName { get; set; } = null;


		/// <summary>
		/// 存储路径位置
		/// </summary>
		public FileStorageType FileStorageType { get; set; } = FileStorageType.InternetPath;

		/// <summary>
		/// 执行下载操作
		/// </summary>
		/// <param name="value">下载的链接</param>
		/// <returns>下载完成后的文件名</returns>
		protected override string FormatValue(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return value;
			}
			//判断URL地址
			bool run = false;
			do
			{
				string expression = @"^(https?|ftp|file|ws)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
				Regex reg = new Regex(expression);
				if (!reg.IsMatch(value))
				{
					//处理url
					if (value.StartsWith("//", StringComparison.InvariantCultureIgnoreCase))
					{
						value = "http:" + value;
						run = true;
					}
				}
				if (reg.IsMatch(value))
				{
					run = false;
				}

			} while (run);


			//文件存储位置
			if (string.IsNullOrEmpty(SavePath))
			{
				SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download", DateTime.Now.Year.ToString(), DateTime.Now.DayOfYear.ToString());
			}
			//文件名称			 
			if (string.IsNullOrEmpty(FileName))
			{
				FileName = Guid.NewGuid().ToString("N") + FileExtension;
			}
			var tempFileName = Path.Combine(SavePath, FileName);
			#region TODO 文件删除可以存在没有权限等异常问题
			try
			{
				if (!Directory.Exists(SavePath))
					Directory.CreateDirectory(SavePath);
				if (File.Exists(tempFileName))
					File.Delete(tempFileName);
			}
			catch (Exception ex)
			{
				Logger?.LogError($"Analyzer.Format.Download {ex.Message}");
			}
			#endregion

#if DEBUG
			Logger?.LogInformation(" Analyzer.Format.Download  开始格式化处理");
#endif



			string tempStorage = value;
			switch (FileStorageType)
			{
				case FileStorageType.LocalFilePath:
					DownFile(value, tempFileName);
					tempStorage = tempFileName;
					break;
				case FileStorageType.LocalFileName:
					tempStorage = FileName;
					break;
				case FileStorageType.InternetPath:
					tempStorage = value;
					break;
			}

			return tempStorage;
		}
		/// <summary>
		/// 下载文件
		/// </summary>
		/// <param name="fileUrl"></param>
		/// <param name="fileNamePath"></param>
		private Task<bool> DownFile(string fileUrl, string fileNamePath)
		{
			bool fileExist = false;
			try
			{
				try
				{

					using (FileStream fileStream = new FileStream(fileNamePath, FileMode.Create, FileAccess.Write, FileShare.Write))
					{
						if (WebRequest.Create(fileUrl) is HttpWebRequest request)
						{
							if (request.GetResponse() is HttpWebResponse response)
							{
								Stream responseStream = response.GetResponseStream();
								byte[] bArr = new byte[1024];
								if (responseStream != null)
								{
									int size = responseStream.Read(bArr, 0, (int)bArr.Length);
									while (size > 0)
									{
										//stream.Write(bArr, 0, size);
										fileStream.Write(bArr, 0, size);
										size = responseStream.Read(bArr, 0, (int)bArr.Length);
									}
									fileExist = File.Exists(fileNamePath);
								}
							}
						}
					}
					GC.Collect();
					GC.WaitForFullGCComplete();

				}
				catch (Exception ex)
				{
					throw new SpiderArgumentException("下载文件 " + ex.Message);
				}
			}
			catch (SpiderException ex)
			{
				throw new SpiderArgumentException("下载文件 " + ex.Message);
			}
			return Task.FromResult<bool>(fileExist);
		}

		/// <summary>
		/// 校验参数是否设置正确
		/// </summary>
		protected override void CheckArguments()
		{
			//if (DownloadFile == null)
			//{

			//	DownloadFile = DownloadFile.NotDownLoad;
			//	//throw new SpiderException("DownloadFile 下载文件为空");
			//}
		}


	}
}
