using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Connector.Teams;
using TeamsHCLProject.Data;
using AdaptiveCards;

namespace TeamsHCLProject
{
    /// <summary>
    /// Simple class that processes an activity and responds with with set of messaging extension results.
    /// </summary>
    public class MessagingExtension
    {
        private Activity activity;

        /// <summary>
        /// Used to generate image index.
        /// </summary>
        private Random random;

        public MessagingExtension(Activity activity)
        {
            this.activity = activity;

        }

        /// <summary>
        /// Helper method to generate a compose extension
        /// 
        /// Note that for this sample, we are returning generated positions for illustration purposes only.
        /// </summary>
        /// <returns></returns>
        public ComposeExtensionResponse CreateResponse()
        {
            try
            {
                ComposeExtensionResponse response = null;
                ComposeExtensionAttachment composeExtensionAttachment = new ComposeExtensionAttachment();
                var query = activity.GetComposeExtensionQueryData();
                var results = new ComposeExtensionResult()
                {
                    AttachmentLayout = "list",
                    Type = "result",
                    Attachments = new List<ComposeExtensionAttachment>(),
                };
                string text = "";
                //Check to make sure a query was actually made:
                if (query.CommandId == null || query.Parameters == null)
                {
                    return null;
                }
                else if (query.Parameters.Count > 0)
                {
                    // query.Parameters has the parameters sent by client

                    string headRefName = "";

                    if (query.CommandId == "PRs")
                    {
                        var titleParam = query.Parameters?.FirstOrDefault(p => p.Name == "PRs" || p.Name == "Repos" || p.Name == "Issues");
                        if (titleParam != null)
                        {
                            headRefName = titleParam.Value.ToString().ToLower();
                        }

                        var query3 = @"query($headRefName: String!) { 
                                      viewer { 
                                      pullRequests (first : 100, headRefName : $headRefName){
                                        edges {   
                                          node {
                                            id
                                            title
                                            body
                                            state
                                            headRefName
                                            revertUrl
                                            url
                                            repository {
                                                nameWithOwner
                                            }
                                            headRefName
                                          }
                                        }
                                      }
                                      }
                                    }";
                        var client = new GraphQLClient();
                        string data = client.Query(query3, new { headRefName = headRefName });
                        RootPullRequest obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RootPullRequest>(data);

                   
                        if (obj.data.viewer.pullRequests.edges.Count == 0)
                        {
                            text = "No  pull request exists.";
                        }
                        else
                        {
                           
                            HeroCard card = new HeroCard
                            {
                                Title = obj.data.viewer.pullRequests.edges[0].node.headRefName,
                                Text = "<b>Id         :</b>" + obj.data.viewer.pullRequests.edges[0].node.id + "</br>"
                                + "<b>Body       :</b>" + obj.data.viewer.pullRequests.edges[0].node.body + "</br>"
                                + "<b>State      :</b>" + obj.data.viewer.pullRequests.edges[0].node.state + "</br>"
                                + "<b>RevertUrl  :</b>" + obj.data.viewer.pullRequests.edges[0].node.revertUrl + "</br>"
                                + "<b>Repository :</b>" + obj.data.viewer.pullRequests.edges[0].node.repository.nameWithOwner,
                                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "More Info", value: obj.data.viewer.pullRequests.edges[0].node.url) }

                        };
                            composeExtensionAttachment = card.ToAttachment().ToComposeExtensionAttachment();
                        }

                        if (text != "")
                        {
                            results.Text = text;
                        }
                        else
                        {
                            results.Attachments.Add(composeExtensionAttachment);
                        }

                    }
                    else if (query.CommandId == "Repos")
                    {
                        string repository = "";
                        var titleParam = query.Parameters?.FirstOrDefault(p => p.Name == "PRs" || p.Name == "Repos" || p.Name == "Issues");
                        if (titleParam != null)
                        {
                            repository = titleParam.Value.ToString().ToLower();
                        }
                        var query3 = @"query($owner: String!,$name: String! ) {
                                                repository(owner: $owner, name: $name)
                                                {
                                                  id
                                                  name
                                                  homepageUrl
                                                  resourcePath
                                                  isPrivate
                                                  updatedAt
                                                  createdAt
                                                  nameWithOwner
                                                  url
                                                            }
                                                        }";
                        var client = new GraphQLClient();
                        string data = client.Query(query3, new { owner = "poonam0025", name = repository });
                        RepositoryRootObject obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RepositoryRootObject>(data);

