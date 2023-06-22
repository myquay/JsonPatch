namespace JsonPatch.Paths.Components
{
    /// <summary>
    /// Path component representing a collection index.
    /// </summary>
    public class CollectionIndexPathComponent : PathComponent
    {
        /// <summary>
        /// Name of the component.
        /// </summary>
        /// <param name="name"></param>
        public CollectionIndexPathComponent(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Index of the collection.
        /// </summary>
        public int CollectionIndex { get; set; }
    }
}
