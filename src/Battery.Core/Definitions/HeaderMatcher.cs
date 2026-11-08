namespace Battery.Core.Definitions;

/// <summary>
/// How a header assertion checks a response header (section 5.4).
/// </summary>
public enum HeaderMatcher
{
    /// <summary>The header value equals the expected value exactly.</summary>
    Equals,

    /// <summary>The header is present. The value does not matter.</summary>
    Exists,

    /// <summary>The header value fits a regular expression.</summary>
    Matches,
}
