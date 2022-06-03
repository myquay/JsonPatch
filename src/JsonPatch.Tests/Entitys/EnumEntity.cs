using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Tests.Entitys
{
    public class EnumEntity
    {
        public SampleEnum Foo { get; set; }
    }

    public enum SampleEnum
    {
        FirstEnum,
        SecondEnum,
        ThirdEnum,
    }
}
