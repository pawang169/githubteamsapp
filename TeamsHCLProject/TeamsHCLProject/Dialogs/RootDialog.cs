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
    public class RootDialog : LuisDialog<object>
    {
        //public async Task StartAsync(IDialogContext context)
        //{
        //    await context.PostAsync(GetOptionCard(context));
        //    context.Wait(OptionResponse);
        //}

        [LuisIntent("Hi")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(GetOptionCard(context));
            context.Wait(MessageReceived);
          //  context.Wait(OptionResponse);
            // context.Wait(PerformRemedyOperation);

        }

        [LuisIntent("PRs")]
        public async Task PRS(IDialogContext context, LuisResult result)
        {
            context.Call<object>(new PullRequestDialog(), ChildDialogIsDone);

        }

        [LuisIntent("Issues")]
        public async Task Issues(IDialogContext context, LuisResult result)
        {
            context.Call<object>(new SearchIssueDialog(), ChildDialogIsDone);

        }
        [LuisIntent("Repository")]
        public async Task Repository(IDialogContext context, LuisResult result)
        {
            context.Call<object>(new RepositoryDetail(), ChildDialogIsDone);

        }

        private async Task OptionResponse(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var reply = context.MakeMessage();
            var message = await argument;
            if (message.Text.Trim() == "Get Repository Detail")
            {
                context.Call<object>(new RepositoryDetail(), ChildDialogIsDone);
            }

            else if (message.Text.Trim() == "Search PRs")
            {
                context.Call<object>(new PullRequestDialog(), ChildDialogIsDone);
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
            context.Wait(MessageReceived);

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
                                               new CardAction(ActionTypes.ImBack, "Get Repository Detail", value: "Get Repository Detail")}
            };

            reply.Attachments = new List<Attachment>();
            reply.Attachments.Add(heroCard.ToAttachment());

            return reply;
        }
    }
}