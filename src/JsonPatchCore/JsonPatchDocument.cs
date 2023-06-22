using JsonPatch.Paths.Resolvers;
using JsonPatch.Infrastructure;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using JsonPatch.Model;

namespace JsonPatch;

/// <summary>
/// Represent a JsonPatchDocument
/// </summary>
public class JsonPatchDocument
{
    /// <summary>
    /// List of operations to apply
    /// </summary>
    protected readonly List<JsonPatchOperation> Operations = new();

    /// <summary>
    /// Add the "Add" operation to the list
    /// </summary>
    /// <param name="path"></param>
    /// <param name="value"></param>
    public void Add(string path, object value) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.add, path, value));

    /// <summary>
    /// Add the "Replace" operation to the list
    /// </summary>
    /// <param name="path"></param>
    /// <param name="value"></param>
    public void Replace(string path, object value) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.replace, path, value));

    /// <summary>
    /// Add the "Remove" operation to the list
    /// </summary>
    /// <param name="path"></param>
    public void Remove(string path) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.remove, path));

    /// <summary>
    /// Add the "Move" operation to the list
    /// </summary>
    /// <param name="from"></param>
    /// <param name="path"></param>
    public void Move(string from, string path) => Operations.Add(new JsonPatchOperation(JsonPatchOperationType.move, path, FromPath: @from));
}

/// <summary>
/// A typed JsonPatchDocument
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class JsonPatchDocument<TEntity> : JsonPatchDocument, IJsonPatchDocument<TEntity> where TEntity : class
{
    private readonly IPathResolver _resolver = JsonPatchSettings.Options.PathResolver;

    /// <summary>
    /// Apply the operations to the entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
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

    /// <summary>
    /// Bind to the HTTP Request
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static ValueTask<JsonPatchDocument<TEntity>?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        return BindAsync(httpContext);
    }

    /// <summary>
    /// Bind to the HTTP Request
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    /// <exception cref="JsonPatchParseException"></exception>
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
