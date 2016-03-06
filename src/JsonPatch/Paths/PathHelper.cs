using System;
using JsonPatch.Formatting;
using JsonPatch.Paths.Components;
using JsonPatch.Paths.Resolvers;

namespace JsonPatch.Paths
{
    public class PathHelper
    {
        internal static IPathResolver PathResolver
        {
            get
            {
                return JsonPatchFormatter.Settings == null
                    ? JsonPatchSettings.DefaultPatchSettings().PathResolver
                    : JsonPatchFormatter.Settings.PathResolver;
            }
        }

        #region Parse/Validate Paths
        
        public static bool IsPathValid(Type entityType, string path)
        {
            try
            {
                ParsePath(path, entityType);
                return true;
            }
            catch (JsonPatchParseException)
            {
                return false;
            }
        }

        public static PathComponent[] ParsePath(string path, Type entityType)
        {
            return PathResolver.ParsePath(path, entityType);
            }

        #endregion

        #region GetValueFromPath

        public static object GetValueFromPath(Type entityType, string path, object entity)
        {
            return GetValueFromPath(entityType, ParsePath(path, entityType), entity);
        }

        public static object GetValueFromPath(Type entityType, PathComponent[] pathComponents, object entity)
        {
            return PathResolver.GetValueFromPath(entityType, pathComponents, entity);
                }

        #endregion

        #region SetValueFromPath

        public static void SetValueFromPath(Type entityType, string path, object entity, object value, JsonPatchOperationType operationType)
        {
            SetValueFromPath(entityType, ParsePath(path, entityType), entity, value, operationType);
        }

        public static void SetValueFromPath(Type entityType, PathComponent[] pathComponents, object entity, object value, JsonPatchOperationType operationType)
        {
            PathResolver.SetValueFromPath(entityType, pathComponents, entity, value, operationType);
        }

        #endregion
    }
}
