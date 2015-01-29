using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JsonPatch.Paths.Components
{
    public class PathComponent
    {
        public PathComponent(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public Type ComponentType { get; set; }

        public bool IsCollection
        {
            get
            {
                return typeof(IEnumerable<>).IsAssignableFrom(ComponentType) ||
                       typeof(IEnumerable).IsAssignableFrom(ComponentType);
            }
        }

        public static string GetFullPath(IEnumerable<PathComponent> pathComponents)
        {
            return "/" + string.Join("/", pathComponents.Select(p => p.Name));
        }
    }
}
