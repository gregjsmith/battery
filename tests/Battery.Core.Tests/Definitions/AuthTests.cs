using Battery.Core.Definitions;
using Xunit;

namespace Battery.Core.Tests.Definitions;

public class AuthTests
{
    [Fact]
    public void Each_auth_kind_is_a_distinct_type()
    {
        Auth bearer = new BearerAuth { Token = "${token}" };
        Auth basic = new BasicAuth { Username = "tester", Password = "${pw}" };
        Auth apiKey = new ApiKeyAuth
        {
            Name = "X-Api-Key",
            Location = ApiKeyLocation.Header,
            Value = "${key}",
        };

        Assert.IsType<BearerAuth>(bearer);
        Assert.IsType<BasicAuth>(basic);
        var key = Assert.IsType<ApiKeyAuth>(apiKey);
        Assert.Equal(ApiKeyLocation.Header, key.Location);
    }
}
