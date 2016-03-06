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
        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            return parentType.GetProperty(component,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }
    }
}
