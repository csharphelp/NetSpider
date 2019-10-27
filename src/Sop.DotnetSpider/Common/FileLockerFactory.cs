using System;
using System.IO;
using System.Threading;

namespace Sop.DotnetSpider.Common
{
	/// <summary>
	/// 文件锁
	/// </summary>
    public class FileLockerFactory : ILockerFactory
    {
        private readonly string _folder;

        public FileLockerFactory()
        {
            _folder = Path.Combine(Framework.GlobalDirectory, "sessions");
            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }
        }
		/// <summary>
		/// 获取文件锁（文件并发使用吧？）
		/// </summary>
		/// <returns></returns>
        ILocker ILockerFactory.GetLocker()
        {
            var path = Path.Combine(_folder, $"{Guid.NewGuid():N}.lock");
            return new FileLocker(path);
        }
		/// <summary>
		///获取文件并发锁
		/// </summary>
		/// <param name="locker"></param>
		/// <returns></returns>
        ILocker ILockerFactory.GetLocker(string locker)
        {
            var path = Path.Combine(Framework.GlobalDirectory, $"{locker}.lock");

            while (true)
            {
                try
                {
                    return new FileLocker(path);
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}