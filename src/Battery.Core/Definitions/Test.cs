namespace Battery.Core.Definitions;

/// <summary>
/// A test. It is a request plus its assertions and optional extractions (FR-2.2).
/// </summary>
public sealed record Test
{
    /// <summary>The test name.</summary>
    public required string Name { get; init; }

    /// <summary>The tags, for selective execution (FR-3.3).</summary>
    public IReadOnlyList<string> Tags { get; init; } = [];

    /// <summary>The request to send.</summary>
    public required Request Request { get; init; }

    /// <summary>The assertions to check on the response (FR-4.5).</summary>
    public IReadOnlyList<Assertion> Assertions { get; init; } = [];

    /// <summary>The values to extract from the response (FR-6.1).</summary>
    public IReadOnlyList<Extraction> Extractions { get; init; } = [];
}
