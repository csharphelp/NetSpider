using System;

namespace Sop.Spider
{
	/// <summary>
	/// 
	/// </summary>
    public class SpiderException : Exception
    {
        public SpiderException(string msg) : base(msg)
        {
        }
    }
}