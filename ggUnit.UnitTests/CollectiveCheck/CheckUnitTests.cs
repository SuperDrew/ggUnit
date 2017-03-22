namespace ggUnit.UnitTests.CollectiveCheck
{
    using System;

    using GgUnit.CollectiveCheck;
    using GgUnit.Generator;

    using NUnit.Framework;

    [TestFixture]
    public class CheckUnitTests
    {
        [Test]
        [TestCase(100, 30, Category = "ShortRunning")]
        [TestCase(10000, 100, Category = "LongRunning")]
        [TestCase(100000, 500, Category = "VeryLongRunning")]
        public void AreEqualShouldReturnTrue(int numberOfCasesToCheck, int maxLengthOfString = 50)
        {
            var testResult = new TestResult<ComparisonTestCase<string>>();
            var regex = $"^.{{1,{maxLengthOfString}}}$";
            for (var i = 0; i < numberOfCasesToCheck; i++)
            {
                var testString = RegexGenerator.GenerateMatch(regex);
                var testCase = new ComparisonTestCase<string>
                                   {
                                       Actual = testString,
                                       Expected = testString,
                                       Name = "Check string comparison is equal."
                                   };
                testResult.TestCases.Add(testCase);
                testCase.Checks.Add(Check.AreEqual(testString, testString, testString));
            }

            var summary = testResult.CreateResultSummary().ToString();
            Assert.That(testResult.CalculateResult(), Is.EqualTo(State.Passed), summary);
            Console.WriteLine(summary);
        }
    }
}
