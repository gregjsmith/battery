namespace Battery.Core.Definitions;

/// <summary>
/// An extraction. It reads a value from a response and sets a variable (FR-6.1).
/// A later request in the suite can use the variable (FR-6.2).
/// </summary>
public sealed record Extraction
{
    /// <summary>The name of the variable to set.</summary>
    public required string Name { get; init; }

    /// <summary>Where to read the value from.</summary>
    public required ExtractionSource Source { get; init; }
}
