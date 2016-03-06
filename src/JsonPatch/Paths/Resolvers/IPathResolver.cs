using System;

using JsonPatch.Paths.Components;

namespace JsonPatch.Paths.Resolvers
{
    public interface IPathResolver
    {
        PathComponent[] ParsePath(string path, Type entityType);

        PathComponent ParsePathComponent(string component, Type rootEntityType, PathComponent previous = null);

        object GetValueFromPath(Type entityType, PathComponent[] pathComponents, object entity);

        void SetValueFromPath(Type entityType, PathComponent[] pathComponents, object entity, object value,
            JsonPatchOperationType operationType);
    }
}
