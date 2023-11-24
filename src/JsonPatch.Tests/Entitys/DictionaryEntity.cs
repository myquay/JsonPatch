﻿using System.Collections.Generic;

namespace JsonPatch.Tests.Entities
{
	public class DictionaryEntity<TKey>
	{
        public Dictionary<TKey, string> Foo { get; set; }

        public IDictionary<TKey, string> Bar { get; set; }
    }
}
