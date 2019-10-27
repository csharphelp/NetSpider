using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sop.DotnetSpider
{
    /// <summary>
    /// 查询器类型
    /// </summary>
    [Flags]
#if !NET451
	[JsonConverter(typeof(StringEnumConverter))]
#endif
    public enum SelectorType
    {
        /// <summary>
        /// XPath
        /// </summary>
        XPath,

        /// <summary>
        /// Regex
        /// </summary>
        Regex,

        /// <summary>
        /// Css
        /// </summary>
        Css,

		/// <summary>
		/// JsonPath
		/// 详细参考：https://blog.csdn.net/fwk19840301/article/details/80452258
		/// </summary>
		JsonPath,

        /// <summary>
        /// Enviroment
        /// </summary>
        Enviroment
    }
}