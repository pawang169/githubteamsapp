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
    public class SearchIssueDialog : IDialog<object>
    {

        public string repository = null;
        public async Task StartAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();
            List<AdaptiveChoice> choice = new List<AdaptiveChoice>();
            choice.Add(new AdaptiveChoice() { Title = "--Select option--", Value = "--Select option--" });
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
   
            foreach (Node1 rep in obj.data.viewer.repositories.nodes)
            {
                choice.Add(new AdaptiveChoice() { Title = rep.name, Value = rep.name });
            }
            AdaptiveCard card = new AdaptiveCard()
            {
                Body = new List<AdaptiveElement>()
                {
                             new AdaptiveTextBlock(){Text="Search Issue",Weight=AdaptiveTextWeight.Bolder,Size=AdaptiveTextSize.Large,Wrap=true, HorizontalAlignment = AdaptiveHorizontalAlignment.Center},
                             new AdaptiveTextBlock(){Text="Select Repository" },
                             new AdaptiveChoiceSetInput(){ Id = "Repository", Style = AdaptiveChoiceInputStyle.Compact, Choices = choice },
                             new AdaptiveTextBlock(){Text="Select Issue State" },
                             new AdaptiveChoiceSetInput(){ Id = "State", Value = "--ALL--", IsMultiSelect = false, Style = AdaptiveChoiceInputStyle.Compact,
                                        Choices = new List<AdaptiveChoice>()
                                        {
                                            new AdaptiveChoice() { Title = "--ALL--", Value = "--ALL--" },
                                            new AdaptiveChoice()
                                            {
                                                Title = "OPEN",
                                                Value = "OPEN"

                                            },
                                                new AdaptiveChoice()
                                            {
                                                Title = "CLOSED",
                                                Value = "CLOSED"

                                            }
                                        }
                                     }
                   },
                Actions = new List<AdaptiveAction>()
                        {
                            new AdaptiveSubmitAction()
                            {
                            Title = "Submit",
                            Id = "IssueDetail",
                            DataJson =  @"{""Action"":""GetIssueDetail""}"
                            }
                        }
            };
            Microsoft.Bot.Connector.Attachment attachment = new Microsoft.Bot.Connector.Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
              //  Name = "ABCD"
            };

            reply.Attachments.Add(attachment);
            await context.PostAsync(reply);
            context.Done<object>(new object());
        }

    }
}