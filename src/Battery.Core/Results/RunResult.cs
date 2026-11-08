namespace Battery.Core.Results;

/// <summary>
/// The result of a whole run (section 9.2). The totals are computed from
/// <see cref="SuiteResults"/>, so they cannot drift. The CLI maps a failed run to
/// a non-zero exit code (FR-9.2).
/// </summary>
public sealed record RunResult
{
    /// <summary>The result of each suite, in order.</summary>
    public IReadOnlyList<SuiteResult> SuiteResults { get; init; } = [];

    /// <summary>How long the run took.</summary>
    public required TimeSpan Duration { get; init; }

    /// <summary>The total count of tests across every suite.</summary>
    public int TotalTests => SuiteResults.Sum(s => s.Total);

    /// <summary>The count of tests that passed.</summary>
    public int PassedCount => SuiteResults.Sum(s => s.PassedCount);

    /// <summary>The count of tests that failed.</summary>
    public int FailedCount => SuiteResults.Sum(s => s.FailedCount);

    /// <summary>The count of tests that errored.</summary>
    public int ErroredCount => SuiteResults.Sum(s => s.ErroredCount);

    /// <summary>
    /// Whether the whole run passed. It is <see langword="true"/> only when every
    /// suite passed.
    /// </summary>
    public bool Passed => SuiteResults.All(s => s.Passed);
}
