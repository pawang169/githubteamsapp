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
            List<AdaptiveChoice> choiceInput = new List<AdaptiveChoice>();

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
            choiceInput.Add(new AdaptiveChoice { Title = "--Select option--", Value = "--Select option--" });
            foreach (Node rep in obj.data.viewer.repositories.nodes)
            {
                choiceInput.Add(new AdaptiveChoice { Title = rep.name, Value = rep.name });
            }

            AdaptiveCard card = new AdaptiveCard()
            {
                Body = new List<AdaptiveElement>()
                {
                             new AdaptiveTextBlock(){Text="Search Issue",Weight=AdaptiveTextWeight.Bolder,Size=AdaptiveTextSize.Large,Wrap=true, HorizontalAlignment = AdaptiveHorizontalAlignment.Center},
                             new AdaptiveTextBlock(){Text="Select Repository" },
                             new AdaptiveChoiceSetInput(){ Id = "Repository", Style = AdaptiveChoiceInputStyle.Compact,
                                                    Choices = choiceInput },
                             new AdaptiveTextBlock(){Text="Enter Issue Title" },
                new AdaptiveTextInput(){Id = "IssueTitle", Placeholder ="Enter issue title" }
                },






                Actions = new List<AdaptiveAction>()
                        {
                            new AdaptiveSubmitAction()
                            {
                            Title = "Update",
                            Id = "IssueDetail",
                            DataJson =  @"{""Action"":""GetIssueDetail""}"
                            }
                        }
            };
            Microsoft.Bot.Connector.Attachment attachment = new Microsoft.Bot.Connector.Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
                Name = "ABCD"
            };

            reply.Attachments.Add(attachment);

            await context.PostAsync(reply);
        }

        private async Task SelectState(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text.Trim().ToLower() == "exit")
            {
                await context.PostAsync(Constants.AppExit);
                context.Done<object>(new object());
            }
            else
            {
                context.Done<object>(new object());
            }
            //else
            //{
            //    repository = message.Text;
            //    await context.PostAsync(GetState(context));
            //    context.Wait(GetIssue);
            //}
        }

        //private async Task GetIssue(IDialogContext context, IAwaitable<IMessageActivity> argument)

        //{
        //    var message = await argument;
        //    if (message.Text.Trim().ToLower() == "exit")
        //    {
        //        await context.PostAsync(Constants.AppExit);
        //        context.Done<object>(new object());
        //    }
        //    else

        //        if (message.Text == "OPEN" || message.Text == "CLOSED" || message.Text == "ALL")
        //        {
        //        var client = new GraphQLClient();
        //        string data = "";
        //        if (message.Text == "ALL")
        //        {
        //var query = @"query(owner:String!,$name:String!) {
        //                        repository(owner : $episode, name: $name)
        //                          {
        //                            issues(first:20) { 
        //                              edges { 
        //                                node { 
        //                                  title 
        //                                  url 
  
        //                                  state
        
        //                                } 
        //                              } 
        //                            } 
        //                          } 
        //                        }";
        //             data = client.Query(query, new { owner = "poonam0025", name = repository });
        //        }
        //        else
        //        {
        //            var query = @"query($owner:String!,$name:String!, $Issuestate:[IssueState!]) {
        //                        repository(owner : $owner, name: $name)
        //                          {
        //                            issues(first:20, states:$Issuestate) { 
        //                              edges { 
        //                                node { 
        //                                  title 
        //                                  url 
        //                                  state

        //                                } 
        //                              } 
        //                            } 
        //                          } 
        //                        }";
        //             data = client.Query(query, new { owner = "poonam0025", name = repository, states = message.Text});

        //        }
        //        RepositoryRoot obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RepositoryRoot>(data);
        //        AdaptiveCard card = SearchAdaptiveCard(obj, message.Text, repository);
        //        Microsoft.Bot.Connector.Attachment attachment = new Microsoft.Bot.Connector.Attachment()
        //        {
        //            ContentType = AdaptiveCard.ContentType,
        //            Content = card,
        //            Name = "ABCD"
        //        };

        //        var reply = context.MakeMessage();
        //        reply.Attachments.Add(attachment);

        //        await context.PostAsync(reply);
        //        context.Done<object>(new object());

        //    }

        //}

        //private static AdaptiveCard SearchAdaptiveCard(RepositoryRoot obj, string search, string repository)
        //{
        //    List<AdaptiveFact> fact = new List<AdaptiveFact>();
        //    AdaptiveFact factObj;
        //    string reportTitle = "Repository" + repository.ToUpper() + "issues detail";


        //        factObj = new AdaptiveFact();
        //        factObj.Value = search;
        //        factObj.Title = "State:";
        //        fact.Add(factObj);

        //    List<AdaptiveElement> bodyAdaptiveElement = new List<AdaptiveElement>() {
        //         new AdaptiveTextBlock() { Text = reportTitle, HorizontalAlignment = AdaptiveHorizontalAlignment.Center, Weight = AdaptiveTextWeight.Bolder, Size = AdaptiveTextSize.Medium },
        //          new AdaptiveFactSet()
        //          {
        //              Facts = fact
        //          },
        //           new AdaptiveColumnSet()
        //    ,
        //            new AdaptiveColumnSet()
        //           {
        //               Spacing = AdaptiveSpacing.Medium,
        //               Separator = true,
        //               Columns = new List<AdaptiveColumn>()
        //                        {
        //                               new AdaptiveColumn()
        //                             {
        //                                   Width = "10",
        //                                 Items = new List<AdaptiveElement>()
        //                                 {
        //                                    new AdaptiveTextBlock { Text = "Title", Weight = AdaptiveTextWeight.Bolder, IsSubtle = true
        //                                 }

        //                             }
        //                             },
        //                             new AdaptiveColumn()
        //                             {
        //                                  Width = "42",
        //                                                   Items = new List<AdaptiveElement>()
        //                                 {
        //                                    new AdaptiveTextBlock { Text = "Url", Weight = AdaptiveTextWeight.Bolder, IsSubtle = true
        //                                    }

        //                                 }
        //                             },
        //                             new AdaptiveColumn()
        //                             {
        //                                  Width = "9",
        //                                                   Items = new List<AdaptiveElement>()
        //                                 {
        //                                    new AdaptiveTextBlock { Text = "State", Wrap = true, Weight = AdaptiveTextWeight.Bolder, IsSubtle = true
        //                                    }
        //                                 }
        //                            }
        //                         }
        //           }

        //        };

        //    for (int i = 0; i < obj.data.repository.issues.edges[0].node.Count; i++)
        //    {
        //        bodyAdaptiveElement.Add(
        //            new AdaptiveColumnSet()
        //            {
        //                Spacing = AdaptiveSpacing.Medium,
        //                Separator = true,
        //                Columns = new List<AdaptiveColumn>()
        //                         {
        //                               new AdaptiveColumn()
        //                             {
        //                                   Width = "10",
        //                                       Items = new List<AdaptiveElement>()
        //                                 {
        //                                    new AdaptiveTextBlock { Text = obj.data.repository.issues.edges[0].node[i].title, Wrap = true, Weight = AdaptiveTextWeight.Default, IsSubtle = true
        //                                    }
        //                                 }
        //                             },
        //                             new AdaptiveColumn()
        //                             {
        //                                  Width = "42",
        //                                       Items = new List<AdaptiveElement>()
        //                                 {
        //                                    new AdaptiveTextBlock { Text =  obj.data.repository.issues.edges[0].node[i].url, Wrap = true, Weight = AdaptiveTextWeight.Default, IsSubtle = true
        //                                    }
        //                                 }
        //                             },
        //                             new AdaptiveColumn()
        //                             {
        //                                  Width = "9",
        //                                       Items = new List<AdaptiveElement>()
        //                                 {
        //                                    new AdaptiveTextBlock { Text =  obj.data.repository.issues.edges[0].node[i].state, Wrap = true, Weight = AdaptiveTextWeight.Default, IsSubtle = true
        //                                    }
        //                                 }
        //                             }
        //                         }
        //            });
        //    }

        //    AdaptiveCard card = new AdaptiveCard()
        //    {
        //        Body = bodyAdaptiveElement
        //    };

        //    return card;

        //}



        //private IMessageActivity GetState(IDialogContext context)
        //{
        //    var reply = context.MakeMessage();

        //    var heroCard = new HeroCard
        //    {
        //        Text = "Please select issue state",
        //        Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Open", value: "OPEN"),
        //                                          new CardAction(ActionTypes.ImBack, "Closed", value: "CLOSED"),
        //                                new CardAction(ActionTypes.ImBack, "All", value: "ALL")}
        //    };

        //    reply.Attachments = new List<Attachment>();
        //    reply.Attachments.Add(heroCard.ToAttachment());

        //    return reply;
        //}


    }
}