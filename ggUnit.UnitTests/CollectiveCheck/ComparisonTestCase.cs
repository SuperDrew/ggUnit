namespace ggUnit.UnitTests.CollectiveCheck
{
    using System.Collections.Generic;
    using System.Linq;

    using GgUnit.CollectiveCheck;

    public class ComparisonTestCase<T> : ITestCase
    {
        public ComparisonTestCase()
        {
            this.Checks = new List<Result>();
            this.Include = true;
        }

        public string Name { get; set; }
        public List<Result> Checks { get; set; }
        public bool Include { private get; set; }
        public T Actual { get; set; }
        public T Expected { get; set; }

        public State CalculateResult()
        {
            if (this.Include)
            {
                return this.Checks.Any(x => x.State == State.Failed) ? State.Failed : State.Passed;
            }

            return State.Ignored;
        }

        public override string ToString()
        {
            return $"Name: {this.Name}, Number of checks: {this.Checks.Count}, State: {this.CalculateResult()}";
        }
    }
}