namespace Battery.Core.Definitions;

/// <summary>
/// Where an extraction reads its value from. Each kind is a sealed record
/// (FR-6.1).
/// </summary>
public abstract record ExtractionSource;
