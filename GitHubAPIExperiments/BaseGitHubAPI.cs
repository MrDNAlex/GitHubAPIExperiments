using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAPIExperiments
{
    public class BaseGitHubAPI
    {
        public static HttpClient GetClient (string githubPAT)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", $"token {githubPAT}");
            client.DefaultRequestHeaders.Add("User-Agent", "CSharp-App");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");

            return client;
        }







    }
}
