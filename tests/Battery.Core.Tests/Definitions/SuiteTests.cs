using Battery.Core.Definitions;
using Xunit;

namespace Battery.Core.Tests.Definitions;

public class SuiteTests
{
    [Fact]
    public void Collections_default_to_empty_and_steps_default_to_null()
    {
        var suite = new Suite { Name = "albums" };

        Assert.Empty(suite.Tests);
        Assert.Empty(suite.Tags);
        Assert.Null(suite.BaseConfig);
        Assert.Null(suite.Setup);
        Assert.Null(suite.Teardown);
    }

    [Fact]
    public void A_test_carries_its_request_assertions_and_extractions()
    {
        var test = new Test
        {
            Name = "get album",
            Request = new Request { Method = RequestMethod.Get, Url = "/albums/1" },
            Assertions = [new StatusAssertion { ExpectedStatus = 200 }],
            Extractions = [new Extraction { Name = "id", Source = new StatusExtractionSource() }],
        };

        Assert.Single(test.Assertions);
        Assert.Single(test.Extractions);
        Assert.Empty(test.Tags);
    }

    [Fact]
    public void A_step_shares_the_test_shape()
    {
        var step = new Step
        {
            Request = new Request { Method = RequestMethod.Post, Url = "/login" },
            Extractions = [new Extraction { Name = "token", Source = new BodyExtractionSource { JsonPath = "$.token" } }],
        };

        Assert.Single(step.Extractions);
        Assert.Empty(step.Assertions);
    }

    [Fact]
    public void An_environment_profile_maps_variables()
    {
        var profile = new EnvironmentProfile
        {
            Name = "staging",
            Variables = new Dictionary<string, string> { ["baseUrl"] = "https://staging.example.com" },
        };

        Assert.Equal("https://staging.example.com", profile.Variables["baseUrl"]);
    }
}
