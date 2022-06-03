using JsonPatch.Common.Paths.Components;
using System;


namespace JsonPatch.Common.Paths.Resolvers
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
