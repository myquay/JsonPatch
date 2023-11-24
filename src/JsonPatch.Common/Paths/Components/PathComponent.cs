using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JsonPatch.Paths.Components
{
    /// <summary>
    /// Represents a component of a JSON Pointer path.
    /// </summary>
    public class PathComponent
    {
        /// <summary>
        /// Path component constructor.
        /// </summary>
        /// <param name="name"></param>
        public PathComponent(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Component name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Component type
        /// </summary>
        public Type ComponentType { get; set; }

        /// <summary>
        /// Full path
        /// </summary>
        /// <param name="pathComponents"></param>
        /// <returns></returns>
        public static string GetFullPath(IEnumerable<PathComponent> pathComponents)
        {
            return "/" + string.Join("/", pathComponents.Select(p => p.Name));
        }

        /// <summary>
        /// Is collection
        /// </summary>
        public bool IsCollection
        {
            get
            {
                return typeof(IEnumerable<>).IsAssignableFrom(ComponentType) ||
                       typeof(IEnumerable).IsAssignableFrom(ComponentType);
            }
        }

        /// <summary>
        /// Is dictionary
        /// </summary>
        public bool IsDictionary
        {
            get
            {
                return typeof(IDictionary).IsAssignableFrom(ComponentType) ||
                   (ComponentType.IsGenericType && typeof(IDictionary<,>).IsAssignableFrom(ComponentType.GetGenericTypeDefinition()));
            }
        }
    }
}
