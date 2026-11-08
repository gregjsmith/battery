using System.Collections.ObjectModel;

namespace Battery.Core.Definitions;

/// <summary>
/// Suite-level shared defaults. A test inherits these and can override them
/// (FR-3.4).
/// </summary>
public sealed record BaseConfig
{
    /// <summary>The base URL, joined to each request path.</summary>
    public string? BaseUrl { get; init; }

    /// <summary>The common headers, as a map of names to values.</summary>
    public IReadOnlyDictionary<string, string> Headers { get; init; }
        = ReadOnlyDictionary<string, string>.Empty;

    /// <summary>The default per-request timeout.</summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>The default request authentication.</summary>
    public Auth? Auth { get; init; }
}
