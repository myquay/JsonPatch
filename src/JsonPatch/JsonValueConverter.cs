using JsonPatch.Paths;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    public class JsonValueConverter : IValueConverter
    {
        public object ConvertTo(object value, Type type)
        {
            if(type.IsPrimitive)
            {
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, value.ToString());
            }
            else
            {
                return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), type);
            }
        }
    }
}
