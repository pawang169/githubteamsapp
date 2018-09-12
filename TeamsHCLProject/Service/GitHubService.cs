using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using TeamsHCLProject.Data;

namespace TeamsHCLProject.Service
{
    public class GitHubService
    {
        public string ValidateToken(string token)
        {

            string status = null;
            var client = new RestClient("https://api.github.com/graphql");
            var request = new RestRequest(Method.POST);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddParameter("graphql", "{\"query\": \"{ viewer { login name id } }\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode.ToString() == "OK")
            {
            }
            else

            {

            }
            status = response.StatusCode.ToString();

            return status;

        }

    }
}