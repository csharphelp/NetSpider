using Serilog.Core;
using System;
using System.IO;
using System.Net;
using Microsoft.Extensions.Logging;

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
		/// 存储路径位置
		/// </summary>
		public DownloadFileStorageType DownloadFileStorageType { get; set; }

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

			string tempValue = value;
			string tempFileName = Path.GetFileName(value) ?? Guid.NewGuid().ToString("N") + ".txt";

			string tmpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download");
			string fileNamePath = Path.Combine(tmpPath, tempFileName);

			//TODO 文件删除可以存在没有权限等异常问题
			try
			{
				if (!Directory.Exists(tmpPath))
					Directory.CreateDirectory(tmpPath);

				//todo 异常暂时不处理，存在删除文件的权限
				if (File.Exists(fileNamePath))
					fileNamePath = Path.Combine(tmpPath, Guid.NewGuid().ToString("N") + Path.GetExtension(tempFileName));
			}
			catch (Exception ex)
			{
				Logger?.LogError($"Analyzer.Format.Download {ex.Message}");
			}
#if DEBUG
			Logger?.LogInformation(" Analyzer.Format.Download  开始格式化处理");
#endif

			switch (DownloadFileStorageType)
			{
				case DownloadFileStorageType.LocalFilePath:
					{
						//文件存储 todo 这里测试了图片下载
						DownFile(value, fileNamePath);
						tempValue = fileNamePath;
					}
					break;
				case DownloadFileStorageType.LocalFileName:
					{
						tempValue = tempFileName;
					}
					break;
				case DownloadFileStorageType.InternetPath:
				default:


					break;
			}
			return tempValue;
		}
		/// <summary>
		/// 下载文件
		/// </summary>
		/// <param name="fileUrl"></param>
		/// <param name="fileNamePath"></param>
		private void DownFile(string fileUrl, string fileNamePath)
		{
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
