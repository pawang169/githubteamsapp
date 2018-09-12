using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using TeamsHCLProject.Data;
using System;
using AdaptiveCards;
using System.Collections.Generic;

namespace TeamsHCLProject
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// changing something
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
           
            if (activity.Type == ActivityTypes.Message)
            {
                if (activity.Value != null)
                {
                    return await PerformSubmit(activity);
                }
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());

            }
            else if (activity.Type == ActivityTypes.Invoke)
            {

               if (activity.IsComposeExtensionQuery())
                {
                    // Determine the response object to reply with
                    var invokeResponse = new MessagingExtension(activity).CreateResponse();

                    // Return the response
                    return Request.CreateResponse(HttpStatusCode.OK, invokeResponse);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private static async Task<HttpResponseMessage> PerformSubmit(Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            TeamsSubmit obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TeamsSubmit>(Convert.ToString(activity.Value));
            Activity replyActivity = activity.CreateReply();

            if (obj.Action == "GetIssueDetail")
            {
                replyActivity = await SearchData(obj, activity);
            }
            await connector.Conversations.ReplyToActivityAsync(replyActivity);
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        private static async Task<Activity> SearchData(TeamsSubmit obj, Activity activity)
        {
            Activity replyActivity = activity.CreateReply();
            if (obj.Repository == "--Select option--")
            {
                
                replyActivity.Text = "Please select Repository";
            }
            else
            {
                string data = "";
                var client = new GraphQLClient();
                if (   obj.State =="ALL")
                {
                    var query = @"query($owner:String!,$name:String!) {
                                repository(owner : $owner, name: $name)
                                  {
                                    issues(first:20) { 
                                      edges { 
                                        node { 
                                          title 
                                          url 
                                          state
        
                                        } 
                                      } 
                                    } 
                                  } 
                                }";
                    data = client.Query(query, new { owner = "poonam0025", name = obj.Repository });
                }
                else
                {
                    var query = @"query($owner:String!,$name:String!, $Issuestate:[IssueState!]) {
                                repository(owner : $owner, name: $name)
                                  {
                                    issues(first:20, states:$Issuestate) { 
                                      edges { 
                                        node { 
                                          title 
                                          createdAt
                                          body
                                          url 
                                          state
        
                                        } 
                                      } 
                                    } 
                                  } 
                                }";
                    data = client.Query(query, new { owner = "poonam0025", name = obj.Repository, states = obj.State });

                }
                RepositoryDetailRoot repositorydata = new RepositoryDetailRoot();
                try
                {
                    repositorydata = Newtonsoft.Json.JsonConvert.DeserializeObject<RepositoryDetailRoot>(data);
                }
                catch(Exception EX)
                {

                }

                if (repositorydata.data.repository.issues.edges.Count == 0)
                {
                    replyActivity.Text = "No issue found under this repository";
                }
                else
                {
                    HeroCard card = new HeroCard();
                    replyActivity.Attachments = new List<Attachment>();
                    replyActivity.Text = "Below are the issue detail of " + obj.Repository;
                    for (int i = 0; i < repositorydata.data.repository.issues.edges.Count; i++)
                    {
                        card = new HeroCard
                        {
                            Title = repositorydata.data.repository.issues.edges[i].node.title,
                            Text = "<b>Description     :</b>" + repositorydata.data.repository.issues.edges[i].node.body + "</br>"
                                   + "<b>Created At  :</b>" + Convert.ToDateTime(repositorydata.data.repository.issues.edges[i].node.createdAt).ToString("dd MMM yyyy hh:mm:ss tt") + "</br>"
                                   + "<b>State :</b>" + repositorydata.data.repository.issues.edges[i].node.state,
                            Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "More Info", value: repositorydata.data.repository.issues.edges[i].node.url) }

                        };

                        replyActivity.Attachments.Add(card.ToAttachment());

                    }

                }
                    }
            return replyActivity;
        }

       
        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}