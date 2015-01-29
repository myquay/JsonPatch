using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Tests.Entitys
{
    public class ComplexEntity
    {
        public ArrayEntity Foo { get; set; }
        public SimpleEntity Bar { get; set; }
        public SimpleEntity[] Baz { get; set; }
        public List<SimpleEntity> Qux { get; set; }
        public List<ListEntity> Norf { get; set; }
    }
}
