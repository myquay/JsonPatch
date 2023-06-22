using JsonPatch.Paths.Resolvers;
using JsonPatch.Infrastructure;

namespace JsonPatch;

/// <summary>
/// Collection of settings related to JsonPath
/// </summary>
public record JsonPatchOptions
{
    public IPathResolver PathResolver { get; set; } = new CaseInsensitivePropertyPathResolver(new JsonValueConverter());

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

