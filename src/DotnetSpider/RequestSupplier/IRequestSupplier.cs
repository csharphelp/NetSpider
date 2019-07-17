using System;
using Sop.Spider.Downloader;

namespace Sop.Spider.RequestSupplier
{
    /// <summary>
    /// 请求供应接口
    /// </summary>
    public interface IRequestSupplier
    {
        /// <summary>
        /// 运行请求供应
        /// </summary>
        /// <param name="enqueueDelegate">请求入队的方法</param>
        void Execute(Action<Request> enqueueDelegate);
    }
}