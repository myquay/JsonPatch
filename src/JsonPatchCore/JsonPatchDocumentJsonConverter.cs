using JsonPatch.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonPatch;

/// <summary>
/// Converts a JsonPatchDocument to and from a Json string
/// </summary>
public class JsonPatchDocumentJsonConverter : JsonConverter<JsonPatchDocument>
{
    /// <summary>
    /// Can this converter convert the specified type
    /// </summary>
    /// <param name="typeToConvert"></param>
    /// <returns></returns>
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(JsonPatchDocument));

    /// <summary>
    /// Read a JsonPatchDocument from the reader
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public override JsonPatchDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var document)) return null;

        if (document.RootElement.ValueKind != JsonValueKind.Array) throw new JsonException("array expected");

        var result = Activator.CreateInstance(typeToConvert) as JsonPatchDocument;
        if (result == null) throw new InvalidOperationException($"Unable to create instance of {typeToConvert.FullName}");

        var operations = document.Deserialize<PatchOperation[]>(options) ?? Array.Empty<PatchOperation>();
        foreach (var operation in operations)
        {
            switch (operation.op)
            {
                case JsonPatch.Constants.Operations.ADD:
                    result.Add(operation.path, operation.value!);
                    break;
                case JsonPatch.Constants.Operations.REPLACE:
                    result.Replace(operation.path, operation.value!);
                    break;
                case JsonPatch.Constants.Operations.REMOVE:
                    result.Remove(operation.path);
                    break;
                case JsonPatch.Constants.Operations.MOVE:
                    result.Move(operation.from!, operation.path);
                    break;
                default:
                    throw new NotSupportedException($"{operation.op} not supported");
            }
        }

        return result;
    }

    /// <summary>
    /// Serailise the JsonPatchDocument to a Json string
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void Write(Utf8JsonWriter writer, JsonPatchDocument value, JsonSerializerOptions options) =>
        throw new NotImplementedException();
}