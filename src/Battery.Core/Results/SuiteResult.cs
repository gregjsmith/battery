namespace Battery.Core.Results;

/// <summary>
/// The result of one suite (section 9.2). The counts are computed from
/// <see cref="TestResults"/>, so they cannot drift.
/// </summary>
public sealed record SuiteResult
{
    /// <summary>The suite name.</summary>
    public required string SuiteName { get; init; }

    /// <summary>The result of each test, in order.</summary>
    public IReadOnlyList<TestResult> TestResults { get; init; } = [];

    /// <summary>How long the suite took.</summary>
    public required TimeSpan Duration { get; init; }

    /// <summary>The count of tests that passed.</summary>
    public int PassedCount => TestResults.Count(r => r.Outcome == TestOutcome.Passed);

    /// <summary>The count of tests that failed.</summary>
    public int FailedCount => TestResults.Count(r => r.Outcome == TestOutcome.Failed);

    /// <summary>The count of tests that errored.</summary>
    public int ErroredCount => TestResults.Count(r => r.Outcome == TestOutcome.Errored);

    /// <summary>The total count of tests.</summary>
    public int Total => TestResults.Count;

    /// <summary>
    /// Whether the suite passed. It is <see langword="true"/> only when no test
    /// failed or errored.
    /// </summary>
    public bool Passed => TestResults.All(r => r.Outcome == TestOutcome.Passed);
}
