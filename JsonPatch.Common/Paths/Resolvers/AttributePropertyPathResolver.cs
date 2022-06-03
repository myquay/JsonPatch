using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Common.Paths.Resolvers
{
    public class AttributePropertyPathResolver : BaseResolver
    {
        public AttributePropertyPathResolver(IValueConverter converter) : base(converter)
        {  }


        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            var dataMemberPropertyMatch = parentType.GetProperties().FirstOrDefault(p =>
            {
                var dataMember = p.GetCustomAttributes<DataMemberAttribute>().FirstOrDefault();
                if (dataMember == null) return false;
                return string.Equals(dataMember.Name, component);
            });

            return dataMemberPropertyMatch;
        }
    }
}
