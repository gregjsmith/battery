using Battery.Core.Results;
using Xunit;

namespace Battery.Core.Tests.Results;

public class RunResultTests
{
    [Fact]
    public void Totals_add_up_across_suites()
    {
        var run = new RunResult
        {
            Duration = TimeSpan.FromSeconds(2),
            SuiteResults =
            [
                Suite("albums", TestOutcome.Passed, TestOutcome.Failed),
                Suite("billing", TestOutcome.Passed, TestOutcome.Errored),
            ],
        };

        Assert.Equal(4, run.TotalTests);
        Assert.Equal(2, run.PassedCount);
        Assert.Equal(1, run.FailedCount);
        Assert.Equal(1, run.ErroredCount);
        Assert.False(run.Passed);
    }

    [Fact]
    public void A_run_passes_only_when_every_suite_passes()
    {
        var run = new RunResult
        {
            Duration = TimeSpan.Zero,
            SuiteResults = [Suite("albums", TestOutcome.Passed, TestOutcome.Passed)],
        };

        Assert.True(run.Passed);
    }

    private static SuiteResult Suite(string name, params TestOutcome[] outcomes) =>
        new()
        {
            SuiteName = name,
            Duration = TimeSpan.Zero,
            TestResults = outcomes
                .Select((outcome, index) => new TestResult
                {
                    TestName = $"{name}-{index}",
                    Outcome = outcome,
                    Duration = TimeSpan.Zero,
                })
                .ToList(),
        };
}
