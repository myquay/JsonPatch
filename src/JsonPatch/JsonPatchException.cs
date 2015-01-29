using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    public class JsonPatchException : Exception
    {
        public JsonPatchException(string message) : base(message) { }
        public JsonPatchException(string message, Exception innerException) : base(message, innerException) { }
    }
}
