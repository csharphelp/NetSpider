namespace Sop.Spider
{
	/// <summary>
	/// 浏览器类型
	/// </summary>
	public enum FileStorageType
	{
		/// <summary>
		/// 本地路径（）
		/// </summary>
		LocalFilePath,
		/// <summary>
		/// 本地文件原名称（存在文件名称时替换）
		/// </summary>
		LocalFileName,
		/// <summary>
		/// 网络路径
		/// </summary>
		InternetPath
	}
}