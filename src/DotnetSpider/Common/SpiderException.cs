using System;

namespace Sop.Spider.Common
{
    public class SpiderException : Exception
    {
        public SpiderException(string msg) : base(msg)
        {
        }
    }
}