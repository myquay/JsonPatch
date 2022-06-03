using System;
using System.Collections.Generic;
using System.Text;

namespace JsonPatch.Common.Paths
{
    public interface IValueConverter
    {
        object ConvertTo(object value, Type target);
    }
}
