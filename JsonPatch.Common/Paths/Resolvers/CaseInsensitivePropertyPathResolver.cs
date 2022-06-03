using System;
using System.Linq;
using System.Reflection;

namespace JsonPatch.Common.Paths.Resolvers
{
    /// <summary>
    /// Case insensitive path resolver
    /// </summary>
    public class CaseInsensitivePropertyPathResolver : BaseResolver
    {
        public CaseInsensitivePropertyPathResolver(IValueConverter converter) : base(converter)
        { }

        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            return parentType.GetProperty(component,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }
    }
}
