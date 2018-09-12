﻿using Newtonsoft.Json;
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
            // public string OperationName { get; set; }
            public string query { get; set; }
            public object variables { get; set; }
        }
        public class GraphQLQueryResult
        {
            private string raw;
            private JObject data;
            private Exception Exception;
            public GraphQLQueryResult(string text, Exception ex = null)
            {
                Exception = ex;
                raw = text;
                data = text != null ? JObject.Parse(text) : null;
            }
            public Exception GetException()
            {
                return Exception;
            }
            public string GetRaw()
            {
                return raw;
            }
            public T Get<T>(string key)
            {
                if (data == null) return default(T);
                try
                {
                    return JsonConvert.DeserializeObject<T>(this.data["data"][key].ToString());
                }
                catch
                {
                    return default(T);
                }
            }
            public dynamic Get(string key)
            {
                if (data == null) return null;
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(this.data["data"][key].ToString());
                }
                catch
                {
                    return null;
                }
            }
            public dynamic GetData()
            {
                if (data == null) return null;
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(this.data["data"].ToString());
                }
                catch
                {
                    return null;
                }
            }
        }
        private string url;
 
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
            request.AddHeader("Authorization", "Bearer b3aa416f2aec93565215b6f3ce8f08cab1bb1f5c");
            //   int limits = 100;
            var client = new RestClient("https://api.github.com/graphql");

            request.AddParameter("graphql", jsonContent, ParameterType.RequestBody);
            //   request.AddParameter("graphql", "{\"query\": \"{ viewer {    name     repositories(last:" + limits + ") {       nodes {         name  }  }   } }\"}", ParameterType.RequestBody);
            //  request.AddParameter("graphql", "{\"query\": \"{ viewer { login name id } }\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            string json = response.Content;
            return json;
        //    return new GraphQLQueryResult(json);
          
          
        }
    }
}