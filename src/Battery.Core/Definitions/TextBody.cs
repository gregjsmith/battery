namespace Battery.Core.Definitions;

/// <summary>
/// A raw text body (FR-1.4).
/// </summary>
public sealed record TextBody : RequestBody
{
    /// <summary>The raw body text.</summary>
    public required string Text { get; init; }
}
