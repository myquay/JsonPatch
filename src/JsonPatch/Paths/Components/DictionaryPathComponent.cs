namespace JsonPatch.Paths.Components
{
    public class DictionaryPathComponent : PathComponent
    {
        public DictionaryPathComponent(string name)
            : base(name)
        {
        }

        public object Key { get; set; }
    }
}
