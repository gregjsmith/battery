using Battery.Core;
using Xunit;

namespace Battery.Core.Tests;

public class CoreInfoTests
{
    [Fact]
    public void Name_is_the_core_assembly_name()
    {
        Assert.Equal("Battery.Core", CoreInfo.Name);
    }
}
