using Common = JsonPatch.Common;
using JsonPatch.Common.Paths.Resolvers;
using JsonPatchCore.Infrastructure;
using System.Reflection;
using System.Text.Json;
using JsonPatch.Common.Model;
using Microsoft.AspNetCore.Http;

namespace JsonPatchCore;

public class JsonPatchDocument
{
    protected readonly List<JsonPatchOperation> Operations = new();

    public void Add(string path, object value) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.Add, path, value));

    public void Replace(string path, object value) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.Replace, path, value));

    public void Remove(string path) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.Remove, path));

    public void Move(string from, string path) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.Move, path, FromPath: @from));
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
                case JsonPatchOperationType.Remove:
                    _resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, null, Common.JsonPatchOperationType.remove);
                    break;
                case JsonPatchOperationType.Replace:
                    _resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, operation.Value, Common.JsonPatchOperationType.replace);
                    break;
                case JsonPatchOperationType.Add:
                    _resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, operation.Value, Common.JsonPatchOperationType.add);
                    break;
                case JsonPatchOperationType.Move:
                    var parsedFromPath = _resolver.ParsePath(operation.FromPath, typeof(TEntity));
                    var value = _resolver.GetValueFromPath(typeof(TEntity), parsedFromPath, entity);
                    _resolver.SetValueFromPath(typeof(TEntity), parsedFromPath, entity, null, Common.JsonPatchOperationType.remove);
                    _resolver.SetValueFromPath(typeof(TEntity), parsedPath, entity, value, Common.JsonPatchOperationType.add);
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
                        document.Operations.Add(new JsonPatchOperation(JsonPatchOperationType.Replace, operation.path, Value: operation.value));
                        break;

                    case Common.Constants.Operations.MOVE:
                        document.Operations.Add(new JsonPatchOperation(JsonPatchOperationType.Move, operation.path, Value: operation.value, FromPath: operation.from));
                        break;
                    case Common.Constants.Operations.ADD:
                        document.Operations.Add(new JsonPatchOperation(JsonPatchOperationType.Add, operation.path, Value: operation.value));
                        break;
                    case Common.Constants.Operations.REMOVE:
                        document.Operations.Add(new JsonPatchOperation(JsonPatchOperationType.Remove, operation.path));
                        break;
                }
            }

            return document;
        }
    }
}
