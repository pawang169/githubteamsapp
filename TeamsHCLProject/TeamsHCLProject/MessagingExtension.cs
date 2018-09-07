using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Connector.Teams;
using TeamsHCLProject.Service;
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
                        var client = new GraphQLClient();
                        string data = client.Query(query3, new { headRefName = headRefName });
                        RootPullRequest obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RootPullRequest>(data);

                        //var card = GetUpdatedAdaptiveCard(obj);
                        //Attachment attachment = new Microsoft.Bot.Connector.Attachment()
                        //{
                        //    ContentType = AdaptiveCard.ContentType,
                        //    Content = card,

                        //};
                        if (obj.data.viewer.pullRequests.edges.Count == 0)
                        {
                            text = "No  pull request exists.";
                        }
                        else
                        {
                            HeroCard card = new HeroCard
                            {
                                Title = obj.data.viewer.pullRequests.edges[0].node.headRefName,
                                Text = "<b>Id : </b> " + obj.data.viewer.pullRequests.edges[0].node.id + "</br>"
                               + "<b>Body : </b> " + obj.data.viewer.pullRequests.edges[0].node.body + "</br>"
                               + "<b>State : </b> " + obj.data.viewer.pullRequests.edges[0].node.state + "</br>"
                             + "<b>RevertUrl : </b> " + obj.data.viewer.pullRequests.edges[0].node.revertUrl + "</br>"
                             + "<b>Url : </b> " + obj.data.viewer.pullRequests.edges[0].node.url + "</br>"
                             + "<b>Repository : </b> " + obj.data.viewer.pullRequests.edges[0].node.repository.nameWithOwner



                            };
                            composeExtensionAttachment = card.ToAttachment().ToComposeExtensionAttachment();
                        }
                       
                        // composeExtensionAttachment = attachment.ToComposeExtensionAttachment();

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
                                                            }
                                                        }";
                        var client = new GraphQLClient();
                        string data = client.Query(query3, new { owner = "poonam0025", name = repository });
                        RepositoryRoot obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RepositoryRoot>(data);

                        if(obj.data.repository == null)
                        {
                            text = "No " + repository + "found.";
                        }
                        else
                        {
                            HeroCard card = new HeroCard
                            {
                                Title = repository,
                                Text = "<b>Id : </b> " + obj.data + "</br>"
                                                    + "<b>Homepage Url : </b> " + obj.data.repository.homepageUrl + "</br>"
                                                    + "<b>Resource path : </b> " + obj.data.repository.resourcePath + "</br>"
                                                    + "<b>IsPrivate : </b> " + obj.data.repository.isPrivate + "</br>"
                                                    + "<b>CreatedAt : </b> " + obj.data.repository.createdAt + "</br>"
                                                    + "<b>UpdatedAt : </b> " + obj.data.repository.updatedAt + "</br>"
                                                    + "<b>Name with Owner : </b> " + obj.data.repository.nameWithOwner

                            };

                            composeExtensionAttachment = card.ToAttachment().ToComposeExtensionAttachment();
                        }
                     
                        //string repo = "";
                        //int i = 1;
                        //foreach (Node rep in obj.data.viewer.repositories.nodes)
                        //    {
                        //    repo += i.ToString() + " " + rep.name + "</br>";
                        //    i++;
                        //}

                        //card.Title = "User Repository detail";
                        //card.Subtitle = repo;



                    }

                    else if (query.CommandId == "Issues")
                    {


                        //   ComposeExtensionAttachment composeExtensionAttachment = card.ToAttachment().ToComposeExtensionAttachment();




                    }

                    // Generate cards for the response.



                    var results = new ComposeExtensionResult()
                    {
                        AttachmentLayout = "list",
                        Type = "result",
                        Attachments = new List<ComposeExtensionAttachment>(),
                    };
                    //   var card = CardHelper.CreatePatientCardForCE(sHeaderText, pos.Name, sPatientId, sWaitTime, sColor);
                    //   var previewCard = CardHelper.CreatePatientCardForCE("Team card", pos.Name, sPatientId, sWaitTime, sColor, false);

                    if(text != "")
                    {
                        results.Text = text;
                    }
                    else
                    {
                        results.Attachments.Add(composeExtensionAttachment);
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

        //private static AdaptiveCard GetUpdatedAdaptiveCard(RootPullRequest obj)
        //{

        //    List<AdaptiveElement> items = new List<AdaptiveElement>()
        //    {
        //     new AdaptiveColumnSet()
        //                     {

        //                         Columns = new List<AdaptiveColumn>()
        //                         {
        //                             new AdaptiveColumn()
        //                             {
        //                                 Items = new List<AdaptiveElement>()
        //                                 {
        //                                     new AdaptiveTextBlock(){Text="Title: " + obj.data.viewer.pullRequests.edges[0].node.headRefName, Color= AdaptiveTextColor.Dark,Wrap=true, Weight=AdaptiveTextWeight.Bolder,Size=AdaptiveTextSize.Large},
        //                                 }
        //                             }
        //                             // new AdaptiveColumn()
        //                             //{
        //                             //    Items = new List<AdaptiveElement>()
        //                             //    {
        //                             //        new AdaptiveTextBlock(){Text="Resolved",IsSubtle= true}
        //                             //    }
        //                             //}
        //                         }
        //                     }
        //    };
        //    items.Add(new AdaptiveTextBlock() { Text = "**Id:**             " + obj.data.viewer.pullRequests.edges[0].node.id , Wrap = true });
        //    items.Add(new AdaptiveTextBlock() { Text = "**Body:**          " + obj.data.viewer.pullRequests.edges[0].node.body, Wrap = true });
        //    items.Add(new AdaptiveTextBlock() { Text = "**State:** " + obj.data.viewer.pullRequests.edges[0].node.state, Wrap = true });
        //    items.Add(new AdaptiveTextBlock() { Text = "**RevertUrl:**         " + obj.data.viewer.pullRequests.edges[0].node.revertUrl, Wrap = true });
        //    items.Add(new AdaptiveTextBlock() { Text = "**Url:**     " + obj.data.viewer.pullRequests.edges[0].node.url, Wrap = true });
        //    items.Add(new AdaptiveTextBlock() { Text = "Repository:** " + obj.data.viewer.pullRequests.edges[0].node.repository.nameWithOwner, Wrap = true });
        //    var card = new AdaptiveCard()
        //    {
        //        Body = items

        //    };

        //    return card;
        //}
    }
}