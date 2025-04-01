using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using NanoDNA.DockerManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubAPIExperiments
{
    internal class GitHubRepository : BaseGitHubAPI
    {
        /// <summary>
        /// Full Name of the Repository (Owner/RepoName)
        /// </summary>
        [JsonProperty("full_name")]
        public string FullName { get; set; }

        /// <summary>
        /// Name of the Repository
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Owner of the Repository
        /// </summary>
        [JsonProperty("owner")]
        public OwnerData Owner { get; set; }

        /// <summary>
        /// Languages the Repository is Written in
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; set; }

        /// <summary>
        /// HTML URL to the Repository
        /// </summary>
        [JsonProperty("html_url")]
        public string HtmlURL { get; set; }

        /// <summary>
        /// API URL to the Repository
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }

        /// <summary>
        /// Extra Data Associated with the Repository
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JToken> ExtraData { get; set; }

        public static GitHubRepository GetRepo (string owner, string repository, string githubPAT)
        {
            using (HttpResponseMessage response = GetClient(githubPAT).GetAsync($"https://api.github.com/repos/{owner}/{repository}").Result)
            {
                if (!response.IsSuccessStatusCode)
                    return null;

                string responseBody = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<GitHubRepository>(responseBody);
            }
        }

        public string GetToken (string githubPAT)
        {
            JObject tokenResponse;
            string tokenRegisterURL = $"{URL}/actions/runners/registration-token";

            using (HttpResponseMessage response = GetClient(githubPAT).PostAsync(tokenRegisterURL, null).Result)
            {
                if (!response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to get Token");
                    Console.ResetColor();
                    return String.Empty;
                }

                tokenResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            }

            return tokenResponse["token"].ToString();

        }

        public GitHubRunner StartGitHubActionsRunner (string githubPAT)
        {
            DockerContainer container = new DockerContainer("gitapiexperiments", "mrdnalex/github-action-worker-container", true);

            container.AddEnvironmentVariable("REPO", HtmlURL);
            container.AddEnvironmentVariable("TOKEN", GetToken(githubPAT));
            container.AddEnvironmentVariable("RUNNERGROUP", "");
            container.AddEnvironmentVariable("RUNNERNAME", "GitHubAPIExperiments");
            container.AddEnvironmentVariable("RUNNERLABELS", "self-hosted,gitapiexperiment");
            container.AddEnvironmentVariable("RUNNERWORKDIR", "WorkDir");

            container.Start();

            Thread.Sleep(10000);

            GitHubRunnersPayload payload = GitHubRunnersPayload.GetRunners(Owner.Login, Name, githubPAT);

            Console.WriteLine("Displaying Payload");
            Console.WriteLine(JsonConvert.SerializeObject(payload, Formatting.Indented));

            GitHubRunner runner = payload.Runners[0];

            runner.Container = container;


            return runner;
        }







    }
}
