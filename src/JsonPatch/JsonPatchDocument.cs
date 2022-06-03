using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonPatch.Common;
using JsonPatch.Common.Paths;
using JsonPatch.Common.Paths.Resolvers;
using JsonPatch.Formatting;

namespace JsonPatch
{
    public class JsonPatchDocument<TEntity> : IJsonPatchDocument where TEntity : class, new()
    {

        private IPathResolver resolver
        {
            get
            {
                return JsonPatchFormatter.Settings?.PathResolver ?? JsonPatchSettings.DefaultPatchSettings()?.PathResolver;
            }
        }

        private List<JsonPatchOperation> _operations = new List<JsonPatchOperation>();

        public List<JsonPatchOperation> Operations { get { return _operations; } }

        public bool HasOperations { get { return _operations.Count > 0; } }

        public void Add(string path, object value)
        {
            _operations.Add(new JsonPatchOperation
            {
                Operation = JsonPatchOperationType.add,
                Path = path,
                ParsedPath = resolver.ParsePath(path, typeof(TEntity)),
                Value = value
            });
        }

        public void Replace(string path, object value)
        {
            _operations.Add(new JsonPatchOperation
            {
                Operation = JsonPatchOperationType.replace,
                Path = path,
                ParsedPath = resolver.ParsePath(path, typeof(TEntity)),
                Value = value
            });
        }

        public void Remove(string path)
        {
            _operations.Add(new JsonPatchOperation
            {
                Operation = JsonPatchOperationType.remove,
                Path = path,
                ParsedPath = resolver.ParsePath(path, typeof(TEntity))
            });
        }

        public void Move(string from, string path)
        {
            _operations.Add(new JsonPatchOperation
            {
                Operation = JsonPatchOperationType.move,
                FromPath = from,
                ParsedFromPath = resolver.ParsePath(from, typeof(TEntity)),
                Path = path,
                ParsedPath = resolver.ParsePath(path, typeof(TEntity)),
            });
        }

        public void ApplyUpdatesTo(TEntity entity)
        {
            foreach (var operation in _operations)
            {
                switch (operation.Operation)
                {
                    case JsonPatchOperationType.remove:
                        resolver.SetValueFromPath(typeof(TEntity), operation.ParsedPath, entity, null, JsonPatchOperationType.remove);
                        break;
                    case JsonPatchOperationType.replace:
                        resolver.SetValueFromPath(typeof(TEntity), operation.ParsedPath, entity, operation.Value, JsonPatchOperationType.replace);
                        break;
                    case JsonPatchOperationType.add:
                        resolver.SetValueFromPath(typeof(TEntity), operation.ParsedPath, entity, operation.Value, JsonPatchOperationType.add);
                        break;
                    case JsonPatchOperationType.move:
                        var value = resolver.GetValueFromPath(typeof(TEntity), operation.ParsedFromPath, entity);
                        resolver.SetValueFromPath(typeof(TEntity), operation.ParsedFromPath, entity, null, JsonPatchOperationType.remove);
                        resolver.SetValueFromPath(typeof(TEntity), operation.ParsedPath, entity, value, JsonPatchOperationType.add);
                        break;
                    default:
                        throw new NotSupportedException("Operation not supported: " + operation.Operation);
                }
            }
        }
    }
}
