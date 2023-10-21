using System;

namespace JsonPatch.Paths
{
    /// <summary>
    /// Value converter interface
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Convert from source type to target type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        object ConvertTo(object value, Type target);
    }
}
