namespace Battery.Core.Definitions;

/// <summary>
/// Where an API key travels on a request (FR-7.3).
/// </summary>
public enum ApiKeyLocation
{
    /// <summary>The key travels in a request header.</summary>
    Header,

    /// <summary>The key travels in a query parameter.</summary>
    Query,
}
