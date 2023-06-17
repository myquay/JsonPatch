using Common = JsonPatch.Common;
using JsonPatch.Common.Paths.Resolvers;
using JsonPatchCore;
using JsonPatchCore.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Text.Json;
using JsonPatch.Common.Model;

namespace JsonPatchCore;

public class JsonPatchDocument<TEntity> : IJsonPatchDocument<TEntity> where TEntity : class
{
    private readonly List<JsonPatchOperation> _operations = new();
    private readonly IPathResolver resolver = JsonPatchSettings.Options.PathResolver;

    public TEntity ApplyTo(TEntity entity)
    {
        foreach (var operation in _operations)
        {
            var parsedPath = resolver.ParsePath(operation.Path, typeof(TEntity));
            switch (operation.Type)
            {
                case JsonPatchOperationType.Remove:
                    resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, null, Common.JsonPatchOperationType.remove);
                    break;
                case JsonPatchOperationType.Replace:
                    resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, operation.Value, Common.JsonPatchOperationType.replace);
                    break;
                case JsonPatchOperationType.Add:
                    resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, operation.Value, Common.JsonPatchOperationType.add);
                    break;
                case JsonPatchOperationType.Move:
                    var value = resolver.GetValueFromPath(typeof(TEntity), parsedPath, entity);
                    resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, null, Common.JsonPatchOperationType.remove);
                    resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, value, Common.JsonPatchOperationType.add);
                    break;
                default:
                    throw new NotSupportedException("Operation not supported: " + operation.Type);
            }
        }

        return entity;
    }

    public static ValueTask<JsonPatchDocument<TEntity>?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        return BindAsync(httpContext);
    }

    public static async ValueTask<JsonPatchDocument<TEntity>?> BindAsync(HttpContext httpContext)
    {
        if (JsonPatchSettings.Options.RequireJsonPatchContentType && httpContext.Request.ContentType != "application/json-patch+json")
            throw new Common.JsonPatchParseException("The request content type must be 'application/json-patch+json'");

        using (var sr = new StreamReader(httpContext.Request.Body))
        {
            var parsedDocument = JsonSerializer.Deserialize<PatchOperation[]>(await sr.ReadToEndAsync()) ?? Array.Empty<PatchOperation>();
            
            var document = new JsonPatchDocument<TEntity>();

            foreach(var operation in parsedDocument)
            {
                switch (operation.op)
                {
                    case Common.Constants.Operations.REPLACE:
                        document._operations.Add(new JsonPatchOperation(JsonPatchOperationType.Replace, operation.path, Value: operation.value));
                        break;

                    case Common.Constants.Operations.MOVE:
                        document._operations.Add(new JsonPatchOperation(JsonPatchOperationType.Move, operation.path, Value: operation.value, FromPath: operation.from));
                        break;
                    case Common.Constants.Operations.ADD:
                        document._operations.Add(new JsonPatchOperation(JsonPatchOperationType.Add, operation.path, Value: operation.value));
                        break;
                    case Common.Constants.Operations.REMOVE:
                        document._operations.Add(new JsonPatchOperation(JsonPatchOperationType.Remove, operation.path));
                        break;
                }
            }

            return document;
        }
    }
}
