namespace Battery.Core.Definitions;

/// <summary>
/// An assertion on a response body value, selected by JSONPath (FR-4.3).
/// </summary>
public sealed record BodyAssertion : Assertion
{
    /// <summary>The JSONPath that selects the value to check.</summary>
    public required string JsonPath { get; init; }

    /// <summary>How to check the selected value (section 9.5).</summary>
    public required BodyMatcher Matcher { get; init; }

    /// <summary>
    /// The expected value, as text. It is not needed when <see cref="Matcher"/> is
    /// <see cref="BodyMatcher.Exists"/>.
    /// </summary>
    public string? Value { get; init; }
}
