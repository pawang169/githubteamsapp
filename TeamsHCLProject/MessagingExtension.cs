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

                var query = activity.GetComposeExtensionQueryData();

                //Check to make sure a query was actually made:
                if (query.CommandId == null || query.Parameters == null)
                {
                    return null;
                }
                else if (query.Parameters.Count > 0)
                {
                    // query.Parameters has the parameters sent by client

                    HeroCard card = new HeroCard();
                    if (query.CommandId == "PRs")
                    {


                        card.Title = "Currenty no PR record";
                     
                        
                   
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
                    issues(first: 20, states: OPEN) {
                        edges {
                            node {
                                title
                                url
                              closed
                            }
                        }
                    }
                }
            }";
                        var client = new GraphQLClient();
                        string data = client.Query(query3, new { owner = "poonam0025", name = repository });
                        RepositoryRoot obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RepositoryRoot>(data);


                        card = new HeroCard
                        {
                           Text = "Title : " + obj.data.repository.issues.edges[0].node[0].title,
                           Subtitle = "Closed Status : "+ obj.data.repository.issues.edges[0].node[0].state.ToString(),
                           Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Url", value: obj.data.repository.issues.edges[0].node[0].url)
                                            }

                        };
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


                    card.Title = "Currenty no Issue record";




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
                    var composeExtensionAttachment = card.ToAttachment().ToComposeExtensionAttachment();
                    results.Attachments.Add(composeExtensionAttachment);

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