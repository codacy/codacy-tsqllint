using System.Collections.Generic;

namespace Codacy.TSQLLint.DocsGenerator.Helpers
{
    public static class DefaultPatterns
    {
        public static ICollection<string> Patterns { get; } = new HashSet<string> {
            "conditional-begin-end",
            "cross-database-transaction",
            "data-compression",
            "data-type-length",
            "disallow-cursors",
            "full-text",
            "information-schema",
            "keyword-capitalization",
            "linked-server",
            "multi-table-alias",
            "non-sargable",
            "object-property",
            "print-statement",
            "schema-qualify",
            "select-star",
            "semicolon-termination",
            "set-ansi",
            "set-nocount",
            "set-quoted-identifier",
            "set-transaction-isolation-level",
            "set-variable",
            "unicode-string",
            "upper-lower"
        };
    }
}
