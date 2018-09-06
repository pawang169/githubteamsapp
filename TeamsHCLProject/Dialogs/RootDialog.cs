using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using TeamsHCLProject.Service;
using System.Collections.Generic;
using TeamsHCLProject.Data;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace TeamsHCLProject.Dialogs
{
    [LuisModel("fde55290-f681-4dc4-9e96-186e92cb89cf", "9566f27b21b04962a90bd7f7a8ba6e15")]
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(GetOptionCard(context));
            context.Wait(OptionResponse);
        }

        [LuisIntent("Hi")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(GetOptionCard(context));
            context.Wait(OptionResponse);
            // context.Wait(PerformRemedyOperation);

        }

        private async Task OptionResponse(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var reply = context.MakeMessage();
            var message = await argument;
            if (message.Text.Trim() == "Get Repository")
            {
                HeroCard card = new HeroCard();
                GitHubService service = new GitHubService();
                //Repository obj = new Repository();
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
                var client = new GraphQLClient();
                string data = client.Query(query, null);
                AllRepository obj = Newtonsoft.Json.JsonConvert.DeserializeObject<AllRepository>(data);
                string repo = "";
                int i = 1;
                foreach (Node1 rep in obj.data.viewer.repositories.nodes)
                {
                    repo += i.ToString() + " " + rep.name + "</br>";
                    i++;
                }

                card.Title = "User Repository detail";
                card.Subtitle = repo;
                Attachment attachment = card.ToAttachment();
                reply.Attachments.Add(attachment);
                await context.PostAsync(reply);
               
                context.Done<object>(new object());
            }

            else if(message.Text.Trim() == "Search PRs")
            {
            }
            else if (message.Text.Trim() == "Search issues")
            {
                context.Call<object>(new SearchIssueDialog(), ChildDialogIsDone);
            }
            else
            {
                await context.PostAsync(GetOptionCard(context));
                context.Wait(OptionResponse);
            }

        }

        public async Task ChildDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(new object());

        }

        private IMessageActivity GetOptionCard(IDialogContext context)
        {

            var reply = context.MakeMessage();

            var heroCard = new HeroCard
            {
                Title = ("Hi "),
                Text = "Please select one option",

                Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Search issues", value: "Search issues"),
                                               new CardAction(ActionTypes.ImBack, "Search PRs", value: "Search PRs"),
                                               new CardAction(ActionTypes.ImBack, "Get Repository", value: "Get Repository")}
            };

            reply.Attachments = new List<Attachment>();
            reply.Attachments.Add(heroCard.ToAttachment());

            return reply;
        }
    }
}