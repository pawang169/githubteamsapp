using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using TeamsHCLProject.Service;
using System.Collections.Generic;
using TeamsHCLProject.Data;

namespace TeamsHCLProject.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Please enter GitHub PAT");
            context.Wait(PATAuthoriztion);
        }

        private async Task PATAuthoriztion(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            GitHubService service = new GitHubService();
            string status = service.ValidateToken(message.Text);
            if(status == "OK")
            {
                Constants.PATToken = message.Text;
                await context.PostAsync("Token saved.");
                await context.PostAsync(GetOptionCard(context));
                context.Wait(OptionResponse);

            }
            else
            {
                await context.PostAsync("Token is not correct.");
            }
            //if (message.Text.Trim().ToLower() == "exit")
            //{
            //    await context.PostAsync(Constants.AppExit);
            //    context.Done<object>(new object());
            //}
            //else
            //{
            //    summary = message.Text;
            //    await context.PostAsync(GetRemedyPriority(context));
            //    context.Wait(CreateRemedy);
            //}

        }

        private async Task OptionResponse(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var reply = context.MakeMessage();
            var message = await argument;
            if (message.Text.Trim() == "Get Repository")
            {
                HeroCard card = new HeroCard();
                GitHubService service = new GitHubService();
                Repository obj = new Repository();
                obj = service.GetRepository();

                string repo = "";
                int i = 1;
                foreach (Node rep in obj.data.viewer.repositories.nodes)
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
            else
            {
                context.Done<object>(new object());
            }

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