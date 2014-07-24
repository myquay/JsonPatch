using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    public class JsonPatchOperation
    {
        public JsonPatchOperationType Operation { get; set; }
        public String PropertyName { get; set; }
        public String Value { get; set; }
    }
}
