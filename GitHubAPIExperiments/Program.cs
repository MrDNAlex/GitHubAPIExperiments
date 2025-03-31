using NanoDNA.GitHubManager;
using NanoDNA.GitHubManager.Events;
using NanoDNA.GitHubManager.Models;
using NanoDNA.GitHubManager.Services;
using NanoDNA.ProcessRunner;
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

        //public static string Owner { get; set; } = "MrDNAlex";
        public static string Owner { get; set; } = "Nano-DNA-Studios";

        //public static string Repository { get; set; } = "GitHubAPIExperiments";
        public static string Repository { get; set; } = "NanoDNA.ProcessRunner";
        //public static string Repository { get; set; } = "DNA.GitHubActionsWorkerManager";


        static void Main(string[] args)
        {
            GitHubPAT = File.ReadAllText("githubtoken.txt").Trim();

            Console.WriteLine(GitHubPAT);

            GitHubAPIClient.SetGitHubPAT(GitHubPAT);

            CommandRunner runner = new CommandRunner();

            runner.RunCommand("(getent group docker | cut -d: -f3)");

            Console.WriteLine(runner.StandardOutput[0]);

            CommandRunner dockerRunner = new CommandRunner();

            string dockerCommand = $"docker run --name dindtest --privileged --group-add {runner.StandardOutput[0]} -v /var/run/docker.sock:/var/run/docker.sock -it -d mrdnalex/github-action-worker-container-dotnet";

            Console.WriteLine(dockerCommand);

            dockerRunner.RunCommand(dockerCommand);

            //TestEphemeralRunners(NanoDNA.GitHubManager.Models.Repository.GetRepository(Owner, "NanoDNA.GitHubManager"), true);
            //TestEphemeralRunners(NanoDNA.GitHubManager.Models.Repository.GetRepository(Owner, "NanoDNA.ProcessRunner"), true);
            //TestEphemeralRunners(NanoDNA.GitHubManager.Models.Repository.GetRepository(Owner, "NanoDNA.DockerManager"), true);
            //TestEphemeralRunners(NanoDNA.GitHubManager.Models.Repository.GetRepository("MrDNAlex", "GitHubAPIExperiments"), true);

            while (true)
            {
            }
        }

        public static void GetRepositoryInfo()
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

        public static void TestingLibrary()
        {
            GitHubAPIClient.SetGitHubPAT(GitHubPAT);

            Repository repo = NanoDNA.GitHubManager.Models.Repository.GetRepository(Owner, Repository);

            //SpawnRunners(repo);

            int count = 0;

            GitHubWebhookService webhookService = new GitHubWebhookService("myWebhookSecret");

            webhookService.On<WorkflowJobEvent>(worflowJob =>
            {
                WorkflowJobEvent workflowJobEvent = worflowJob as WorkflowJobEvent;
                Console.WriteLine("Received Workflow Job");
                File.WriteAllText(@$"C:\Users\MrDNA\Downloads\GitHubActionWorker\workflowJob-{count}.json", JsonConvert.SerializeObject(workflowJobEvent, Formatting.Indented));
                count++;
            });

            webhookService.On<WorkflowRunEvent>(workflowRun =>
            {
                WorkflowRunEvent workflowRunEvent = workflowRun as WorkflowRunEvent;
                Console.WriteLine("Received Workflow Run");
                File.WriteAllText(@$"C:\Users\MrDNA\Downloads\GitHubActionWorker\workflowRun-{count}.json", JsonConvert.SerializeObject(workflowRunEvent, Formatting.Indented));


                if (workflowRun.WorkflowRun.Status != "queued")
                    return;

                RunnerBuilder builder = new RunnerBuilder($"{workflowRunEvent.WorkflowRun.Repository.Name}-{workflowRun.WorkflowRun.ID}", "mrdnalex/github-action-worker-container-dotnet", repo, true);

                builder.AddLabel($"run-{workflowRun.WorkflowRun.ID}");

                Runner runner = builder.Build();

                runner.Start();

                runner.StopRunner += (run) =>
                {

                    Console.WriteLine(run.Container.GetLogs());

                    WorkflowRun[] runs = repo.GetWorkflows();

                    foreach (WorkflowRun workRun in runs)
                    {
                        if (workRun.ID == workflowRun.WorkflowRun.ID)
                        {
                            Console.WriteLine($"Workflow Run: {workRun.ID} Status: {workRun.Status}");

                            workRun.GetLogs();

                            WorkflowJob[] jobs = workRun.GetJobs();

                        }
                    }
                };

                count++;
            });

            webhookService.StartAsync();
        }

        static void TestEphemeralRunners(Repository repo, bool ephemeral)
        {
            WorkflowRun[] workflows = repo.GetWorkflows();

            foreach (WorkflowRun workflow in workflows)
            {
                if (workflow.Status != "queued")
                    continue;

                Console.WriteLine($"Starting Runner for Workflow: {workflow.ID}");

                RunnerBuilder builder = new RunnerBuilder($"GitHubAPIExperiments-{workflow.ID}", "mrdnalex/github-action-worker-container-dotnet", repo, ephemeral);

                builder.AddLabel($"run-{workflow.ID}");

                Runner runner = builder.Build();

                runner.Start();

                runner.WaitForBusy();

                if (ephemeral)
                    continue;
            }
        }

        static private void SpawnRunners(Repository repo)
        {
            List<Runner> runners = new List<Runner>();

            for (int i = 0; i < 3; i++)
            {
                RunnerBuilder builder = new RunnerBuilder($"GitHubAPIExperiments{i}", "mrdnalex/github-action-worker-container-dotnet", repo, true);

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
