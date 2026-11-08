namespace Battery.Core.Results;

/// <summary>
/// The outcome of a single test (section 9.2).
/// </summary>
public enum TestOutcome
{
    /// <summary>Every assertion passed.</summary>
    Passed,

    /// <summary>The request completed, but at least one assertion did not pass.</summary>
    Failed,

    /// <summary>The request did not complete. For example a transport fault.</summary>
    Errored,
}
