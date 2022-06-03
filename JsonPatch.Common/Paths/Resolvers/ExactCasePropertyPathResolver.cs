using System;
using System.Linq;
using System.Reflection;

namespace JsonPatch.Common.Paths.Resolvers
{
    /// <summary>
    /// Implementation of path resolver that will only match on exact case
    /// </summary>
    public class ExactCasePropertyPathResolver : BaseResolver
    {
        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            return parentType.GetProperty(component);
        }
    }
}
