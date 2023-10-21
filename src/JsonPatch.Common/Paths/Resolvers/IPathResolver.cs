using JsonPatch.Paths.Components;
using System;

namespace JsonPatch.Paths.Resolvers
{
    /// <summary>
    /// Path resolver interface
    /// </summary>
    public interface IPathResolver
    {
        /// <summary>
        /// Parse the path into components
        /// </summary>
        /// <param name="path"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        PathComponent[] ParsePath(string path, Type entityType);

        /// <summary>
        /// Parse a single path component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="rootEntityType"></param>
        /// <param name="previous"></param>
        /// <returns></returns>
        PathComponent ParsePathComponent(string component, Type rootEntityType, PathComponent previous = null);

        /// <summary>
        /// Get the value from the path
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="pathComponents"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        object GetValueFromPath(Type entityType, PathComponent[] pathComponents, object entity);

        /// <summary>
        /// Set the value from the path
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="pathComponents"></param>
        /// <param name="entity"></param>
        /// <param name="value"></param>
        /// <param name="operationType"></param>
        void SetValueFromPath(Type entityType, PathComponent[] pathComponents, object entity, object value,
            JsonPatchOperationType operationType);
    }
}
