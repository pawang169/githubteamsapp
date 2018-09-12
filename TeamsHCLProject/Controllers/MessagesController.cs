using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using TeamsHCLProject.Data;
using System;

namespace TeamsHCLProject
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it sdlfjldsjf;lds jfsd f lkjldsjfs;ldfj;lsajfd
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
            }
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