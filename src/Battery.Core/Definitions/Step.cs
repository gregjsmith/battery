namespace Battery.Core.Definitions;

/// <summary>
/// A setup or teardown step. It holds a request, optional extractions, and
/// optional assertions. It uses the same shape as a test (section 9.1). A setup
/// step often logs in and extracts a token (FR-3.5).
/// </summary>
public sealed record Step
{
    /// <summary>The request to send.</summary>
    public required Request Request { get; init; }

    /// <summary>The values to extract from the response.</summary>
    public IReadOnlyList<Extraction> Extractions { get; init; } = [];

    /// <summary>The assertions to check on the response.</summary>
    public IReadOnlyList<Assertion> Assertions { get; init; } = [];
}
