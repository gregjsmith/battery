namespace Battery.Core.Definitions;

/// <summary>
/// Bearer / JWT token authentication (FR-7.1).
/// </summary>
public sealed record BearerAuth : Auth
{
    /// <summary>The bearer token. It can be a <c>${var}</c> template.</summary>
    public required string Token { get; init; }
}
