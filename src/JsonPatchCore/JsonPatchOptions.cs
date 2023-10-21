using JsonPatch.Paths.Resolvers;

namespace JsonPatch;

/// <summary>
/// Collection of settings related to JsonPath
/// </summary>
public record JsonPatchOptions
{
    /// <summary>
    /// The Path Resolver to use when parsing JsonPath
    /// </summary>
    public IPathResolver PathResolver { get; set; } = new CaseInsensitivePropertyPathResolver(new JsonValueConverter());

    /// <summary>
    /// Weather or not to require the Content-Type header to be application/json-patch+json
    /// </summary>
    public bool RequireJsonPatchContentType { get; set; } = true;
}

/// <summary>
/// Default settings
/// </summary>
public class JsonPatchSettings
{
    /// <summary>
    /// Default options
    /// </summary>
    public static JsonPatchOptions Options { get; set; } = new JsonPatchOptions();
}
