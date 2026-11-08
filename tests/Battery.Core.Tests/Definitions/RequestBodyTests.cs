using System.Text.Json.Nodes;
using Battery.Core.Definitions;
using Xunit;

namespace Battery.Core.Tests.Definitions;

public class RequestBodyTests
{
    [Fact]
    public void Each_body_kind_is_a_distinct_type()
    {
        RequestBody inline = new JsonInlineBody { Json = JsonNode.Parse("""{"name":"Aja"}""")! };
        RequestBody file = new JsonFileBody { Path = "data/album.json" };
        RequestBody form = new FormBody
        {
            Fields = new Dictionary<string, string> { ["grant_type"] = "client_credentials" },
        };
        RequestBody text = new TextBody { Text = "hello" };

        Assert.IsType<JsonInlineBody>(inline);
        Assert.IsType<JsonFileBody>(file);
        Assert.IsType<FormBody>(form);
        Assert.IsType<TextBody>(text);
    }

    [Fact]
    public void Form_body_fields_default_to_empty()
    {
        var form = new FormBody();

        Assert.Empty(form.Fields);
    }
}
