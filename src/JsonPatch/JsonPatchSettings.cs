using JsonPatch.Common.Paths.Resolvers;

namespace JsonPatch
{
    /// <summary>
    /// Collection of settings related to JsonPath
    /// </summary>
    public class JsonPatchSettings
    {
        public IPathResolver PathResolver { get; set; }

        internal static JsonPatchSettings DefaultPatchSettings()
        {
            return new JsonPatchSettings
            {
                PathResolver = new ExactCasePropertyPathResolver()
            };
        }
    }
}
