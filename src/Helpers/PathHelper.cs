using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch.Helpers
{
    public class PathHelper
    {
        public static bool IsPathValid(Type entityType, string path)
        {
            if (String.IsNullOrEmpty(path))
                return false;

            string[] pathComponents = path.Trim('/').Split('/');

            if (String.IsNullOrEmpty(pathComponents[0]) || pathComponents[0].IsPositiveInteger())
                return false;

            var currentPathComponent = pathComponents[0];

            if (entityType.GetProperties().Any(p => p.Name == pathComponents[0]))
            {
                var property = entityType.GetProperties().Single(p => p.Name == pathComponents[0]);
                if (pathComponents.Length == 1)
                    return true;

                entityType = property.PropertyType;

                while (pathComponents[1].IsPositiveInteger())
                {
                    if (!entityType.IsArray)
                    {
                        return false;
                    }
                    else
                    {
                        entityType = entityType.GetElementType();
                        pathComponents = pathComponents.Skip(1).ToArray();

                        if (pathComponents.Length == 1)
                            return true;
                    }
                }

                return IsPathValid(entityType, String.Join("/", pathComponents.Skip(1)));
            }
            else
            {
                return false;
            }
        }

        public static void SetValueFromPath(Type entityType, string path, object entity, string value)
        {
            string[] properties = path.Trim('/').Split('/');

            if (entityType.GetProperties().Any(p => p.Name == properties[0]))
            {
                var property = entityType.GetProperties().Single(p => p.Name == properties[0]);

                if (properties.Length == 1){
                    property.SetValue(entity, value);
                    return;
                }

                if (property.GetValue(entity) == null)
                {
                    property.SetValue(entity, Activator.CreateInstance(property.PropertyType));
                }

                SetValueFromPath(property.PropertyType, String.Join("/", properties.Skip(1)), property.GetValue(entity), value);
            }
        }
    }
}
