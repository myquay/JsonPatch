using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonPatch
{
    public class JsonPatchDocument<TEntity> : IJsonPatchDocument where TEntity : class, new()
    {

        private List<JsonPatchOperation> _operations = new List<JsonPatchOperation>();

        public void Add(string path, object value)
        {
            if (GetPropertyFromPath(typeof(TEntity), path) == null)
            {
                throw new JsonPatchParseException(String.Format("The path '{0}' is not valid.", path));
            }

            _operations.Add(new JsonPatchOperation
            {
                Operation = JsonPatchOperationType.add,
                PropertyName = path,
                Value = value
            });
        }

        public void Replace(string path, object value)
        {
            if (GetPropertyFromPath(typeof(TEntity), path) == null)
            {
                throw new JsonPatchParseException(String.Format("The path '{0}' is not valid.", path));
            }

            _operations.Add(new JsonPatchOperation
            {
                Operation = JsonPatchOperationType.replace,
                PropertyName = path,
                Value = value
            });
        }

        public void Remove(string path)
        {
            if (GetPropertyFromPath(typeof(TEntity), path) == null)
            {
                throw new JsonPatchParseException(String.Format("The path '{0}' is not valid.", path));
            }
            

            _operations.Add(new JsonPatchOperation
            {
                Operation = JsonPatchOperationType.remove,
                PropertyName = path
            });
        }

        private PropertyInfo GetPropertyFromPath(Type entityType, string path)
        {
            string[] properties = path.Trim('/').Split('/');

            if (entityType.GetProperties().Any(p => p.Name == properties[0]))
            {
                var property = entityType.GetProperties().Single(p => p.Name == properties[0]);
                if (properties.Length == 1)
                    return property;

                return GetPropertyFromPath(property.PropertyType, String.Join(".", properties.Skip(1)));
            }
            else
            {
                return null;
            }
        }

        private void SetPropertyFromPath(Type entityType, string path, object entity, object value)
        {
            string[] properties = path.Trim('/').Split('/');

            if (entityType.GetProperties().Any(p => p.Name == properties[0]))
            {
                var property = entityType.GetProperties().Single(p => p.Name == properties[0]);
                
                if (properties.Length == 1)
                    property.SetValue(entity, value);

                if(property.GetValue(entity) == null){
                    property.SetValue(entity, Activator.CreateInstance(property.PropertyType));
                }

                SetPropertyFromPath(property.PropertyType, String.Join(".", properties.Skip(1)), property.GetValue(entity), value);
            }
        }

        public void ApplyUpdatesTo(TEntity entity)
        {
            foreach (var operation in _operations)
            {
                var property = GetPropertyFromPath(typeof(TEntity), operation.PropertyName);
                if (operation.Operation == JsonPatchOperationType.remove)
                {
                    SetPropertyFromPath(typeof(TEntity), operation.PropertyName, entity, null);
                }
                else if (operation.Operation == JsonPatchOperationType.replace)
                {
                    SetPropertyFromPath(typeof(TEntity), operation.PropertyName, entity, operation.Value);
                }
                else if (operation.Operation == JsonPatchOperationType.add)
                {
                    SetPropertyFromPath(typeof(TEntity), operation.PropertyName, entity, operation.Value);
                }
            }
        }
    }
}
