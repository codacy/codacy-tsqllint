using System.Collections.Generic;
using Codacy.Engine.Seed;
using Newtonsoft.Json;

namespace Codacy.TSQLLint.Configuration
{
    /// <summary>
    ///     TSQLLint Configuration Model
    ///     This is to create the configuration file for tsqllint tool
    /// </summary>
    public sealed class TSQLLintConfiguration : JsonModel
    {
        /// <summary>
        ///     Dictionary of rules activated or deactivated ("error", "warning", "off")
        /// </summary>
        [JsonProperty(PropertyName = "rules")]
        public Dictionary<string, string> Rules { get; set; }

        /// <summary>
        ///     Dictionary of available plugins
        /// </summary>
        [JsonProperty(PropertyName = "plugins")]
        public Dictionary<string, string> Plugins { get; set; }

        /// <summary>
        ///     Compatibility level of the tool with TSQL language
        /// </summary>
        [JsonProperty(PropertyName = "compatability-level")]
        public int? CompatibilityLevel { get; set; }
    }
}
