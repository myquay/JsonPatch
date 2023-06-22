namespace JsonPatch.Infrastructure;

public record JsonPatchOperation(JsonPatchOperationType Type, string Path, object? Value = null, string? FromPath = null);
