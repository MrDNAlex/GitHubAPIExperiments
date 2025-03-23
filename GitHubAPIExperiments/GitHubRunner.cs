using NanoDNA.DockerManager;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace GitHubAPIExperiments
{
    /// <summary>
    /// Describes a GitHub Runner Instances Information
    /// </summary>
    public class GitHubRunner
    {

        /// <summary>
        /// ID of the Runner Instance
        /// </summary>
        [JsonProperty("id")]
        public long ID { get; set; }

        /// <summary>
        /// Name of the Runner Instance
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Operating System of the Runner Instance
        /// </summary>
        [JsonProperty("os")]
        public string OS { get; set; }

        /// <summary>
        /// Status of the Runner Instance (online , offline, etc.)
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Flag indicating if the Runner Instance is Busy running a Workflow
        /// </summary>
        [JsonProperty("busy")]
        public bool Busy { get; set; }

        /// <summary>
        /// Labels assigned to the Runner Instance
        /// </summary>
        [JsonProperty("labels")]
        public GitHubRunnerLabel[] Labels { get; set; }

        public DockerContainer Container { get; set; }

        public void StopRunner ()
        {
            Container.Execute($"/home/GitWorker/ActionRunner/config.sh remove --token {Container.EnvironmentVariables["TOKEN"]}");

            Thread.Sleep(2000);

            Console.WriteLine(Container.GetLogs());

            Container.Stop();
            Container.Remove();
        }
    }
}