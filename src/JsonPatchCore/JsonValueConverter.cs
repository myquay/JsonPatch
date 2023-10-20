using JsonPatch.Paths;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonPatch {

    /// <summary>
    /// Converts a value from one type to another using Json serialisation as a fallback
    /// </summary>
    public class JsonValueConverter : IValueConverter
    {
        /// <summary>
        /// Convert from one object to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ConvertTo(object value, Type type)
        {
#pragma warning disable CS8604, CS8603 
            if (type.IsPrimitive && value is IConvertible)
            {
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            else if (Nullable.GetUnderlyingType(type)?.IsPrimitive == true && value is IConvertible)
            {
                if (value == null)
                    return null;
                return Convert.ChangeType(value, Nullable.GetUnderlyingType(type), CultureInfo.InvariantCulture);
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, value.ToString());
            }
            else if (Nullable.GetUnderlyingType(type)?.IsEnum == true)
            {
                if (value == null)
                    return null;
                return Enum.Parse(Nullable.GetUnderlyingType(type), value.ToString());
            }
            else
            {
                return JsonSerializer.Deserialize(JsonSerializer.Serialize(value), type);
            }
#pragma warning restore CS8604, CS8603 
        }
    }
}
