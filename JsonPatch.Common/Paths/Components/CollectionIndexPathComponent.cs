namespace JsonPatch.Paths.Components
{
    public class CollectionIndexPathComponent : PathComponent
    {
        public CollectionIndexPathComponent(string name)
            : base(name)
        {
        }

        public int CollectionIndex { get; set; }
    }
}
