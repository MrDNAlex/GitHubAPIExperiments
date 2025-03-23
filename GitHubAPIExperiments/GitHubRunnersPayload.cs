using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;

namespace GitHubAPIExperiments
{
    /// <summary>
    /// JSON Payload received from GitHub API for Runner Instances
    /// </summary>
    public class GitHubRunnersPayload : BaseGitHubAPI
    {
        /// <summary>
        /// Number of Runners currently Registered on GitHub
        /// </summary>
        [JsonProperty("total_count")]
        public long TotalCount { get; set; }

        /// <summary>
        /// List of Runners currently Registered on GitHub
        /// </summary>
        [JsonProperty("runners")]
        public GitHubRunner[] Runners { get; set; }

        /// <summary>
        /// Extra Data provided to the Owner Object
        /// </summary>
        //[JsonExtensionData]
        //public Dictionary<string, JToken> ExtraData { get; set; }

        public static GitHubRunnersPayload GetRunners(string owner, string repository, string githubPAT)
        {
            using (HttpResponseMessage response = GetClient(githubPAT).GetAsync($"https://api.github.com/repos/{owner}/{repository}/actions/runners").Result)
            {
                if (!response.IsSuccessStatusCode)
                    return null;

                string responseBody = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<GitHubRunnersPayload>(responseBody);
            }
        }
    }
}
