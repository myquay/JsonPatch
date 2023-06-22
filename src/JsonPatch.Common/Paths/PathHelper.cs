using System;
using JsonPatch.Paths.Components;
using JsonPatch.Paths.Resolvers;

namespace JsonPatch.Paths
{
    /// <summary>
    /// Path helper
    /// </summary>
    public static class PathHelper
    {
        #region Parse/Validate Paths

        /// <summary>
        /// Check if a path is valid for a given type
        /// </summary>
        /// <param name="pathResolver"></param>
        /// <param name="entityType"></param>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the path components for a given path
        /// </summary>
        /// <param name="pathResolver"></param>
        /// <param name="path"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static PathComponent[] ParsePath(this IPathResolver pathResolver, string path, Type entityType)
        {
            return pathResolver.ParsePath(path, entityType);
        }

        #endregion

        #region GetValueFromPath

        /// <summary>
        /// Get the value from a path
        /// </summary>
        /// <param name="pathResolver"></param>
        /// <param name="entityType"></param>
        /// <param name="path"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object GetValueFromPath(this IPathResolver pathResolver, Type entityType, string path, object entity)
        {
            return GetValueFromPath(pathResolver, entityType, ParsePath(pathResolver, path, entityType), entity);
        }

        /// <summary>
        /// Get the value from a path
        /// </summary>
        /// <param name="pathResolver"></param>
        /// <param name="entityType"></param>
        /// <param name="pathComponents"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object GetValueFromPath(this IPathResolver pathResolver, Type entityType, PathComponent[] pathComponents, object entity)
        {
            return pathResolver.GetValueFromPath(entityType, pathComponents, entity);
        }

        #endregion

        #region SetValueFromPath

        /// <summary>
        /// Set the value from a path
        /// </summary>
        /// <param name="pathResolver"></param>
        /// <param name="entityType"></param>
        /// <param name="path"></param>
        /// <param name="entity"></param>
        /// <param name="value"></param>
        /// <param name="operationType"></param>
        public static void SetValueFromPath(this IPathResolver pathResolver, Type entityType, string path, object entity, object value, JsonPatchOperationType operationType)
        {
            SetValueFromPath(pathResolver, entityType, ParsePath(pathResolver, path, entityType), entity, value, operationType);
        }

        /// <summary>
        /// Set the value from a path
        /// </summary>
        /// <param name="pathResolver"></param>
        /// <param name="entityType"></param>
        /// <param name="pathComponents"></param>
        /// <param name="entity"></param>
        /// <param name="value"></param>
        /// <param name="operationType"></param>
        public static void SetValueFromPath(this IPathResolver pathResolver, Type entityType, PathComponent[] pathComponents, object entity, object value, JsonPatchOperationType operationType)
        {
            pathResolver.SetValueFromPath(entityType, pathComponents, entity, value, operationType);
        }

        #endregion
    }
}
