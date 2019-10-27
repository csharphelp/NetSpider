using System.Collections.Generic;

namespace Sop.DotnetSpider.Analyzer.HtmlAgilityPack.Css
{
	/// <summary>
    /// Represents a selector implementation over an arbitrary type of elements.
    /// </summary>
    public delegate IEnumerable<TElement> Selector<TElement>(IEnumerable<TElement> elements);
}