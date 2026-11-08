namespace Battery.Core.Definitions;

/// <summary>
/// TLS options for a request under test (FR-1.6).
/// </summary>
public sealed record TlsOptions
{
    /// <summary>
    /// When <see langword="true"/>, the request accepts a self-signed server
    /// certificate. The default is <see langword="false"/>.
    /// </summary>
    public bool AllowSelfSigned { get; init; }
}
