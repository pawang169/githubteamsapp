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
    public class AuthenticateDialog : IDialog<object>
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
            if (status == "OK")
            {
                Constants.PATToken = message.Text;
                await context.PostAsync("Token saved.");
                context.Done<object>(new object());
            }
            else
            {
                await context.PostAsync("Token is not correct.");
                await context.PostAsync("Please enter correct token.");
                context.Wait(PATAuthoriztion);
                context.Done<object>(new object());
            }
         

        }
    }
}