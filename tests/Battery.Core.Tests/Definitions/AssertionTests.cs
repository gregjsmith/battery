using Battery.Core.Definitions;
using Xunit;

namespace Battery.Core.Tests.Definitions;

public class AssertionTests
{
    [Fact]
    public void Each_assertion_kind_is_a_distinct_type()
    {
        Assertion status = new StatusAssertion { ExpectedStatus = 200 };
        Assertion header = new HeaderAssertion { Name = "ETag", Matcher = HeaderMatcher.Exists };
        Assertion body = new BodyAssertion { JsonPath = "$.id", Matcher = BodyMatcher.Exists };
        Assertion time = new ResponseTimeAssertion { Under = TimeSpan.FromMilliseconds(500) };

        Assert.Equal("status", Describe(status));
        Assert.Equal("header", Describe(header));
        Assert.Equal("body", Describe(body));
        Assert.Equal("time", Describe(time));
    }

    [Fact]
    public void Status_assertion_holds_a_single_code_or_a_set()
    {
        var single = new StatusAssertion { ExpectedStatus = 201 };
        var set = new StatusAssertion { AllowedStatuses = [200, 201, 204] };

        Assert.Equal(201, single.ExpectedStatus);
        Assert.Null(single.AllowedStatuses);
        Assert.Null(set.ExpectedStatus);
        Assert.Equal(3, set.AllowedStatuses!.Count);
    }

    [Fact]
    public void Equal_body_assertions_are_equal()
    {
        var a = new BodyAssertion { JsonPath = "$.id", Matcher = BodyMatcher.Equals, Value = "7" };
        var b = new BodyAssertion { JsonPath = "$.id", Matcher = BodyMatcher.Equals, Value = "7" };

        Assert.Equal(a, b);
    }

    private static string Describe(Assertion assertion) => assertion switch
    {
        StatusAssertion => "status",
        HeaderAssertion => "header",
        BodyAssertion => "body",
        ResponseTimeAssertion => "time",
        _ => "unknown",
    };
}
