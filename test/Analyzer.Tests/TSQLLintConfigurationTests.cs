using System.Collections.Generic;
using Codacy.TSQLLint.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace Codacy.TSQLLint.Tests
{
    public class TSQLLintConfigurationTests
    {
        [Fact]
        public void JsonTest()
        {
            var config = new TSQLLintConfiguration
            {
                Rules = new Dictionary<string, string>
                {
                    {"rule1", "error"},
                    {"rule2", "off"}
                },
                Plugins = new Dictionary<string, string>
                {
                    {"tuna-plugin", "plugins/tuna-plugin/TunaPlugin.dll"}
                },
                CompatibilityLevel = 120
            };

            var configJSON =
                "{\"rules\":{\"rule1\": \"error\",\"rule2\": \"off\"},\"plugins\":{\"tuna-plugin\": \"plugins/tuna-plugin/TunaPlugin.dll\"},\"compatability-level\": 120}";

            Assert.Equal(config.ToString(),
                JsonConvert.DeserializeObject<TSQLLintConfiguration>(configJSON).ToString());
        }
    }
}
