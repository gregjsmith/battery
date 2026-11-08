using Battery.Core.Results;
using Xunit;

namespace Battery.Core.Tests.Results;

public class SuiteResultTests
{
    [Fact]
    public void Counts_reflect_the_test_outcomes()
    {
        var suite = new SuiteResult
        {
            SuiteName = "albums",
            Duration = TimeSpan.FromSeconds(1),
            TestResults =
            [
                Test("a", TestOutcome.Passed),
                Test("b", TestOutcome.Passed),
                Test("c", TestOutcome.Failed),
                Test("d", TestOutcome.Errored),
            ],
        };

        Assert.Equal(4, suite.Total);
        Assert.Equal(2, suite.PassedCount);
        Assert.Equal(1, suite.FailedCount);
        Assert.Equal(1, suite.ErroredCount);
        Assert.False(suite.Passed);
    }

    [Fact]
    public void A_suite_passes_only_when_every_test_passes()
    {
        var suite = new SuiteResult
        {
            SuiteName = "albums",
            Duration = TimeSpan.Zero,
            TestResults = [Test("a", TestOutcome.Passed), Test("b", TestOutcome.Passed)],
        };

        Assert.True(suite.Passed);
    }

    private static TestResult Test(string name, TestOutcome outcome) =>
        new() { TestName = name, Outcome = outcome, Duration = TimeSpan.Zero };
}
