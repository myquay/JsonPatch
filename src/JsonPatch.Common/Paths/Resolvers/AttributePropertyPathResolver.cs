using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace JsonPatch.Paths.Resolvers
{
    /// <summary>
    /// Resolve based on attribute property (DataMemberAttribute)
    /// </summary>
    public class AttributePropertyPathResolver : BaseResolver
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="converter"></param>
        public AttributePropertyPathResolver(IValueConverter converter) : base(converter)
        {
        }

        /// <summary>
        /// Get the property based on the component name
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            var dataMemberPropertyMatch = parentType.GetProperties().FirstOrDefault(p =>
            {
                var dataMember = p.GetCustomAttributes<DataMemberAttribute>().FirstOrDefault();
                if (dataMember == null) return false;
                return string.Equals(dataMember.Name, component);
            });

            return dataMemberPropertyMatch;
        }
    }
}
