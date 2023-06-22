using System;
using JsonPatch.Paths.Components;
using JsonPatch.Paths.Resolvers;

namespace JsonPatch.Paths
{
    public static class PathHelper
    {
        #region Parse/Validate Paths

        public static bool IsPathValid(this IPathResolver pathResolver, Type entityType, string path)
        {
            try
            {
                ParsePath(pathResolver, path, entityType);
                return true;
            }
            catch (JsonPatchParseException)
            {
                return false;
            }
        }

        public static PathComponent[] ParsePath(this IPathResolver pathResolver, string path, Type entityType)
        {
            return pathResolver.ParsePath(path, entityType);
        }

        #endregion

        #region GetValueFromPath

        public static object GetValueFromPath(this IPathResolver pathResolver, Type entityType, string path, object entity)
        {
            return GetValueFromPath(pathResolver, entityType, ParsePath(pathResolver, path, entityType), entity);
        }

        public static object GetValueFromPath(this IPathResolver pathResolver, Type entityType, PathComponent[] pathComponents, object entity)
        {
            return pathResolver.GetValueFromPath(entityType, pathComponents, entity);
        }

        #endregion

        #region SetValueFromPath

        public static void SetValueFromPath(this IPathResolver pathResolver, Type entityType, string path, object entity, object value, JsonPatchOperationType operationType)
        {
            SetValueFromPath(pathResolver, entityType, ParsePath(pathResolver, path, entityType), entity, value, operationType);
        }

        public static void SetValueFromPath(this IPathResolver pathResolver, Type entityType, PathComponent[] pathComponents, object entity, object value, JsonPatchOperationType operationType)
        {
            pathResolver.SetValueFromPath(entityType, pathComponents, entity, value, operationType);
        }

        #endregion
    }
}
