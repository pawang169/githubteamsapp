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
                             new AdaptiveChoiceSetInput(){ Id = "Repository", Style = AdaptiveChoiceInputStyle.Compact,
                                                    Choices = choice },
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
                Name = "ABCD"
            };

            reply.Attachments.Add(attachment);

            await context.PostAsync(reply);
            context.Done<object>(new object());
        }

        private static AdaptiveCard SearchAdaptiveCard(RepositoryRoot obj, string search, string repository)
        {
            List<AdaptiveFact> fact = new List<AdaptiveFact>();
            AdaptiveFact factObj;
            string reportTitle = "Repository" + repository.ToUpper() + "issues detail";
         
          
                factObj = new AdaptiveFact();
                factObj.Value = search;
                factObj.Title = "State:";
                fact.Add(factObj);
          
            List<AdaptiveElement> bodyAdaptiveElement = new List<AdaptiveElement>() {
                 new AdaptiveTextBlock() { Text = reportTitle, HorizontalAlignment = AdaptiveHorizontalAlignment.Center, Weight = AdaptiveTextWeight.Bolder, Size = AdaptiveTextSize.Medium },
                  new AdaptiveFactSet()
                  {
                      Facts = fact
                  },
                   new AdaptiveColumnSet()
            ,
                    new AdaptiveColumnSet()
                   {
                       Spacing = AdaptiveSpacing.Medium,
                       Separator = true,
                       Columns = new List<AdaptiveColumn>()
                                {
                                       new AdaptiveColumn()
                                     {
                                           Width = "10",
                                         Items = new List<AdaptiveElement>()
                                         {
                                            new AdaptiveTextBlock { Text = "Title", Weight = AdaptiveTextWeight.Bolder, IsSubtle = true
                                         }

                                     }
                                     },
                                     new AdaptiveColumn()
                                     {
                                          Width = "42",
                                                           Items = new List<AdaptiveElement>()
                                         {
                                            new AdaptiveTextBlock { Text = "Url", Weight = AdaptiveTextWeight.Bolder, IsSubtle = true
                                            }

                                         }
                                     },
                                     new AdaptiveColumn()
                                     {
                                          Width = "9",
                                                           Items = new List<AdaptiveElement>()
                                         {
                                            new AdaptiveTextBlock { Text = "State", Wrap = true, Weight = AdaptiveTextWeight.Bolder, IsSubtle = true
                                            }
                                         }
                                    }
                                 }
                   }

                };

            for (int i = 0; i < obj.data.repository.issues.edges[0].node.Count; i++)
            {
                bodyAdaptiveElement.Add(
                    new AdaptiveColumnSet()
                    {
                        Spacing = AdaptiveSpacing.Medium,
                        Separator = true,
                        Columns = new List<AdaptiveColumn>()
                                 {
                                       new AdaptiveColumn()
                                     {
                                           Width = "10",
                                               Items = new List<AdaptiveElement>()
                                         {
                                            new AdaptiveTextBlock { Text = obj.data.repository.issues.edges[0].node[i].title, Wrap = true, Weight = AdaptiveTextWeight.Default, IsSubtle = true
                                            }
                                         }
                                     },
                                     new AdaptiveColumn()
                                     {
                                          Width = "42",
                                               Items = new List<AdaptiveElement>()
                                         {
                                            new AdaptiveTextBlock { Text =  obj.data.repository.issues.edges[0].node[i].url, Wrap = true, Weight = AdaptiveTextWeight.Default, IsSubtle = true
                                            }
                                         }
                                     },
                                     new AdaptiveColumn()
                                     {
                                          Width = "9",
                                               Items = new List<AdaptiveElement>()
                                         {
                                            new AdaptiveTextBlock { Text =  obj.data.repository.issues.edges[0].node[i].state, Wrap = true, Weight = AdaptiveTextWeight.Default, IsSubtle = true
                                            }
                                         }
                                     }
                                 }
                    });
            }

            AdaptiveCard card = new AdaptiveCard()
            {
                Body = bodyAdaptiveElement
            };

            return card;

        }






    }
}