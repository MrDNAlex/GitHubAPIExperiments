using NanoDNA.GitHubActionsManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubAPIExperiments
{
    internal class Program
    {
        public static string GitHubPAT { get; set; }

        public static string Owner { get; set; } = "MrDNAlex";
        //public static string Owner { get; set; } = "Nano-DNA-Studios";

        public static string Repository { get; set; } = "GitHubAPIExperiments";
        //public static string Repository { get; set; } = "NanoDNA.DockerManager";
        //public static string Repository { get; set; } = "DNA.GitHubActionsWorkerManager";


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

            //SpawnRunners(repo);

            Workflow[] workflows = repo.GetWorkflows();

            foreach (Workflow workflow in workflows)
            {
                if (workflow.Status != "queued")
                    continue;

                RunnerBuilder builder = new RunnerBuilder($"GitHubAPIExperiments-{workflow.ID}", repo, true);

                builder.AddLabel($"run-{workflow.ID}");

                Runner runner = builder.Build();

                runner.Start();

                Thread.Sleep(5000);

                //runner.WaitForNotBusy();

                //runner.Stop();
            }


            // repo.GetWorkflowJobs();
        }

        static private void SpawnRunners (Repository repo )
        {
            List<Runner> runners = new List<Runner>();

            for (int i = 0; i < 3; i++)
            {
                RunnerBuilder builder = new RunnerBuilder($"GitHubAPIExperiments{i}", repo, true);

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
