using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Model
{
    public class PatchOperation
    {
        public string op { get; set; }
        public string path { get; set; }
        public string value { get; set; }
    }
}
