using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Paths.Resolvers
{
    public class FlexiblePathResolver : BaseResolver
    {
        private IValueConverter converter;

        public FlexiblePathResolver(IValueConverter converter) : base(converter)
        {
            this.converter = converter;
        }

        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            var property = new AttributePropertyPathResolver(converter).GetProperty(parentType, component);
            if (property == null)
                property = new ExactCasePropertyPathResolver(converter).GetProperty(parentType, component);
            if (property == null)
                property = new CaseInsensitivePropertyPathResolver(converter).GetProperty(parentType, component);
            return property;
        }
    }
}
