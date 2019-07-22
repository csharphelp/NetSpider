using System;
using System.Collections.Generic;
using System.Text;

namespace Sop.Spider
{
	/// <summary>
	/// 文件下载
	/// </summary>
	public enum DownloadFile
	{
		/// <summary>
		/// 不下载
		/// </summary>
		NotDownLoad,
        /// <summary>
		/// 分类下载
		/// </summary>
		SortDownLoad,
         
		/// <summary>
		/// 压缩格式：RAR、ZIP
		/// </summary>
		CompressionDownLoad,
		/// <summary>
		/// 视频格式有：FLV、RMVB、MP4、MVB等
		/// </summary>
		VideoDownLoad,
		/// <summary>
		/// 声音格式有：WMA、MP3等
		/// </summary>
		SoundDownLoad,
		/// <summary>
		/// 图片格式有：JPG、PNG、PDF、TIFF、SWF等
		/// </summary>
		ImgAgeDownLoad,
		/// <summary>
		/// 文档格式有：TXT、DOC、XLS、PPT、DOCX、XLSX、PPTX 等
		/// </summary>
		DocumentDownLoad,


		RemoveWebResDownload
	}
}
