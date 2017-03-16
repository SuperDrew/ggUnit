namespace GgUnit.CollectiveCheck
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class TestResultSummary
    {
        public static StringBuilder CreateSummary<T>(string name, List<T> testCases) where T : ITestCase
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(name);
            sb.AppendLine("Summary:");
            AppendStateResult(testCases, sb, State.Passed);
            AppendStateResult(testCases, sb, State.Failed);
            AppendStateResult(testCases, sb, State.Ignored);
            sb.AppendFormat("  Total test cases: {0}", testCases.Count);
            sb.AppendLine();
            sb.AppendLine("Total number of checks: " + testCases.Sum(testCase => testCase.Checks.Count));
            return sb;
        }

        private static void AppendStateResult<T>(List<T> results, StringBuilder sb, State state) where T : ITestCase
        {
            sb.AppendFormat(" TestCases {0}: {1}", state, results.Count(x => x.CalculateResult() == state));
            sb.AppendLine();
        }
    }
}