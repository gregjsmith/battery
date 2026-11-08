namespace Battery.Core.Definitions;

/// <summary>
/// API key authentication, in a header or a query parameter (FR-7.3).
/// </summary>
public sealed record ApiKeyAuth : Auth
{
    /// <summary>The name of the header or query parameter that carries the key.</summary>
    public required string Name { get; init; }

    /// <summary>Where the key travels on the request.</summary>
    public required ApiKeyLocation Location { get; init; }

    /// <summary>The key value. It can be a <c>${var}</c> template.</summary>
    public required string Value { get; init; }
}
