using System;

namespace JsonPatch.Extensions
{
    internal static class StringExtensions
    {
        internal static bool IsPositiveInteger(this string @string)
        {
            int n;

            bool isInteger = Int32.TryParse(@string, out n);

            if (!isInteger)
                return false;

            if (n < 0)
                return false;

            return true;
        }

        internal static int ToInt32(this string @string)
        {
            return Int32.Parse(@string);
        }
    }
}
