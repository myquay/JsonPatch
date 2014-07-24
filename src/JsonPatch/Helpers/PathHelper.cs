using Newtonsoft.Json;
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

            if (String.IsNullOrEmpty(pathComponents[0]))
                return false;

            var currentPathComponent = pathComponents[0];

            if (currentPathComponent.IsPositiveInteger())
            {
                if(!entityType.IsArray)
                    return false;

                if (pathComponents.Length == 1)
                    return true;

                return IsPathValid(entityType.GetElementType(), String.Join("/", pathComponents.Skip(1)));
            }
            else if (entityType.GetProperties().Any(p => p.Name == pathComponents[0]))
            {
                var property = entityType.GetProperties().Single(p => p.Name == pathComponents[0]);
                if (pathComponents.Length == 1)
                    return true;

                return IsPathValid(property.PropertyType, String.Join("/", pathComponents.Skip(1)));
            }
            else
            {
                return false;
            }
        }

        public static object SetValueFromPath(Type entityType, string path, object entity, object value, JsonPatchOperationType operationType)
        {
            if (!IsPathValid(entityType, path))
                throw new JsonPatchException(String.Format("The path specified ('{0}') is invalid", path));

            string[] pathComponents = path.Trim('/').Split('/');

            var currentPathComponent = pathComponents[0];

            if (currentPathComponent.IsPositiveInteger())
            {
                var numberOfElements = ((Array)entity).Length;
                var accessIndex = currentPathComponent.ToInt32();

                if (operationType == JsonPatchOperationType.add && pathComponents.Length == 1)
                    numberOfElements++; //We can add to the end of an array

                if (accessIndex >= numberOfElements)
                {
                    throw new JsonPatchException(String.Format(
                        "Index out of bounds: The array has '{0}' elements, attempted to {1} at {2}",
                        numberOfElements,
                        operationType,
                        pathComponents[0]));
                }

                //If we're on the last component, try set the value in the array
                if (pathComponents.Length == 1)
                {
                    if (operationType == JsonPatchOperationType.add)
                    {
                        var oldArray = ((Array)entity);
                        var newArray = (Array)Activator.CreateInstance(entityType, new object[] { ((Array)oldArray).Length + 1 });

                        for (int i = 0; i < newArray.Length; i++)
                        {
                            if (i < accessIndex)
                                newArray.SetValue(oldArray.GetValue(i), i);
                            if (i == accessIndex)
                                newArray.SetValue(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), entityType.GetElementType()), i);
                            if (i > accessIndex)
                                newArray.SetValue(oldArray.GetValue(i - 1), i);
                        }

                        return newArray;
                    }
                    else if (operationType == JsonPatchOperationType.remove)
                    {
                        var oldArray = ((Array)entity);
                        var newArray = (Array)Activator.CreateInstance(entityType, new object[] { ((Array)oldArray).Length - 1 });

                        for (int i = 0; i < oldArray.Length; i++)
                        {
                            if (i < accessIndex)
                                newArray.SetValue(oldArray.GetValue(i), i);
                            if (i > accessIndex)
                                newArray.SetValue(oldArray.GetValue(i), i - 1);
                        }

                        return newArray;
                    }
                    else
                    {
                        ((Array)entity).SetValue(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), entityType.GetElementType()), accessIndex);
                        return entity;
                    }
                }

                var updatedEntity = SetValueFromPath(entityType.GetElementType(), String.Join("/", pathComponents.Skip(1)), ((Array)entity).GetValue(accessIndex), value, operationType);
                ((Array)entity).SetValue(updatedEntity, accessIndex);

            }else if (entityType.GetProperties().Any(p => p.Name == pathComponents[0]))
            {
                var property = entityType.GetProperties().Single(p => p.Name == pathComponents[0]);

                if (pathComponents.Length == 1)
                {
                    if (operationType == JsonPatchOperationType.add && property.GetValue(entity) != null)
                        throw new JsonPatchException("You are trying to perform an add operation on a property that already has a value.");

                    property.SetValue(entity, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), property.PropertyType));
                    return entity;
                }

                //If we're still traversing the path, make sure we've instantiated objects along the way
                var propertyValue = property.GetValue(entity);
                var propertyType = property.PropertyType;

                if (propertyValue == null)
                {
                    if (property.PropertyType.IsArray)
                    {
                        propertyValue = Activator.CreateInstance(property.PropertyType, new object[] { 0 });
                    }
                    else
                    {
                        propertyValue = Activator.CreateInstance(property.PropertyType);
                    }
                }

                propertyValue = SetValueFromPath(propertyType, String.Join("/", pathComponents.Skip(1)), propertyValue, value, operationType);
                property.SetValue(entity, propertyValue);
            }

            return entity;
        }
    }
}
