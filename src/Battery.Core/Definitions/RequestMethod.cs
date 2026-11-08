namespace Battery.Core.Definitions;

/// <summary>
/// The HTTP method of a request (FR-1.1).
/// </summary>
public enum RequestMethod
{
    /// <summary>The HTTP GET method.</summary>
    Get,

    /// <summary>The HTTP POST method.</summary>
    Post,

    /// <summary>The HTTP PUT method.</summary>
    Put,

    /// <summary>The HTTP PATCH method.</summary>
    Patch,

    /// <summary>The HTTP DELETE method.</summary>
    Delete,

    /// <summary>The HTTP HEAD method.</summary>
    Head,

    /// <summary>The HTTP OPTIONS method.</summary>
    Options,
}
