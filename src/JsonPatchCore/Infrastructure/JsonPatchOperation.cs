namespace JsonPatchCore.Infrastructure;

public record JsonPatchOperation(JsonPatchOperationType Type, string Path, object? Value = null, string? FromPath = null);

public enum JsonPatchOperationType
{
    Add,
    Remove,
    Replace,
    Move
}