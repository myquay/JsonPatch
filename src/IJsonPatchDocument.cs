using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    public interface IJsonPatchDocument
    {
        void Add(string path, object value);
        void Replace(string path, object value);
        void Remove(string path);
    }
}
