namespace Battery.Core.Definitions;

/// <summary>
/// An assertion on a response header (FR-4.2).
/// </summary>
public sealed record HeaderAssertion : Assertion
{
    /// <summary>The header name.</summary>
    public required string Name { get; init; }

    /// <summary>How to check the header.</summary>
    public required HeaderMatcher Matcher { get; init; }

    /// <summary>
    /// The expected value. It is not needed when <see cref="Matcher"/> is
    /// <see cref="HeaderMatcher.Exists"/>.
    /// </summary>
    public string? Value { get; init; }
}
