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
        protected override PropertyInfo GetProperty(Type parentType, string component)
        {
            return parentType.GetProperty(component,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }

        protected override string[] GetPathComponents(string path)
        {
            // Normalize the path by ensuring it begins with a single forward slash, and has
            // no trailing slashes. Modify the path variable itself so that any character
            // positions we report in error messages are accurate.

            path = "/" + path.ToLowerInvariant().Trim('/');

            return path.Split('/').Skip(1).Select(s => s.ToLowerInvariant()).ToArray();
        }
    }
}
