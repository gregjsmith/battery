using System.Text.Json.Nodes;

namespace Battery.Core.Definitions;

/// <summary>
/// A JSON body written inline in the suite file (FR-1.3).
/// </summary>
public sealed record JsonInlineBody : RequestBody
{
    /// <summary>The JSON content. Its string leaves can hold <c>${var}</c> templates.</summary>
    public required JsonNode Json { get; init; }
}
