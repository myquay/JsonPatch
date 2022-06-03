namespace JsonPatchCore.Infrastructure;

public record JsonPatchOperation
{
    public JsonPatchOperation(JsonPatchOperationType Type, string Path, object? Value = null, string? FromPath = null) {
        this.Type = Type;
        this.FromPath = FromPath;
        this.Value = Value;
        this.Path = Path;
    }

    public JsonPatchOperationType Type { get; set; }

    public string Path { get; set; }

    public object? Value { get; set; }

    public string? FromPath { get; set; }

}

public enum JsonPatchOperationType
{
    Add,
    Remove,
    Replace,
    Move
}