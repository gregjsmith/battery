using Battery.Core.Definitions;
using Xunit;

namespace Battery.Core.Tests.Definitions;

public class ExtractionTests
{
    [Fact]
    public void Extraction_holds_a_name_and_a_source()
    {
        var extraction = new Extraction
        {
            Name = "token",
            Source = new BodyExtractionSource { JsonPath = "$.access_token" },
        };

        Assert.Equal("token", extraction.Name);
        var source = Assert.IsType<BodyExtractionSource>(extraction.Source);
        Assert.Equal("$.access_token", source.JsonPath);
    }

    [Fact]
    public void Each_source_kind_is_a_distinct_type()
    {
        ExtractionSource body = new BodyExtractionSource { JsonPath = "$.id" };
        ExtractionSource header = new HeaderExtractionSource { HeaderName = "Location" };
        ExtractionSource status = new StatusExtractionSource();

        Assert.IsType<BodyExtractionSource>(body);
        Assert.IsType<HeaderExtractionSource>(header);
        Assert.IsType<StatusExtractionSource>(status);
    }
}
