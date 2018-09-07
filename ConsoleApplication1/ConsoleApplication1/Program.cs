using GraphQL;
using GraphQL.Common.Request;
using GraphQL.Types;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Executor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using GraphQL.Client;
using static ConsoleApplication1.GraphQLClient;

namespace ConsoleApplication1
{

   
    class Program
    {
        static void Main(string[] args)
        {
            var client1 = new GraphQLClient("https://api.github.com/graphql");
//            var query = @"
//    query($id: String) { 
//        someObject(id: $id) {
//            id
//            name
//        }
//    }
//";
            //var query1 = @"query($id: Int!) {
            //    viewer {
            //        name
            //       repositories(last: $id) {
            //            nodes {
            //                name
            //            }
            //        }
            //    }
            //}";

            var query = @"query  {
                    viewer{
                        name
                      repositories(first: 100)
                      {
                            nodes
      {
                                name
      }
                        }
                    }
                }";

            var query3 = @"query($headRefName: String!) { 
                                      viewer { 
                                      pullRequests (first : 100, headRefName : $headRefName){
                                        totalCount
                                        edges {   
                                          node {
                                            id
                                            title
                                            body
                                            state
                                            headRefName
                                            revertUrl
                                            url
                                            bodyText
                                            repository {
                                                id
                                                name
                                                nameWithOwner
                                                resourcePath  
                                                url
                                                owner{
                                                  __typename
                                                  resourcePath
                                                }
                                            }
                                         assignees(first:100)
                                            {
                                              totalCount
                                            }
                                            comments(first:100)
                                            {
                                              totalCount
                                            }
                                            headRef{
                                              name
                                            }
                                            headRefName
                                          }
                                        }
                                      }
                                      }
                                    }";


            string obj = client1.Query(query3, new { headRefName = "composeextension" });
         //   Console.WriteLine((string)obj.name);
            Console.ReadLine();
            //          
          //  string s = GetUserData().Result;

        }

        public static async Task<string> GetUserData()
        {


                   



            var client = new RestClient("https://api.github.com/graphql");

            var request = new RestRequest(Method.POST);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer 17c03b21ecc77b2a5328ebb1ba32165d47dee6d5");
            int limits = 100;
            string uname = "poonam0025";
            string comp = "HCLSample";
            request.AddParameter("graphql", "{\"query\": \"{ repository(owner: "+uname +", name: "+comp+") { 	issues    (last:20, state:OPEN) { edge node{ title url} }}   } }\"}", ParameterType.RequestBody);
            //   request.AddParameter("graphql", "{\"query\": \"{ viewer {    name     repositories(last:" + limits + ") {       nodes {         name  }  }   } }\"}", ParameterType.RequestBody);
            //  request.AddParameter("graphql", "{\"query\": \"{ viewer { login name id } }\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var json =  response.Content;
            if (response.StatusCode.ToString() == "OK")
            {
            }
            else

            {

            }

            string jsondata = JsonConvert.SerializeObject(response.Content);



            string token = "17c03b21ecc77b2a5328ebb1ba32165d47dee6d5";
            var myUri = new Uri("https://api.github.com/graphql");
            //   var myUri = new Uri("https://github.com/login/oauth/authorize?client_id=f477608c348cc0f72c37&redirect_uri=http://localhost:80/xyz&scope&state&allow_signup=false");
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var myWebRequest = WebRequest.Create(myUri);
            var myHttpWebRequest = (HttpWebRequest)myWebRequest;
            myHttpWebRequest.PreAuthenticate = true;
            myHttpWebRequest.ContentType = "application/json";
            myHttpWebRequest.Headers.Add("Authorization", "Bearer 17c03b21ecc77b2a5328ebb1ba32165d47dee6d5");
            myHttpWebRequest.Method = "POST";




          //  RootObject obj = new RootObject();
          //  obj.query = "query { viewer { login } }";
          ////  string jsondata = "{\"query\": \"{ viewer { login name id } }\"}";
          //    string jsondata = JsonConvert.SerializeObject(obj);

          //  try
          //  {
          //      using (var streamWriter = new StreamWriter(myHttpWebRequest.GetRequestStream()))
          //      {
          //          streamWriter.Write(jsondata);
          //          streamWriter.Flush();
          //          streamWriter.Close();
          //      }

          //      var myWebResponse = await myWebRequest.GetResponseAsync();
          //      var responseStream = myWebResponse.GetResponseStream();
          //  }
          //  catch (Exception ex)
          //  {

          //  }


            //var paymentServicePostClient = new HttpClient();

            //var requestObj = obj;
            //var formcontent = JsonConvert.SerializeObject(requestObj);
            //var content = new StringContent(formcontent, Encoding.UTF8, "application/json");
            //var serviceurl = myUri;
            //try
            //{
            //    using (var apiclient = new HttpClient())
            //    {
            //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //        apiclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //        var apiResponse = await apiclient.PostAsync(serviceurl, content).ConfigureAwait(false);
            //        if (!apiResponse.IsSuccessStatusCode)
            //        {
            //            throw new Exception("error occured");

            //        }
            //        return Convert.ToString(apiResponse.Content.ReadAsStringAsync().Result);
            //    }
            //}
            //catch (Exception ex)
            //{

            //}

            return "";


   

        }
    }
}
