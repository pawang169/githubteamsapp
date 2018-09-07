using AdaptiveCards;
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
    public class RepositoryDetail : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();
            HeroCard card = new HeroCard();
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
            List<CardAction> button = new List<CardAction>();
            CardAction cardAction;
            foreach (Node1 rep in obj.data.viewer.repositories.nodes)
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
            context.Wait(RepositoryDetailData);
        }

        private async Task RepositoryDetailData(IDialogContext context, IAwaitable<IMessageActivity> argument)
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
               + "<b>Homepage Url : </b> " + obj.data.repository.url + "</br>"
               + "<b>Resource path : </b> " + obj.data.repository.resourcePath + "</br>"
               + "<b>IsPrivate : </b> " + obj.data.repository.isPrivate + "</br>"
               + "<b>CreatedAt : </b> " + obj.data.repository.createdAt + "</br>"
               + "<b>UpdatedAt : </b> " + obj.data.repository.updatedAt + "</br>"
               + "<b>Name with Owner : </b> " + obj.data.repository.nameWithOwner

            };
            reply.Attachments = new List<Attachment>();
            reply.Attachments.Add(card.ToAttachment());
            await context.PostAsync(reply);
            context.Done<object>(new object());


        }
    }
}