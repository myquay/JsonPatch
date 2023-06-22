using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JsonPatch.Tests.Entities
{
    public class SimpleEntity
    {
        public string Foo { get; set; }
        public int Bar { get; set; }
        public string Baz { get; set; }

        [DataMember(Name = "pId")]       
        public string ParId { get; set; }

        [JsonPropertyName("jsonProperty")]
        public string Car { get; set; }

        public bool BooleanValue { get; set; }
    }
}
