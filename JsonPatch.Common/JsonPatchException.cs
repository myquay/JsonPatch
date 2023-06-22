using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    /// <summary>
    /// Raised when an error occurs while processing changes
    /// </summary>
    public class JsonPatchException : Exception
    {
        /// <summary>
        /// New instance of JsonPatchException with message
        /// </summary>
        /// <param name="message"></param>
        public JsonPatchException(string message) : base(message) { }

        /// <summary>
        /// New instance of JsonPatchException with message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public JsonPatchException(string message, Exception innerException) : base(message, innerException) { }
    }
}
