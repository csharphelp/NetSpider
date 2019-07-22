using System;
using System.IO;
using System.Net;
using Serilog.Core;

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
		public DownloadFile DownloadFile { get; set; } = DownloadFile.NotDownLoad; 

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
			var filePath = value;
			var name = Path.GetFileName(filePath);
			string tmpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
			string file = Path.Combine(tmpPath, name);
			//文件存储 todo 这里测试了图片下载
			try
			{
				if (!Directory.Exists(tmpPath))
				{
					Directory.CreateDirectory(tmpPath);
				}
				if (File.Exists(file))
				{
					File.Delete(file);
				}
				//using (WebClient my = new WebClient())
				//{
				//	my.DownloadFile(filePath, file);
				//}
				try
				{
					using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
					{
						if (WebRequest.Create(filePath) is HttpWebRequest request)
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
										fs.Write(bArr, 0, size);
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
			return file;
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
