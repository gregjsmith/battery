using Battery.Core.Definitions;
using Battery.Core.Results;
using Xunit;

namespace Battery.Core.Tests.Results;

public class TestResultTests
{
    [Fact]
    public void A_passed_test_carries_its_assertion_results()
    {
        var result = new TestResult
        {
            TestName = "get album",
            Outcome = TestOutcome.Passed,
            Duration = TimeSpan.FromMilliseconds(12),
            AssertionResults =
            [
                new AssertionResult
                {
                    Assertion = new StatusAssertion { ExpectedStatus = 200 },
                    Passed = true,
                    Expected = "200",
                    Actual = "200",
                },
            ],
        };

        Assert.Equal(TestOutcome.Passed, result.Outcome);
        Assert.Single(result.AssertionResults);
        Assert.Null(result.Error);
    }

    [Fact]
    public void An_errored_test_carries_an_error_message()
    {
        var result = new TestResult
        {
            TestName = "get album",
            Outcome = TestOutcome.Errored,
            Duration = TimeSpan.Zero,
            Error = "The connection timed out.",
        };

        Assert.Equal(TestOutcome.Errored, result.Outcome);
        Assert.Empty(result.AssertionResults);
        Assert.Equal("The connection timed out.", result.Error);
    }
}
