using Codacy.Engine.Seed.Results;
using Codacy.TSQLLint.Reporters;
using TSQLLint.Common;
using Xunit;

namespace Codacy.TSQLLint.Tests
{
    public class CodacyConsoleReporterTests
    {
        public CodacyConsoleReporterTests()
        {
            reporter = new CodacyConsoleReporter();
        }

        private class RuleViolationTest : IRuleViolation
        {
            public int Column { get; set; }
            public string FileName { get; set; }
            public int Line { get; set; }
            public string RuleName { get; set; }
            public RuleViolationSeverity Severity { get; set; }
            public string Text { get; set; }
        }

        private readonly CodacyConsoleReporter reporter;

        [Fact]
        public void CodacyResultTest()
        {
            var result = new CodacyResult
            {
                Filename = "foo.sql",
                Line = 10,
                Message = "Just a test",
                PatternId = "pattern-test"
            };
            reporter.ReportViolation(result);

            Assert.Equal(result.ToString(), reporter.Results[0].ToString());
        }

        [Fact]
        public void ReportViolationTest()
        {
            reporter.ReportViolation("bar.sql", "10", "1", "error", "pattern-test", "Just a test");

            var expectedResult = new CodacyResult
            {
                Filename = "bar.sql",
                Line = 10,
                Message = "Just a test",
                PatternId = "pattern-test"
            }.ToString();

            Assert.Equal(expectedResult, reporter.Results[0].ToString());
        }

        [Fact]
        public void RuleViolationReportTest()
        {
            reporter.ReportViolation(new RuleViolationTest
            {
                FileName = "foobar.sql",
                Column = 1,
                Line = 15,
                RuleName = "pattern-test-2",
                Severity = RuleViolationSeverity.Error,
                Text = "Just another test"
            });

            var expectedResult = new CodacyResult
            {
                Filename = "foobar.sql",
                Line = 15,
                Message = "Just another test",
                PatternId = "pattern-test-2"
            }.ToString();

            Assert.Equal(expectedResult, reporter.Results[0].ToString());
        }
    }
}
