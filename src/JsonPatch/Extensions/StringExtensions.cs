using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtensions
    {
        public static bool IsPositiveInteger(this string @string){
            int n;

            bool isInteger = Int32.TryParse(@string, out n);

            if (!isInteger)
                return false;

            if (n < 0)
                return false;

            return true;
        }

        public static int ToInt32(this string @string)
        {
            return Int32.Parse(@string);
        }
    }
}
