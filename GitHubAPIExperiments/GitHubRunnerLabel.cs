using Newtonsoft.Json;

namespace GitHubAPIExperiments
{
    /// <summary>
    /// Describes a GitHub Runner Label
    /// </summary>
    public class GitHubRunnerLabel
    {
        /// <summary>
        /// ID of the Label
        /// </summary>
        [JsonProperty("id")]
        public long ID { get; set; }

        /// <summary>
        /// Name of the Label
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Type of the Label (read-only, etc.)
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}