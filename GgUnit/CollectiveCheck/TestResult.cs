namespace GgUnit.CollectiveCheck
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TestResult<T> where T : ITestCase
    {
        public TestResult()
        {
            this.TestCases = new List<T>();
        }

        public string Name { get; set; }

        public List<T> TestCases { get; set; }

        public State CalculateResult()
        {
            return this.TestCases.Any(y => y.CalculateResult() == State.Failed) ? State.Failed : State.Passed;
        }

        public virtual StringBuilder CreateResultSummary()
        {
            return TestResultSummary.CreateSummary(this.Name, this.TestCases);
        }

        public override string ToString()
        {
            return string.Format("Name: {0}", this.Name);
        }
    }
}
