using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Codacy.Engine.Seed.Patterns;
using Codacy.TSQLLint.DocsGenerator.Helpers;
using TSQLLint.Core.Interfaces;

namespace Codacy.TSQLLint.DocsGenerator
{
    internal static class Program
    {
        private const string docsFolder = @"docs/";
        private const string descriptionFolder = docsFolder + @"description/";
        private const string rulesDocumentation = @"tsqllint/documentation/rules/";

        public static int Main(string[] args)
        {
            Directory.CreateDirectory(docsFolder);
            Directory.CreateDirectory(descriptionFolder);

            var files = Directory.GetFiles(rulesDocumentation);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(descriptionFolder, fileName);
                File.Copy(file, destFile, true);
            }

// The reference to the tsqllint version from their repository was deleted.
// Below should be the same version as from file tsqllint.version
            var tsqllintVersion = "1.14.5";

            var patternsFile = new CodacyPatterns
            {
                Name = "tsqllint",
                Version = tsqllintVersion,
                Patterns = new List<Pattern>()
            };

            var descriptions = new CodacyDescription();

            var types = Assembly
                .LoadFile(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/TSQLLint.Infrastructure.dll")
                .GetTypes()
                .Where(t => t.Namespace == "TSQLLint.Infrastructure.Rules" &&
                            t.GetInterfaces().Contains(typeof(ISqlRule)) &&
                            t.Name.EndsWith("Rule")
                );

            foreach (var ruleType in types)
            {
                var instance = Activator.CreateInstance(ruleType,
                    (Action<string, string, int, int>) ((_, __, ___, ____) => { }));

                var description = new Description();

                foreach (var prop in ruleType.GetProperties())
                {
                    if (prop.Name == "RULE_NAME")
                    {
                        var patternId = (string) prop.GetValue(instance);
                        var enabled = DefaultPatterns.Patterns.Contains(patternId);
                        var pattern = new Pattern(patternId,
                            LevelHelper.ToLevel(patternId),
                            CategoryHelper.ToCategory(patternId),
                            enabled: enabled);
                        patternsFile.Patterns.Add(pattern);

                        description.PatternId = patternId;
                    }
                    else if (prop.Name == "RULE_TEXT")
                    {
                        description.Title = (string) prop.GetValue(instance);
                    }
                }

                descriptions.Add(description);
            }

            File.WriteAllText(docsFolder + @"/patterns.json", patternsFile.ToString(true) + Environment.NewLine);
            File.WriteAllText(descriptionFolder + @"/description.json",
                descriptions.ToString(true) + Environment.NewLine);

            return 0;
        }
    }
}
