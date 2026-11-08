using System.Collections.ObjectModel;

namespace Battery.Core.Definitions;

/// <summary>
/// An authored request template. It can hold <c>${var}</c> templates. The engine
/// resolves it to a resolved request before it sends the request (section 9.4).
/// </summary>
public sealed record Request
{
    /// <summary>The HTTP method (FR-1.1).</summary>
    public required RequestMethod Method { get; init; }

    /// <summary>A path joined to the base URL, or an absolute URL (FR-1.2).</summary>
    public required string Url { get; init; }

    /// <summary>The query parameters, as a map of names to values (FR-1.2).</summary>
    public IReadOnlyDictionary<string, string> Query { get; init; }
        = ReadOnlyDictionary<string, string>.Empty;

    /// <summary>The request headers, as a map of names to values (FR-1.2).</summary>
    public IReadOnlyDictionary<string, string> Headers { get; init; }
        = ReadOnlyDictionary<string, string>.Empty;

    /// <summary>The request body, or <see langword="null"/> for no body.</summary>
    public RequestBody? Body { get; init; }

    /// <summary>
    /// The per-request timeout, or <see langword="null"/> to inherit the default
    /// (FR-1.5).
    /// </summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>
    /// Whether the request follows redirects, or <see langword="null"/> to inherit
    /// the default (FR-1.5).
    /// </summary>
    public bool? FollowRedirects { get; init; }

    /// <summary>
    /// The TLS options, or <see langword="null"/> to inherit the default (FR-1.6).
    /// </summary>
    public TlsOptions? Tls { get; init; }

    /// <summary>
    /// The request authentication, or <see langword="null"/> to inherit the default
    /// (section 5.7).
    /// </summary>
    public Auth? Auth { get; init; }
}
