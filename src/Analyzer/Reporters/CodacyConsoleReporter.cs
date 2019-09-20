using System;
using System.Collections.Generic;
using Codacy.Engine.Seed.Results;
using TSQLLint.Common;

namespace Codacy.TSQLLint.Reporters
{
    /// <summary>
    ///     Custom console reporter for codacy report scheme.
    ///     This is a class needed by the tool to report the violated rules
    ///     on TSQL code.
    /// </summary>
    public class CodacyConsoleReporter : IReporter
    {
        /// <summary>
        ///     Codacy results list
        /// </summary>
        public readonly List<CodacyResult> Results = new List<CodacyResult>();

        /// <summary>
        ///     Report to stdout a message.
        /// </summary>
        /// <param name="message">Message to report</param>
        public virtual void Report(string message)
        {
            // Need to be implemented by the interface, but useless to report
            // codacy format. So it needs to be empty
        }

        /// <summary>
        ///     Summary of total analyzed files and total run time.
        /// </summary>
        /// <param name="timespan">Total run time</param>
        /// <param name="fileCount">number of files analyzed</param>
        public void ReportResults(TimeSpan timespan, int fileCount)
        {
            // Need to be implemented by the interface, but useless to report
            // codacy format. So it needs to be empty
        }

        public void ReportFileResults()
        {
            // Need to be implemented by the interface, but useless to report
            // codacy format. So it needs to be empty
        }

        /// <summary>
        ///     Report a rule violation.
        ///     This is called when a rule is violated on the TSQL code.
        /// </summary>
        /// <param name="violation">rule violation model</param>
        public void ReportViolation(IRuleViolation violation)
        {
            ReportViolation(
                new CodacyResult
                {
                    Filename = violation.FileName.Substring(
                        violation.FileName.IndexOf("/", StringComparison.CurrentCulture) + 1),
                    Message = violation.Text,
                    Line = violation.Line,
                    PatternId = violation.RuleName
                });
        }

        /// <summary>
        ///     Report a rule violation.
        ///     This is called when a rule is violated on the TSQL code.
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="line">line number</param>
        /// <param name="column">index of the line</param>
        /// <param name="severity">rule severity</param>
        /// <param name="ruleName">rule name</param>
        /// <param name="violationText">violation text description</param>
        public void ReportViolation(string fileName, string line, string column, string severity, string ruleName,
            string violationText)
        {
            var result = new CodacyResult
            {
                Filename = fileName.Substring(fileName.IndexOf("/", StringComparison.CurrentCulture) + 1),
                Message = violationText,
                Line = long.Parse(line),
                PatternId = ruleName
            };

            ReportViolation(result);
        }

        /// <summary>
        ///     Report a rule violation.
        ///     This is called when a rule is violated on the TSQL code.
        /// </summary>
        /// <param name="result">codacy result model</param>
        public void ReportViolation(CodacyResult result)
        {
            Results.Add(result);
        }
    }
}
