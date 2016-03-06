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
        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            var property = new AttributePropertyPathResolver().GetProperty(parentType, component);
            if (property == null)
                property = new ExactCasePropertyPathResolver().GetProperty(parentType, component);
            if (property == null)
                property = new CaseInsensitivePropertyPathResolver().GetProperty(parentType, component);
            return property;
        }
    }
}
