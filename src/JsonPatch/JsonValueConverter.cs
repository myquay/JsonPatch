using JsonPatch.Paths;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace JsonPatch
{
    public class JsonValueConverter : IValueConverter
    {
        public object ConvertTo(object value, Type type)
        {
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
                return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), type);
            }
        }
    }
}
