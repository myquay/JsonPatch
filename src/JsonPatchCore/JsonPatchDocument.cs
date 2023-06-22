using JsonPatch.Paths.Resolvers;
using JsonPatch.Infrastructure;
using System.Reflection;
using System.Text.Json;
using JsonPatch.Model;
using Microsoft.AspNetCore.Http;

namespace JsonPatch;

public class JsonPatchDocument
{
    protected readonly List<JsonPatchOperation> Operations = new();

    public void Add(string path, object value) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.add, path, value));

    public void Replace(string path, object value) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.replace, path, value));

    public void Remove(string path) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.remove, path));

    public void Move(string from, string path) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.move, path, FromPath: @from));
}

public class JsonPatchDocument<TEntity> : JsonPatchDocument, IJsonPatchDocument<TEntity> where TEntity : class
{
    private readonly IPathResolver _resolver = JsonPatchSettings.Options.PathResolver;

    public TEntity ApplyTo(TEntity entity)
    {
        foreach (var operation in Operations)
        {
            var parsedPath = _resolver.ParsePath(operation.Path, typeof(TEntity));
            switch (operation.Type)
            {
                case JsonPatchOperationType.remove:
                    _resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, null, JsonPatchOperationType.remove);
                    break;
                case JsonPatchOperationType.replace:
                    _resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, operation.Value, JsonPatchOperationType.replace);
                    break;
                case JsonPatchOperationType.add:
                    _resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, operation.Value, JsonPatchOperationType.add);
                    break;
                case JsonPatchOperationType.move:
                    var parsedFromPath = _resolver.ParsePath(operation.FromPath, typeof(TEntity));
                    var value = _resolver.GetValueFromPath(typeof(TEntity), parsedFromPath, entity);
                    _resolver.SetValueFromPath(typeof(TEntity), parsedFromPath, entity, null, JsonPatchOperationType.remove);
                    _resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, value, JsonPatch.JsonPatchOperationType.add);
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
            throw new JsonPatchParseException("The request content type must be 'application/json-patch+json'");

        using (var sr = new StreamReader(httpContext.Request.Body))
        {
            var parsedDocument = JsonSerializer.Deserialize<PatchOperation[]>(await sr.ReadToEndAsync()) ?? Array.Empty<PatchOperation>();
            
            var document = new JsonPatchDocument<TEntity>();

            foreach(var operation in parsedDocument)
            {
                switch (operation.op)
                {
                    case Constants.Operations.REPLACE:
                        document.Operations.Add(new JsonPatchOperation(JsonPatchOperationType.replace, operation.path, Value: operation.value));
                        break;

                    case Constants.Operations.MOVE:
                        document.Operations.Add(new JsonPatchOperation(JsonPatchOperationType.move, operation.path, Value: operation.value, FromPath: operation.from));
                        break;
                    case Constants.Operations.ADD:
                        document.Operations.Add(new JsonPatchOperation(JsonPatchOperationType.add, operation.path, Value: operation.value));
                        break;
                    case Constants.Operations.REMOVE:
                        document.Operations.Add(new JsonPatchOperation(JsonPatchOperationType.remove, operation.path));
                        break;
                }
            }

            return document;
        }
    }
}
