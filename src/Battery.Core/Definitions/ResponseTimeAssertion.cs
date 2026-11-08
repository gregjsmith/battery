namespace Battery.Core.Definitions;

/// <summary>
/// An assertion on the response time (FR-4.4).
/// </summary>
public sealed record ResponseTimeAssertion : Assertion
{
    /// <summary>The response time must stay under this threshold.</summary>
    public required TimeSpan Under { get; init; }
}
