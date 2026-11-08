namespace Battery.Core.Definitions;

/// <summary>
/// A suite. It is an ordered collection of tests, with optional shared config and
/// setup/teardown steps (FR-3.1, FR-3.4, FR-3.5).
/// </summary>
public sealed record Suite
{
    /// <summary>The suite name.</summary>
    public required string Name { get; init; }

    /// <summary>The shared defaults, or <see langword="null"/> for none.</summary>
    public BaseConfig? BaseConfig { get; init; }

    /// <summary>The tests, in run order (FR-8.1).</summary>
    public IReadOnlyList<Test> Tests { get; init; } = [];

    /// <summary>The tags, for selective execution (FR-3.3).</summary>
    public IReadOnlyList<string> Tags { get; init; } = [];

    /// <summary>The step that runs before the tests, or <see langword="null"/> for none.</summary>
    public Step? Setup { get; init; }

    /// <summary>The step that runs after the tests, or <see langword="null"/> for none.</summary>
    public Step? Teardown { get; init; }
}
