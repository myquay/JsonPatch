using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    /// <summary>
    /// Constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Operation names
        /// </summary>
        public static class Operations
        {
            /// <summary>
            /// Adds a value to an object or inserts it into an array. 
            /// </summary>
            public const string ADD = "add";

            /// <summary>
            /// Removes the element of the array, or member from an object
            /// </summary>
            public const string REMOVE = "remove";

            /// <summary>
            /// Replaces a value. Equivalent to a "remove" followed by an "add".
            /// </summary>
            public const string REPLACE = "replace";

            /// <summary>
            /// Moves a value from one location to the other.
            /// </summary>
            public const string MOVE = "move";

        }
    }
}
