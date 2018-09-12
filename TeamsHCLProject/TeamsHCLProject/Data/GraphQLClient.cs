using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace TeamsHCLProject.Data
{
    public class GraphQLClient
    {
        private class GraphQLQuery
        {
            public string query { get; set; }
            public object variables { get; set; }
        }

        public string Query(string query, object variables)
        {
            var fullQuery = new GraphQLQuery()
            {
                query = query,
                variables = variables,
            };

            string jsonContent = JsonConvert.SerializeObject(fullQuery);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            string token = System.Configuration.ConfigurationManager.AppSettings["Token"];
            request.AddHeader("Authorization", "Bearer " + token);
            //   int limits = 100;
            var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings["GitGraphqlUrl"]);
            request.AddParameter("graphql", jsonContent, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string json = response.Content;
            return json;

        }
    }
}