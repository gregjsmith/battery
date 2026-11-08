namespace Battery.Core.Definitions;

/// <summary>
/// An extraction that reads a response body value, selected by JSONPath.
/// </summary>
public sealed record BodyExtractionSource : ExtractionSource
{
    /// <summary>The JSONPath that selects the value.</summary>
    public required string JsonPath { get; init; }
}
