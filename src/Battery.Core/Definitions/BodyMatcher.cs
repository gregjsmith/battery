namespace Battery.Core.Definitions;

/// <summary>
/// How a body assertion checks the value that its JSONPath selects (section 9.5).
/// </summary>
public enum BodyMatcher
{
    /// <summary>The value equals the expected value exactly.</summary>
    Equals,

    /// <summary>The field is present. The value does not matter.</summary>
    Exists,

    /// <summary>The value holds the expected text or item.</summary>
    Contains,

    /// <summary>The value fits a regular expression.</summary>
    Matches,
}
