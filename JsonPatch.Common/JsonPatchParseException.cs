using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    public class JsonPatchParseException : Exception
    {
        public JsonPatchParseException(string message) : base(message) { }
        public JsonPatchParseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
