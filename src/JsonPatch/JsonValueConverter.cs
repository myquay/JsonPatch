using JsonPatch.Common.Paths;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    public class JsonValueConverter : IValueConverter
    {
        public object ConvertTo(object value, Type type)
        {
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), type);
        }
    }
}
