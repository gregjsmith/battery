namespace Battery.Core.Definitions;

/// <summary>
/// The body of a request. Each kind is a sealed record (FR-1.3, FR-1.4).
/// Multipart is deferred to Phase 2 (FR-1.7).
/// </summary>
public abstract record RequestBody;
