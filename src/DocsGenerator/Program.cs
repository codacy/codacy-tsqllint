using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            try
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
            
                // Read the version from the file
                var tsqllintVersion = File.ReadAllText("tsqllint.version").Trim();

                var patternsFile = new CodacyPatterns
                {
                    Name = "tsqllint",
                    Version = tsqllintVersion,
                    Patterns = new List<Pattern>()
                };

                var descriptions = new CodacyDescription();

                var assembly = Assembly.LoadFile(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/TSQLLint.Infrastructure.dll");
                var types = assembly.GetTypes()
                    .Where(t => t.Namespace == "TSQLLint.Infrastructure.Rules" &&
                                t.GetInterfaces().Contains(typeof(ISqlRule)) &&
                                t.IsClass && // Ensure it is a class
                                t.Name.EndsWith("Rule")
                    );

                foreach (var ruleType in types)
                {
                    if (ruleType.IsAbstract)
                    {
                        GenerateDocumentationForAbstractClass(ruleType, patternsFile, descriptions);
                    }
                    else
                    {
                        GenerateDocumentationForConcreteClass(ruleType, patternsFile, descriptions);
                    }
                }

                File.WriteAllText(docsFolder + @"/patterns.json", patternsFile.ToString(true) + Environment.NewLine);
                File.WriteAllText(descriptionFolder + @"/description.json",
                    descriptions.ToString(true) + Environment.NewLine);

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return 1;
            }
        }
        private static void GenerateDocumentationForConcreteClass(Type ruleType, CodacyPatterns patternsFile, CodacyDescription descriptions)
        {
            try
            {
                var instance = Activator.CreateInstance(ruleType, 
                    (Action<string, string, int, int>)((_, __, ___, ____) => { }));

                var description = new Description();

                foreach (var prop in ruleType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (prop.Name == "RULE_NAME")
                    {
                        var patternId = (string)prop.GetValue(instance);
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
                        description.Title = (string)prop.GetValue(instance);
                    }
                }

                descriptions.Add(description);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating instance of {ruleType.Name}: {ex.Message}");
            }
        }
        private static void GenerateDocumentationForAbstractClass(Type ruleType, CodacyPatterns patternsFile, CodacyDescription descriptions)
        {
            var subclasses = ruleType.Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(ruleType));

            foreach (var subclass in subclasses)
            {
                GenerateDocumentationForConcreteClass(subclass, patternsFile, descriptions);
            }
        }
    }
}
