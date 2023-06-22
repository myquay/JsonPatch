namespace JsonPatch.Paths.Components
{
    /// <summary>
    /// Dictionary path component.
    /// </summary>
    public class DictionaryPathComponent : PathComponent
    {
        /// <summary>
        /// Name of the component.
        /// </summary>
        /// <param name="name"></param>
        public DictionaryPathComponent(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Key of the dictionary.
        /// </summary>
        public object Key { get; set; }
    }
}
