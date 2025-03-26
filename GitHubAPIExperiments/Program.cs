using NanoDNA.GitHubActionsManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace GitHubAPIExperiments
{
    internal class Program
    {
        public static string GitHubPAT { get; set; }

        public static string Owner { get; set; } = "MrDNAlex";

        public static string Repository { get; set; } = "GitHubAPIExperiments";


        static void Main(string[] args)
        {   
            GitHubPAT = File.ReadAllText("githubtoken.txt");

            Console.WriteLine(GitHubPAT);

            //GetRepositoryInfo();

            //GitHubRepository repo = GitHubRepository.GetRepo(Owner, Repository, GitHubPAT);
            //
            //Console.WriteLine(JsonConvert.SerializeObject(repo, Formatting.Indented));
            //
            //GitHubRunner runner = repo.StartGitHubActionsRunner(GitHubPAT);
            //
            //GitHubRunnersPayload runners = GitHubRunnersPayload.GetRunners(Owner, Repository, GitHubPAT);
            //
            //Console.WriteLine(JsonConvert.SerializeObject(runners, Formatting.Indented));
            //
            //runner.StopRunner();

            TestingLibrary();
        }

        public static void GetRepositoryInfo ()
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", $"token {GitHubPAT}");
            client.DefaultRequestHeaders.Add("User-Agent", "CSharp-App");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");

            GitHubRepository repoPayload;

            using (HttpResponseMessage response = client.GetAsync($"https://api.github.com/repos/{Owner}/{Repository}").Result)
            {
                if (!response.IsSuccessStatusCode)
                    return;

                string responseBody = response.Content.ReadAsStringAsync().Result;

                repoPayload = JsonConvert.DeserializeObject<GitHubRepository>(responseBody);

                Console.WriteLine(JsonConvert.SerializeObject(repoPayload, Formatting.Indented));
            }

            using (HttpResponseMessage response = client.GetAsync($"{repoPayload.URL}").Result)
            {
                if (!response.IsSuccessStatusCode)
                    return;

                string responseBody = response.Content.ReadAsStringAsync().Result;

                repoPayload = JsonConvert.DeserializeObject<GitHubRepository>(responseBody);

                Console.WriteLine(JsonConvert.SerializeObject(repoPayload, Formatting.Indented));
            }
        }

        public static void TestingLibrary ()
        {
            GitHubAPIClient.SetGitHubPAT(GitHubPAT);

            Repository repo = NanoDNA.GitHubActionsManager.Repository.GetRepo(Owner, Repository);

            List<Runner> runners = new List<Runner>();

            for (int i = 0; i < 3; i ++)
            {
                RunnerBuilder builder = new RunnerBuilder($"GitHubAPIExperiments{i}", repo);

                builder.AddLabel("GitHub APIExperiments");

                Runner runner = builder.Build();

                runner.Start();

                runners.Add(runner);
            }

            repo.GetRunners();


            //Runner.GetRunners(Owner, Repository);

            foreach (Runner runner in runners)
            {
                Console.WriteLine(JsonConvert.SerializeObject(runner, Formatting.Indented));

                runner.Stop();
            }
        }
    }
}
