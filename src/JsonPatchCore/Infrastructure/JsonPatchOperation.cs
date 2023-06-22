namespace JsonPatch.Infrastructure;

/// <summary>
/// An individual JsonPatch Operation
/// </summary>
/// <param name="Type"></param>
/// <param name="Path"></param>
/// <param name="Value"></param>
/// <param name="FromPath"></param>
public record JsonPatchOperation(JsonPatchOperationType Type, string Path, object? Value = null, string? FromPath = null);
