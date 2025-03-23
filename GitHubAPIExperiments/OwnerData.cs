using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubAPIExperiments
{
    /// <summary>
    /// Owner Information from the Webhook Payload
    /// </summary>
    public class OwnerData
    {
        /// <summary>
        /// Login Name of the Repository Owner
        /// </summary>
        [JsonProperty("login")]
        public string Login { get; set; }

        /// <summary>
        /// Extra Data provided to the Owner Object
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JToken> ExtraData { get; set; }
    }
}
