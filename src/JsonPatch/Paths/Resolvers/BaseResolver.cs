using System;
using System.Collections;
using System.Linq;
using System.Reflection;

using JsonPatch.Extensions;
using JsonPatch.Helpers;
using JsonPatch.Paths.Components;
using Newtonsoft.Json;

namespace JsonPatch.Paths.Resolvers
{
    public abstract class BaseResolver : IPathResolver
    {
        internal abstract PropertyInfo GetProperty(Type parentType, string component);

        protected string[] GetPathComponents(string path)
        {
            // Normalize the path by ensuring it begins with a single forward slash, and has
            // no trailing slashes. Modify the path variable itself so that any character
            // positions we report in error messages are accurate.

            path = "/" + path.Trim('/');

            return path.Split('/').Skip(1).ToArray();
        }

        public PathComponent ParsePathComponent(string component, Type rootEntityType, PathComponent previous = null)
        {
            if (string.IsNullOrWhiteSpace(component))
            {
                throw new JsonPatchParseException("Path component may not be empty.");
            }

            // If the path component is a positive integer, it represents a collection index.
            if (component.IsPositiveInteger())
            {
                if (previous == null)
                {
                    throw new JsonPatchParseException("The first path component may not be a collection index.");
                }

                if (!previous.IsCollection)
                {
                    throw new JsonPatchParseException(string.Format(
                        "Collection index (\"{0}\") is not valid here because the previous path component (\"{1}\") " +
                        "does not represent a collection type.",
                        component, previous.Name));
                }

                return new CollectionIndexPathComponent(component)
                {
                    CollectionIndex = component.ToInt32(),
                    ComponentType = GetCollectionType(previous.ComponentType)
                };
            }

            // Otherwise, the path component represents a property name.

            // Attempt to retrieve the corresponding property.
            Type parentType = (previous == null) ? rootEntityType : previous.ComponentType;
            var property = GetProperty(parentType, component);

            if (property == null)
            {
                throw new JsonPatchParseException(string.Format("There is no property named \"{0}\" on type {1}.",
                    component, parentType.Name));
            }

            return new PropertyPathComponent(component)
            {
                PropertyInfo = property,
                ComponentType = property.PropertyType
            };
        }

        public PathComponent[] ParsePath(string path, Type entityType)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new JsonPatchParseException("Path may not be empty.");
            }

            var pathComponents = GetPathComponents(path);
            var parsedComponents = new PathComponent[pathComponents.Length];

            // Keep track of our current position in the path string (for error reporting).
            int pos = 1;
            for (int i = 0; i < pathComponents.Length; i++)
            {
                var pathComponent = pathComponents[i];

                try
                {
                    parsedComponents[i] = ParsePathComponent(pathComponent, entityType,
                        i > 0 ? parsedComponents[i - 1] : null);
                }
                catch (JsonPatchParseException e)
                {
                    throw new JsonPatchParseException(string.Format(
                        "The path \"{0}\" is not valid: {1}\n" +
                        "(Error occurred while parsing path component \"{2}\" at character position {3}.)",
                        path, e.Message, pathComponent, pos), e);
                }

                pos += pathComponent.Length + 1;
            }

            return parsedComponents;
        }

        public object GetValueFromPath(Type entityType, PathComponent[] pathComponents, object entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new JsonPatchException("The root object is null.");
                }

                object previous = entity;

                for (int i = 0; i < pathComponents.Length; i++)
                {
                    string parentPath = PathComponent.GetFullPath(pathComponents.Take(i));
                    var pathComponent = pathComponents[i];

                    TypeSwitch.On(pathComponent)
                        .Case((PropertyPathComponent component) =>
                        {
                            if (previous == null)
                            {
                                throw new JsonPatchException(string.Format(
                                    "Cannot get property \"{0}\" from null object at path \"{1}\".",
                                    component.Name, parentPath));
                            }

                            previous = component.PropertyInfo.GetValue(previous);
                        })
                        .Case((CollectionIndexPathComponent component) =>
                        {
                            try
                            {
                                var list = (IList)previous;
                                previous = list[component.CollectionIndex];
                            }
                            catch (Exception e)
                            {
                                throw new JsonPatchException(string.Format(
                                    "Cannot access index {0} from collection at path \"{1}\".",
                                    component.CollectionIndex, parentPath), e);
                            }
                        });
                }

                return previous;
            }
            catch (Exception e)
            {
                throw new JsonPatchException(string.Format(
                    "Failed to get value from path \"{0}\": {1}",
                    PathComponent.GetFullPath(pathComponents), e.Message), e);
            }
        }

        public void SetValueFromPath(Type entityType, PathComponent[] pathComponents, object entity, object value, JsonPatchOperationType operationType)
        {
            try
            {
                PathComponent[] parent = pathComponents.Take(pathComponents.Length - 1).ToArray();
                string parentPath = PathComponent.GetFullPath(parent);

                object previous = GetValueFromPath(entityType, parent, entity);

                if (previous == null)
                {
                    throw new JsonPatchException(string.Format("Value at parent path \"{0}\" is null.", parentPath));
                }

                var target = pathComponents.Last();

                TypeSwitch.On(target)
                    .Case((PropertyPathComponent component) =>
                    {
                        switch (operationType)
                        {
                            case JsonPatchOperationType.add:
                            case JsonPatchOperationType.replace:
                                component.PropertyInfo.SetValue(previous, ConvertValue(value, component.ComponentType));
                                break;
                            case JsonPatchOperationType.remove:
                                component.PropertyInfo.SetValue(previous, null);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("operationType");
                        }
                    })
                    .Case((CollectionIndexPathComponent component) =>
                    {
                        var list = previous as IList;
                        if (list == null)
                        {
                            throw new JsonPatchException(string.Format("Value at parent path \"{0}\" is not a valid collection.", parentPath));
                        }

                        switch (operationType)
                        {
                            case JsonPatchOperationType.add:
                                list.Insert(component.CollectionIndex, ConvertValue(value, component.ComponentType));
                                break;
                            case JsonPatchOperationType.remove:
                                list.RemoveAt(component.CollectionIndex);
                                break;
                            case JsonPatchOperationType.replace:
                                list[component.CollectionIndex] = ConvertValue(value, component.ComponentType);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("operationType");
                        }
                    });
            }
            catch (Exception e)
            {
                throw new JsonPatchException(string.Format(
                    "Failed to set value at path \"{0}\" while performing \"{1}\" operation: {2}",
                    PathComponent.GetFullPath(pathComponents), operationType, e.Message), e);
            }
        }

        private static Type GetCollectionType(Type entityType)
        {
            return entityType.GetElementType() ?? entityType.GetGenericArguments().First();
        }

        private static object ConvertValue(object value, Type type)
        {
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), type);
        }
    }
}
