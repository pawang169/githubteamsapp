using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TeamsHCLProject.Data;

namespace TeamsHCLProject.Dialogs
{
    [Serializable]
    public class PullRequestDialog : IDialog<object>
    {
        public string repository = null;
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Enter pull request to search");
            context.Wait(RequestDetail);
        }
        private async Task RequestDetail(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var reply = context.MakeMessage();
            var message = await argument;
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
            string data = client.Query(query3, new { headRefName = message.Text });
            RootPullRequest obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RootPullRequest>(data);
            if(obj.data.viewer.pullRequests.edges.Count == 0)
            {
                reply.Text = "No pull request found.";
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
                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(card.ToAttachment());
            }
            
         
            await context.PostAsync(reply);
            context.Done<object>(new object());
        }
    }
}