using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    public enum JsonPatchOperationType
    {
        add = 0,
        remove = 1,
        replace = 2,
        move = 3
    }
}
