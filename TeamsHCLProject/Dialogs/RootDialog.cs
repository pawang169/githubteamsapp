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
            context.Wait(OptionResponse);
            // context.Wait(PerformRemedyOperation);

        }

        private async Task OptionResponse(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var reply = context.MakeMessage();
            var message = await argument;
            if (message.Text.Trim() == "Get Repository Detail")
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
                foreach (Node rep in obj.data.viewer.repositories.nodes)
                {
                    repo += i.ToString() + " " + rep.name + "</br>";
                    i++;
                }
                List<CardAction> button = new List<CardAction>();
                CardAction cardAction;
                foreach (Node rep in obj.data.viewer.repositories.nodes)
                {
                    cardAction = new CardAction(ActionTypes.ImBack, rep.name, value: rep.name);
                    button.Add(cardAction);


                }
                card.Title = "Below are your repositories";
                card.Subtitle = "To get detail click on it";
                card.Buttons = button;
                Attachment attachment = card.ToAttachment();
                reply.Attachments.Add(attachment);
                await context.PostAsync(reply);

                context.Wait(RepositoryDetail);
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

        private async Task RepositoryDetail(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            
            var reply = context.MakeMessage();
            var message = await argument;

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
            string data = client.Query(query3, new { owner = "poonam0025", name = message.Text });
            RepositoryRoot obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RepositoryRoot>(data);


            HeroCard card = new HeroCard
            {
                Title = message.Text,
                Text = "<b>Id : </b> " + obj.data + "</br>"
               + "<b>Homepage Url : </b> " + obj.data.repository.homepageUrl + "</br>"
               + "<b>Resource path : </b> " + obj.data.repository.resourcePath + "</br>"
               + "<b>IsPrivate : </b> " + obj.data.repository.isPrivate + "</br>"
               + "<b>CreatedAt : </b> " + obj.data.repository.createdAt + "</br>"
               + "<b>UpdatedAt : </b> " + obj.data.repository.updatedAt + "</br>"
               + "<b>Name with Owner : </b> " + obj.data.repository.nameWithOwner

            };
            reply.Attachments = new List<Attachment>();
            reply.Attachments.Add(card.ToAttachment());
            await context.PostAsync(reply);

            
        }

        public async Task ChildDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(null);

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