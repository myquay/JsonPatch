using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Tests.Entities
{
	public class DictionaryEntity<TKey>
	{
		public Dictionary<TKey, string> Foo { get; set; }
	}
}
