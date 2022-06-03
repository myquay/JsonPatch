using JsonPatch.Common.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonPatchCore
{
    public class JsonValueConverter : IValueConverter
    {
        public object ConvertTo(object value, Type type)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return JsonSerializer.Deserialize(JsonSerializer.Serialize(value), type);
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
