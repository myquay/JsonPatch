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

        public void Add(string path, String value)
        {
            if (!PathHelper.IsPathValid(typeof(TEntity), path))
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

        public void Replace(string path, String value)
        {
            if (!PathHelper.IsPathValid(typeof(TEntity), path))
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
            if (!PathHelper.IsPathValid(typeof(TEntity), path))
            {
                throw new JsonPatchParseException(String.Format("The path '{0}' is not valid.", path));
            }
            

            _operations.Add(new JsonPatchOperation
            {
                Operation = JsonPatchOperationType.remove,
                PropertyName = path
            });
        }

        public void ApplyUpdatesTo(TEntity entity)
        {
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
    }
}
