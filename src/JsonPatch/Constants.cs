using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    public static class Constants
    {
        public static class Operations
        {
            public const string ADD = "add";
            public const string REMOVE = "remove";
            public const string REPLACE = "replace";
            public const string MOVE = "move";
        }
    }
}
