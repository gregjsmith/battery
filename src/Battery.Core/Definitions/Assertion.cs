namespace Battery.Core.Definitions;

/// <summary>
/// An assertion on a response. Each kind is a sealed record (section 5.4).
/// A test can hold more than one assertion (FR-4.5).
/// </summary>
public abstract record Assertion;
