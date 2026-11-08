using System.Collections.ObjectModel;

namespace Battery.Core.Definitions;

/// <summary>
/// An environment profile, for example <c>dev</c>, <c>staging</c>, or <c>prod</c>
/// (FR-5.2). It maps variable names to values (section 9.1).
/// </summary>
public sealed record EnvironmentProfile
{
    /// <summary>The profile name.</summary>
    public required string Name { get; init; }

    /// <summary>The variables, as a map of names to values.</summary>
    public IReadOnlyDictionary<string, string> Variables { get; init; }
        = ReadOnlyDictionary<string, string>.Empty;
}
