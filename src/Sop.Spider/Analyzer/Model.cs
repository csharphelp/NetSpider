using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sop.Spider.DataStorage;

namespace Sop.Spider.Analyzer
{
	/// <summary>
	/// 实体模型
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Model<T> where T : EntityBase<T>, new()
    {
	    /// <summary>
	    /// 实体的类型名称
	    /// </summary>
        public string TypeName { get; }
        
        /// <summary>
        /// 数据模型的选择器
        /// </summary>
        public SelectAttribute Selector { get; }

        /// <summary>
        /// 从最终解析到的结果中取前 Take 个实体
        /// </summary>
        public int Take { get; }

        /// <summary>
        /// 设置 Take 的方向, 默认是从头部取
        /// </summary>
        public bool TakeFromHead { get; }

        /// <summary>
        /// 爬虫实体定义的数据库列信息
        /// </summary>
        public HashSet<ValueSelectAttribute> ValueSelectors { get; }

        /// <summary>
        /// 目标链接的选择器
        /// </summary>
        public HashSet<FollowSelectAttribute> FollowSelectors { get; }

        /// <summary>
        /// 共享值的选择器
        /// </summary>
        public HashSet<ValueSelectAttribute> ShareValueSelectors { get; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public Model()
        {
            var type = typeof(T);
            TypeName = type.FullName;
            var entitySelector =
                type.GetCustomAttributes(typeof(EntitySelectAttribute), true).FirstOrDefault() as EntitySelectAttribute;
            int take = 0;
            bool takeFromHead = true;
	        SelectAttribute selector = null;
            if (entitySelector != null)
            {
                take = entitySelector.Take;
                takeFromHead = entitySelector.TakeFromHead;
                selector = new SelectAttribute { Expression = entitySelector.Expression, Type = entitySelector.Type};
            }
           
			var followSelectors = type.GetCustomAttributes(typeof(FollowSelectAttribute), true).Select(x => (FollowSelectAttribute) x)
                .ToList();

            var sharedValueSelectors = type.GetCustomAttributes(typeof(ValueSelectAttribute), true)
                .Select(x => (ValueSelectAttribute) x).ToList();

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var valueSelectors = new HashSet<ValueSelectAttribute>();
            foreach (var property in properties)
            {
                var valueSelector = property.GetCustomAttributes(typeof(ValueSelectAttribute), true).FirstOrDefault() as ValueSelectAttribute;

                if (valueSelector == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(valueSelector.Name))
                {
                    valueSelector.Name = property.Name;
                }

                valueSelector.FormatBaseAttributes = property.GetCustomAttributes(typeof(FormatBaseAttribute), true)
                    .Select(p => (FormatBaseAttribute) p).ToArray();
                valueSelector.PropertyInfo = property;
                valueSelector.NotNull = property.GetCustomAttributes(typeof(Required), false).Any();
                valueSelectors.Add(valueSelector);
            }

            Selector = selector;
            ValueSelectors = valueSelectors;
            FollowSelectors = new HashSet<FollowSelectAttribute>(followSelectors);
            ShareValueSelectors = new HashSet<ValueSelectAttribute>(sharedValueSelectors);
            Take = take;
            TakeFromHead = takeFromHead;
        }
    }
}