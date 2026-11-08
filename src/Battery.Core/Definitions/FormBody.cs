using System.Collections.ObjectModel;

namespace Battery.Core.Definitions;

/// <summary>
/// A body sent as <c>application/x-www-form-urlencoded</c> (FR-1.4).
/// </summary>
public sealed record FormBody : RequestBody
{
    /// <summary>The form fields, as a map of names to values.</summary>
    public IReadOnlyDictionary<string, string> Fields { get; init; }
        = ReadOnlyDictionary<string, string>.Empty;
}
