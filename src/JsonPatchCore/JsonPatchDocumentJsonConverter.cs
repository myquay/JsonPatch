using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonPatchCore;

public class JsonPatchDocumentJsonConverter : JsonConverter<JsonPatchDocument>
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(JsonPatchDocument));

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
                case JsonPatch.Common.Constants.Operations.ADD:
                    result.Add(operation.path, operation.value!);
                    break;
                case JsonPatch.Common.Constants.Operations.REPLACE:
                    result.Replace(operation.path, operation.value!);
                    break;
                case JsonPatch.Common.Constants.Operations.REMOVE:
                    result.Remove(operation.path);
                    break;
                case JsonPatch.Common.Constants.Operations.MOVE:
                    result.Move(operation.from!, operation.path);
                    break;
                default:
                    throw new NotSupportedException($"{operation.op} not supported");
            }
        }

        return result;
    }

    public class PatchOperation
    {
        // ReSharper disable InconsistentNaming
        public string op { get; set; } = null!;
        public string? from { get; set; }
        public string path { get; set; } = null!;
        public object? value { get; set; }
        // ReSharper restore InconsistentNaming
    }

    public override void Write(Utf8JsonWriter writer, JsonPatchDocument value, JsonSerializerOptions options) =>
        throw new NotImplementedException();
}