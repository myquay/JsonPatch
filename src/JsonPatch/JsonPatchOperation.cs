using JsonPatch.Paths.Components;

namespace JsonPatch
{
    public class JsonPatchOperation
    {
        public JsonPatchOperationType Operation { get; set; }
        public string FromPath { get; set; }
        public PathComponent[] ParsedFromPath { get; set; }
        public string Path { get; set; }
        public PathComponent[] ParsedPath { get; set; }
        public object Value { get; set; }
    }
}
