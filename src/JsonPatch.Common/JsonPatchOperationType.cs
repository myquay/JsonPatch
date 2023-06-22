using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    /// <summary>
    /// Json Patch Operation Type
    /// </summary>
    public enum JsonPatchOperationType
    {
        /// <summary>
        /// Add
        /// </summary>
        add = 0,

        /// <summary>
        /// Remove
        /// </summary>
        remove = 1,

        /// <summary>
        /// Replace
        /// </summary>
        replace = 2,

        /// <summary>
        /// Move
        /// </summary>
        move = 3,

        /// <summary>
        /// Test
        /// </summary>
        test = 4
    }
}
