namespace JsonPatch
{
    public interface IJsonPatchDocument
    {
        void Add(string path, object value);
        void Replace(string path, object value);
        void Remove(string path);
        void Move(string from, string path);
    }
}
