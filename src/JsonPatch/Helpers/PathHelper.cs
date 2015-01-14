using Newtonsoft.Json;
using System;
using System.Collections;
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
                if(!typeof(IEnumerable).IsAssignableFrom(entityType))
                    return false;

                if (pathComponents.Length == 1)
                    return true;

                return IsPathValid(entityType.GetElementType() ?? entityType.GetGenericArguments().First(), String.Join("/", pathComponents.Skip(1)));
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
                //Cast it once to an iList to save CPU. 
                var listEntity = (IList)entity;
                var numberOfElements = (listEntity).Count;
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
                        //If this isn't an array, we can just insert it and return. 
                        if (listEntity is Array == false)
                        {
                            listEntity.Insert(accessIndex, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), entityType.GetGenericArguments().First()));
                            return listEntity;
                        }

                        var oldArray = listEntity;
                        IList newArray = (IList)Activator.CreateInstance(entityType, new object[] { (oldArray).Count + 1 });

                        for (int i = 0; i < newArray.Count; i++)
                        {
                            if (i < accessIndex)
                                newArray[i] = oldArray[i];
                            if (i == accessIndex)
                                newArray[i] = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), entityType.GetElementType());
                            if (i > accessIndex)
                                newArray[i] = oldArray[i - 1];
                        }

                        return newArray;
                    }
                    else if (operationType == JsonPatchOperationType.remove)
                    {
                        //If this isn't an array, we can just remove at and return. 
                        if (listEntity is Array == false)
                        {
                            listEntity.RemoveAt(accessIndex);
                            return listEntity;
                        }

                        var oldArray = listEntity;
                        var newArray = (IList)Activator.CreateInstance(entityType, new object[] { (oldArray).Count - 1 });

                        for (int i = 0; i < oldArray.Count; i++)
                        {
                            if (i < accessIndex)
                                newArray[i] = oldArray[i];
                            if (i > accessIndex)
                                newArray[i - 1] = oldArray[i];
                        }

                        return newArray;
                    }
                    else
                    {
                        listEntity[accessIndex] = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), entityType.GetElementType() ?? entityType.GetGenericArguments().First());
                        return entity;
                    }
                }

                var updatedEntity = SetValueFromPath(entityType.GetElementType() ?? entityType.GetGenericArguments().First(), String.Join("/", pathComponents.Skip(1)), listEntity[accessIndex], value, operationType);
                listEntity[accessIndex] = updatedEntity;

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
