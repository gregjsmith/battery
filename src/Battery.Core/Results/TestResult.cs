namespace Battery.Core.Results;

/// <summary>
/// The result of one test (section 9.2).
/// </summary>
public sealed record TestResult
{
    /// <summary>The test name.</summary>
    public required string TestName { get; init; }

    /// <summary>The outcome: passed, failed, or errored.</summary>
    public required TestOutcome Outcome { get; init; }

    /// <summary>The result of each assertion, in order.</summary>
    public IReadOnlyList<AssertionResult> AssertionResults { get; init; } = [];

    /// <summary>How long the test took.</summary>
    public required TimeSpan Duration { get; init; }

    /// <summary>
    /// The error message when the outcome is <see cref="TestOutcome.Errored"/>, or
    /// <see langword="null"/> otherwise.
    /// </summary>
    public string? Error { get; init; }
}
