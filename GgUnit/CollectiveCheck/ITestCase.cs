namespace GgUnit.CollectiveCheck
{
    using System.Collections.Generic;

    public interface ITestCase
    {
        string Name { get; set; }

        List<Result> Checks { get; set; }

        bool Include { set; }

        State CalculateResult();
    }
}