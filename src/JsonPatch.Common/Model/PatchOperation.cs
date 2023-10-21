namespace JsonPatch.Model
{
    /// <summary>
    /// JSON Patch Operation
    /// </summary>
    public class PatchOperation
    {

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// Operation
        /// </summary>
        public string op { get; set; }

        /// <summary>
        /// From (if applicable)
        /// </summary>
        public string from { get; set; }

        /// <summary>
        /// Where the operation should be applied
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// Value for the operation (if applicable)
        /// </summary>
        public object value { get; set; }

#pragma warning restore IDE1006 // Naming Styles

    }
}
