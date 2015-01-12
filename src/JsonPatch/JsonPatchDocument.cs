using JsonPatch.Helpers;
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

        public List<JsonPatchOperation> Operations { get { return _operations; } }

        public bool HasOperations { get { return _operations.Count > 0; } }

        public void Add(string path, object value)
        {
            UpdateOperations(path, JsonPatchOperationType.add, value);
        }

        public void Replace(string path, object value)
        {
            UpdateOperations(path, JsonPatchOperationType.replace, value);
        }

        public void Remove(string path)
        {
            UpdateOperations(path, JsonPatchOperationType.remove);
        }

        public void ApplyUpdatesTo(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            foreach (var operation in _operations)
            {
                if (operation.Operation == JsonPatchOperationType.remove)
                {
                    PathHelper.SetValueFromPath(typeof(TEntity), operation.PropertyName, entity, null, JsonPatchOperationType.remove);
                }
                else if (operation.Operation == JsonPatchOperationType.replace)
                {
                    PathHelper.SetValueFromPath(typeof(TEntity), operation.PropertyName, entity, operation.Value, JsonPatchOperationType.replace);
                }
                else if (operation.Operation == JsonPatchOperationType.add)
                {
                    PathHelper.SetValueFromPath(typeof(TEntity), operation.PropertyName, entity, operation.Value, JsonPatchOperationType.add);
                }
            }
        }

        private void UpdateOperations(string property, JsonPatchOperationType operation, object value = null)
        {
            if (!PathHelper.IsPathValid(typeof(TEntity), property))
            {
                throw new JsonPatchParseException(String.Format("The path '{0}' is not valid.", property));
            }

            _operations.Add(new JsonPatchOperation
            {
                Operation = operation,
                PropertyName = property,
                Value = value
            });
        }
    }
}
