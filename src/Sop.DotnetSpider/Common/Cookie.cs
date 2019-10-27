using Sop.DotnetSpider.DataStorage;

namespace Sop.DotnetSpider.Common
{
	/// <summary>
	/// 
	/// </summary>
    public class Cookie
    {
        public string Name { get; }

        public string Value { get; }

        public string Domain { get; }

        public string Path { get; }

        public Cookie(string name, string value, string domain, string path = "/")
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(value, nameof(value));

            Name = name;
            Value = value;
            Domain = domain;
            Path = path;
        }
    }
}