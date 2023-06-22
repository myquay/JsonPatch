using JsonPatch.Paths;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonPatch {
    public class JsonValueConverter : IValueConverter
    {
        public object ConvertTo(object value, Type type)
        {
#pragma warning disable CS8603 // Possible null reference return.
            if (type.IsPrimitive)
            {
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, value.ToString());
            }
            else
            {
                return JsonSerializer.Deserialize(JsonSerializer.Serialize(value), type);
            }
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
