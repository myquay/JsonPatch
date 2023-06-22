using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Paths.Resolvers
{
    /// <summary>
    /// Flexible path resolver. It will try to resolve the path using the following resolvers:
    /// * Attribute Property Path Resolver
    /// * Exact Case Property Path Resolver
    /// * Case Insensitive Property Path Resolver
    /// </summary>
    public class FlexiblePathResolver : BaseResolver
    {
        private readonly IValueConverter converter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="converter"></param>
        public FlexiblePathResolver(IValueConverter converter) : base(converter)
        {
            this.converter = converter;
        }

        /// <summary>
        /// Get this property
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            var property = (new AttributePropertyPathResolver(converter).GetProperty(parentType, component) ?? 
                new ExactCasePropertyPathResolver(converter).GetProperty(parentType, component)) ?? 
                new CaseInsensitivePropertyPathResolver(converter).GetProperty(parentType, component);
            return property;
        }
    }
}
