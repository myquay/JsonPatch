using System.Collections.Generic;

namespace JsonPatch.Tests.Entities
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
