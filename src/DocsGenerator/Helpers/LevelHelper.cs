using System;
using Codacy.Engine.Seed.Patterns;

namespace Codacy.TSQLLint.DocsGenerator.Helpers
{
    /// <summary>
    ///     Helper to convert to codacy level from a TSQLLint rule
    /// </summary>
    public static class LevelHelper
    {
        /// <summary>
        ///     This map a rule to a codacy level.
        /// </summary>
        /// <param name="rule">rule name</param>
        /// <returns>Codacy level</returns>
        /// <exception cref="NotImplementedException">When found a non explicit mapped rule</exception>
        public static Level ToLevel(string rule)
        {
            switch (rule)
            {
                case "keyword-capitalization":
                case "semicolon-termination":
                    return Level.Warning;

                case "conditional-begin-end":
                case "cross-database-transaction":
                case "data-compression":
                case "data-type-length":
                case "disallow-cursors":
                case "full-text":
                case "information-schema":
                case "linked-server":
                case "multi-table-alias":
                case "named-constraint":
                case "non-sargable":
                case "object-property":
                case "print-statement":
                case "schema-qualify":
                case "select-star":
                case "set-ansi":
                case "set-nocount":
                case "set-quoted-identifier":
                case "set-transaction-isolation-level":
                case "set-variable":
                case "upper-lower":
                case "unicode-string":
                    return Level.Error;

                default:
                    throw new NotImplementedException($"Should map {rule} rule category");
            }
        }
    }
}
