using System.Collections.Generic;

namespace JsonPatch.Tests.Entities
{
	public class DictionaryEntity<TKey>
	{
		public IDictionary<TKey, string> Foo { get; set; }
	}
}
