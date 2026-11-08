namespace Battery.Core.Definitions;

/// <summary>
/// A JSON body read from a referenced <c>.json</c> data file (FR-2.3).
/// </summary>
public sealed record JsonFileBody : RequestBody
{
    /// <summary>The path to the JSON data file.</summary>
    public required string Path { get; init; }
}
