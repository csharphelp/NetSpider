namespace Sop.DotnetSpider.Common
{
	/// <summary>
	/// 文件锁工厂接口
	/// </summary>
    public interface ILockerFactory
    {
        ILocker GetLocker();

        ILocker GetLocker(string locker);
    }
}