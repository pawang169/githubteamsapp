using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

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
            Constants.PATToken = message.Text;
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
    }
}