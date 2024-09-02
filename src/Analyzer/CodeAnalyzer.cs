using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codacy.Engine.Seed;
using Codacy.Engine.Seed.Results;
using Codacy.TSQLLint.Configuration;
using Codacy.TSQLLint.Reporters;
using Newtonsoft.Json;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Configuration;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Plugins;
using TSQLLint.Infrastructure.Reporters;

namespace Codacy.TSQLLint
{
    /// <summary>
    ///     Tool integration for TSQLLint using the Seed class.
    /// </summary>
    public class CodeAnalyzer : Engine.Seed.CodeAnalyzer, IDisposable
    {
        private const string sqlExtension = ".sql";
        private const string defaultTSQLLintConfiguration = ".tsqllintrc";
        private readonly IConfigReader configReader;
        private readonly CodacyConsoleReporter reporter;
        private readonly IConsoleTimer timer;

        private readonly string tmpTSQLLintPath;

        /// <summary>
        ///     Tool integration constructor.
        ///     This will prepare everything needed for the tool to work with codacy
        ///     integration, using the Seed.
        /// </summary>
        public CodeAnalyzer() : base(sqlExtension)
        {
            timer = new ConsoleTimer();
            reporter = new CodacyConsoleReporter();
            configReader = new ConfigReader(reporter);

            // create temporary directory
            var tmpTSQLLintFolder = Path.Combine(Path.GetTempPath(), "tsqllint_" + Guid.NewGuid());
            Directory.CreateDirectory(tmpTSQLLintFolder);

            tmpTSQLLintPath = Path.Combine(tmpTSQLLintFolder, defaultTSQLLintConfiguration);

            var defaultTSQLLintConfigurationPath = Path.Combine("/src", defaultTSQLLintConfiguration);

            var tsqllintConfig = new TSQLLintConfiguration();
            if (!(PatternIds is null) && PatternIds.Any())
            {
                tsqllintConfig.Rules = new Dictionary<string, string>();
                foreach (var pattern in CurrentTool.Patterns)
                {
                    tsqllintConfig.Rules.Add(pattern.PatternId, "error");
                }

                foreach (var unusedPattern in Patterns.Patterns.Select(p => p.PatternId)
                    .Except(CurrentTool.Patterns.Select(p => p.PatternId)))
                {
                    tsqllintConfig.Rules.Add(unusedPattern, "off");
                }
            }
            else if (File.Exists(defaultTSQLLintConfigurationPath))
            {
                var tsqlliteJSON = File.ReadAllText(defaultTSQLLintConfigurationPath);
                var currentTSQLLintConfig = JsonConvert.DeserializeObject<TSQLLintConfiguration>(tsqlliteJSON);
                tsqllintConfig.Rules = currentTSQLLintConfig.Rules;
                tsqllintConfig.CompatibilityLevel = currentTSQLLintConfig.CompatibilityLevel;
            }
            else
            {
                tsqllintConfig.Rules = new Dictionary<string, string>();
                foreach (var pattern in Patterns.Patterns.Select(p => p.PatternId))
                {
                    tsqllintConfig.Rules.Add(pattern, "error");
                }
            }

            if (tsqllintConfig.CompatibilityLevel is null)
            {
                tsqllintConfig.CompatibilityLevel = 120;
            }

            File.WriteAllText(tmpTSQLLintPath, tsqllintConfig.ToString());
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Free temporary resources created on object construction.
        /// </summary>
        ~CodeAnalyzer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            // delete created temporary directory
            Directory.Delete(Path.GetDirectoryName(tmpTSQLLintPath), true);
        }

        /// <summary>
        ///     Run analyze task
        /// </summary>
        /// <param name="cancellationToken">task cancellation token</param>
        /// <returns>Task of the tool running</returns>
        protected override async Task Analyze(CancellationToken cancellationToken)
        {
            await Task.Run(RunTool, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        ///     Run the actual tool.
        ///     This will call the rule processors of the tool, handle the available plugins
        ///     and analyze every file.
        /// </summary>
        private void RunTool()
        {
            timer.Start();

            configReader.LoadConfig(tmpTSQLLintPath);

            var fragmentBuilder = new FragmentBuilder(configReader.CompatabilityLevel);
            var ruleVisitorBuilder = new RuleVisitorBuilder(configReader, reporter);
            var ruleVisitor = new SqlRuleVisitor(ruleVisitorBuilder, fragmentBuilder, reporter);
            var pluginHandler = new PluginHandler(reporter);
            var fileProcessor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, new FileSystem());

            pluginHandler.ProcessPaths(configReader.GetPlugins());

            foreach (var file in Config.Files)
            {
                try
                {
                    fileProcessor.ProcessPath(DefaultSourceFolder + file);
                }
                catch (Exception e)
                {
                    Logger.Send(e);

                    reporter.Results.Add(new CodacyResult
                    {
                        Filename = file,
                        Message = "could not parse the file"
                    });
                }
            }

            var resultBuilder = new StringBuilder();
            foreach (var result in reporter.Results)
            {
                resultBuilder.Append(result + Environment.NewLine);
            }

            Console.Write(resultBuilder.ToString());

            if (fileProcessor.FileCount > 0)
            {
                reporter.ReportResults(timer.Stop(), fileProcessor.FileCount);
            }
        }
    }
}
