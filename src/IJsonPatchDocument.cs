using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    public interface IJsonPatchDocument
    {
        void Add(string path, String value);
        void Replace(string path, String value);
        void Remove(string path);
    }
}
