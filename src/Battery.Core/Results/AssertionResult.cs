using Battery.Core.Definitions;

namespace Battery.Core.Results;

/// <summary>
/// The result of one assertion (section 9.2).
/// </summary>
public sealed record AssertionResult
{
    /// <summary>The assertion that produced this result.</summary>
    public required Assertion Assertion { get; init; }

    /// <summary>Whether the assertion passed.</summary>
    public required bool Passed { get; init; }

    /// <summary>The expected value, as text, or <see langword="null"/> when none applies.</summary>
    public string? Expected { get; init; }

    /// <summary>The actual value, as text, or <see langword="null"/> when none applies.</summary>
    public string? Actual { get; init; }

    /// <summary>A message that explains the result, or <see langword="null"/> for none.</summary>
    public string? Message { get; init; }
}