                        if(obj.data.repository == null)
                        {
                            text = "No " + repository + "found.";
                        }
                        else
                        {
                            HeroCard card = new HeroCard
                            {
                                Title = repository,
                                Text = "<b>Id : </b> " + obj.data.repository.id + "</br>"
                                                    + "<b>Resource path : </b> " + obj.data.repository.resourcePath + "</br>"
                                                    + "<b>IsPrivate : </b> " + obj.data.repository.isPrivate + "</br>"
                                                    + "<b>CreatedAt : </b> " + Convert.ToDateTime(obj.data.repository.createdAt).ToString("dd MMM yyyy hh:mm:ss tt") + "</br>"
                                                    + "<b>UpdatedAt : </b> " + Convert.ToDateTime(obj.data.repository.updatedAt).ToString("dd MMM yyyy hh:mm:ss tt") + "</br>"
                                                    + "<b>Name with Owner : </b> " + obj.data.repository.nameWithOwner,
                                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "More Info", value: obj.data.repository.url) }

                            };

                            composeExtensionAttachment = card.ToAttachment().ToComposeExtensionAttachment();
                        }

                        if (text != "")
                        {
                            results.Text = text;
                        }
                        else
                        {
                            results.Attachments.Add(composeExtensionAttachment);
                        }

                    }

                    else if (query.CommandId == "Issues")
                    {
                      

                        string repository = "";
                        var titleParam = query.Parameters?.FirstOrDefault(p => p.Name == "PRs" || p.Name == "Repos" || p.Name == "Issues");
                        if (titleParam != null)
                        {
                            repository = titleParam.Value.ToString().ToLower();
                        }
                        var query3 = @"query($owner:String!,$name:String!) {
                                repository(owner : $owner, name: $name)
                                  {
                                    issues(first:20) { 
                                      edges { 
                                        node { 
                                          title 
                                          url 
                                          state
                                          body
                                          createdAt
                                        } 
                                      } 
                                    } 
                                  } 
                                }";
                        var client = new GraphQLClient();
                        string data = client.Query(query3, new { owner = "poonam0025", name = repository });
                        RepositoryDetailRoot repositorydata = Newtonsoft.Json.JsonConvert.DeserializeObject<RepositoryDetailRoot>(data);

                        if (repositorydata.data.repository.issues.edges.Count == 0)
                        {
                            text = "No issue found.";
                        }
                        else
                        {

                          
                            HeroCard card = new HeroCard();
                            for (int i = 0; i < repositorydata.data.repository.issues.edges.Count; i++)
                            {
                                card = new HeroCard
                                {
                                    Title = "<b>" +repositorydata.data.repository.issues.edges[i].node.title +"</b>",
                                    Text = "<b>Description     :</b>" + repositorydata.data.repository.issues.edges[i].node.body + "</br>"
                                           + "<b>Created At  :</b>" + Convert.ToDateTime(repositorydata.data.repository.issues.edges[i].node.createdAt).ToString("dd MMM yyyy hh:mm:ss tt") + "</br>"
                                           + "<b>State :</b>" + repositorydata.data.repository.issues.edges[i].node.state,
                                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "More Info", value: repositorydata.data.repository.issues.edges[i].node.url) }

                                };

                                composeExtensionAttachment = card.ToAttachment().ToComposeExtensionAttachment();
                                results.Attachments.Add(composeExtensionAttachment);

                            }

                        }

                    }

                    response = new ComposeExtensionResponse()
                    {
                        ComposeExtension = results
                    };
                }
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }


        }

    }
}