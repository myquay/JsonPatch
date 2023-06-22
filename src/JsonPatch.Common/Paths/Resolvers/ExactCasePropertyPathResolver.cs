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
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="converter"></param>
        public ExactCasePropertyPathResolver(IValueConverter converter) : base(converter)
        { }

        /// <summary>
        /// Returns the property info for the given component (case sensitive)
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            return parentType.GetProperty(component);
        }
    }
}
