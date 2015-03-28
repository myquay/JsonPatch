using System;
using System.Linq;
using System.Reflection;

namespace JsonPatch.Paths.Resolvers
{
    /// <summary>
    /// Implementation of path resolver that will only match on exact case
    /// </summary>
    public class ExactCasePropertyPathResolver : BaseResolver
    {
        protected override PropertyInfo GetProperty(Type parentType, string component)
        {
            return parentType.GetProperty(component);
        }

        protected override string[] GetPathComponents(string path)
        {
            // Normalize the path by ensuring it begins with a single forward slash, and has
            // no trailing slashes. Modify the path variable itself so that any character
            // positions we report in error messages are accurate.

            path = "/" + path.Trim('/');

            return path.Split('/').Skip(1).ToArray();
        }
    }
}
