namespace Battery.Core.Definitions;

/// <summary>
/// An assertion on the HTTP status code (FR-4.1). It sets one of
/// <see cref="ExpectedStatus"/> or <see cref="AllowedStatuses"/>.
/// </summary>
public sealed record StatusAssertion : Assertion
{
    /// <summary>
    /// The single expected status code, or <see langword="null"/> when a set of
    /// allowed codes is used.
    /// </summary>
    public int? ExpectedStatus { get; init; }

    /// <summary>
    /// The set of allowed status codes, or <see langword="null"/> when a single
    /// code is used.
    /// </summary>
    public IReadOnlyList<int>? AllowedStatuses { get; init; }
}
