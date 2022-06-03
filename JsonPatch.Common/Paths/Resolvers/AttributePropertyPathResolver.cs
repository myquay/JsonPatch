using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JsonPatch.Common.Paths.Resolvers
{
    public class AttributePropertyPathResolver : BaseResolver
    {
        internal override PropertyInfo GetProperty(Type parentType, string component)
        {
            var dataMemberPropertyMatch = parentType.GetProperties().FirstOrDefault(p =>
            {
                var dataMember = p.GetCustomAttributes<DataMemberAttribute>().FirstOrDefault();
                if (dataMember == null) return false;
                return string.Equals(dataMember.Name, component);
            });

            if (dataMemberPropertyMatch != null)
                return dataMemberPropertyMatch;

            return parentType.GetProperties().FirstOrDefault(p =>
            {
                var jsonProperty = p.GetCustomAttributes<JsonPropertyNameAttribute>().FirstOrDefault();
                if (jsonProperty == null) return false;
                return string.Equals(jsonProperty.Name, component);
            });
        }
    }
}
