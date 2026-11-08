using Battery.Core.Definitions;
using Xunit;

namespace Battery.Core.Tests.Definitions;

public class RequestTests
{
    [Fact]
    public void Collections_default_to_empty_not_null()
    {
        var request = new Request { Method = RequestMethod.Get, Url = "/albums" };

        Assert.Empty(request.Query);
        Assert.Empty(request.Headers);
        Assert.Null(request.Body);
        Assert.Null(request.Auth);
        Assert.Null(request.Timeout);
        Assert.Null(request.FollowRedirects);
        Assert.Null(request.Tls);
    }

    [Fact]
    public void With_copies_and_changes_one_value()
    {
        var request = new Request { Method = RequestMethod.Get, Url = "/albums" };
        var post = request with { Method = RequestMethod.Post };

        Assert.Equal(RequestMethod.Get, request.Method);
        Assert.Equal(RequestMethod.Post, post.Method);
        Assert.Equal(request.Url, post.Url);
    }
}
