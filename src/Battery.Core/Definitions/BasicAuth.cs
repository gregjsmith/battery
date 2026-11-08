namespace Battery.Core.Definitions;

/// <summary>
/// HTTP Basic authentication (FR-7.2).
/// </summary>
public sealed record BasicAuth : Auth
{
    /// <summary>The user name. It can be a <c>${var}</c> template.</summary>
    public required string Username { get; init; }

    /// <summary>The password. It can be a <c>${var}</c> template.</summary>
    public required string Password { get; init; }
}
