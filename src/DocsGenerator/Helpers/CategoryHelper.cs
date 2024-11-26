using System;
using Codacy.Engine.Seed.Patterns;

namespace Codacy.TSQLLint.DocsGenerator.Helpers
{
    /// <summary>
    ///     Helper to convert to codacy category from a TSQLLint rule
    /// </summary>
    public static class CategoryHelper
    {
        /// <summary>
        ///     This map a rule to a codacy category.
        /// </summary>
        /// <param name="rule">rule name</param>
        /// <returns>Codacy category</returns>
        /// <exception cref="NotImplementedException">When found a non explicit mapped rule</exception>
        public static Category ToCategory(string rule)
        {
            switch (rule)
            {
                case "data-compression":
                case "data-type-length":
                case "disallow-cursors":
                case "full-text":
                case "linked-server":
                case "non-sargable":
                    return Category.Performance;

                case "conditional-begin-end":
                case "cross-database-transaction":
                case "information-schema":
                case "multi-table-alias":
                case "named-constraint":
                case "object-property":
                case "schema-qualify":
                case "select-star":
                case "set-ansi":
                case "set-nocount":
                case "set-quoted-identifier":
                case "set-transaction-isolation-level":
                case "set-variable":
                case "upper-lower":
                case "unicode-string":
                case "case-sensitive-variables":
                case "count-star":
                case "delete-where":
                case "duplicate-empty-line":
                case "duplicate-go":
                case "update-where":
                    return Category.ErrorProne;

                case "keyword-capitalization":
                case "semicolon-termination":
                case "CaseSensitiveVariablesRule":
                    return Category.CodeStyle;

                case "print-statement":
                    return Category.Security;

                default:
                    throw new NotImplementedException($"Should map {rule} rule category");
            }
        }
    }
}
