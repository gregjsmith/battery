namespace Battery.Core.Definitions;

/// <summary>
/// An extraction that reads a response header.
/// </summary>
public sealed record HeaderExtractionSource : ExtractionSource
{
    /// <summary>The header name to read.</summary>
    public required string HeaderName { get; init; }
}
