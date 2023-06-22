using System;
using System.Linq;
using System.Reflection;

namespace JsonPatch.Paths.Resolvers
{
    /// <summary>
    /// Case insensitive path resolver
    /// </summary>
    public class CaseInsensitivePropertyPathResolver : BaseResolver
    {
        /// <summary>
        /// Case insensitive path resolver
        /// </summary>
        /// <param name="converter"></param>
        public CaseInsensitivePropertyPathResolver(IValueConverter converter) : base(converter)
        { }

        /// <summary>
        /// Get property info from parent type (case insensitive)
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            return parentType.GetProperty(component,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }
    }
}
