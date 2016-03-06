using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonPatch.Tests.Entitys
{
    public class SimpleEntity
    {
        public string Foo { get; set; }
        public int Bar { get; set; }
        public string Baz { get; set; }

        [DataMember(Name = "pId")]       
        public string ParId { get; set; }

        [JsonProperty("jsonProperty")]
        public string Car { get; set; }
    }
}
