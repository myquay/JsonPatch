using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    /// <summary>
    /// Exception raised when an error occurs while parsing a JSON Patch document
    /// </summary>
    public class JsonPatchParseException : Exception
    {
        /// <summary>
        /// New instance of JsonPatchParseException with message
        /// </summary>
        /// <param name="message"></param>
        public JsonPatchParseException(string message) : base(message) { }

        /// <summary>
        /// New instance of JsonPatchParseException with message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public JsonPatchParseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
