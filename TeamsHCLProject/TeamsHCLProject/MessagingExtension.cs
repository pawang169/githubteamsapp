using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Connector.Teams;

namespace TeamsHCLProject
{
    /// <summary>
    /// Simple class that processes an activity and responds with with set of messaging extension results.
    /// </summary>
    public class MessagingExtension
    {
        private Activity activity;

        /// <summary>
        /// Used to generate image index.
        /// </summary>
        private Random random;

        public MessagingExtension(Activity activity)
        {
            this.activity = activity;

        }

        /// <summary>
        /// Helper method to generate a compose extension
        /// 
        /// Note that for this sample, we are returning generated positions for illustration purposes only.
        /// </summary>
        /// <returns></returns>
        public ComposeExtensionResponse CreateResponse()
        {
            try
            {
                ComposeExtensionResponse response = null;

                var query = activity.GetComposeExtensionQueryData();

                //Check to make sure a query was actually made:
                if (query.CommandId == null || query.Parameters == null)
                {
                    return null;
                }
                else if (query.Parameters.Count > 0)
                {
                    // query.Parameters has the parameters sent by client

                    HeroCard card = new HeroCard();
                    if (query.CommandId == "PRs")
                    {


                        card.Title = "Currenty no PR record";
                     
                        
                   
                    }
                    else if (query.CommandId == "Repos")
                    {


                    card.Title = "Currenty no Repos found.";

                       

                    }

                   else if (query.CommandId == "Issues")
                    {


                    card.Title = "Currenty no issues found.";

                   

                    }

                    // Generate cards for the response.



                    var results = new ComposeExtensionResult()
                    {
                        AttachmentLayout = "list",
                        Type = "result",
                        Attachments = new List<ComposeExtensionAttachment>(),
                    };
                    //   var card = CardHelper.CreatePatientCardForCE(sHeaderText, pos.Name, sPatientId, sWaitTime, sColor);
                  //   var previewCard = CardHelper.CreatePatientCardForCE("Team card", pos.Name, sPatientId, sWaitTime, sColor, false);
                    var composeExtensionAttachment = card.ToAttachment().ToComposeExtensionAttachment();
                    results.Attachments.Add(composeExtensionAttachment);

                    response = new ComposeExtensionResponse()
                    {
                        ComposeExtension = results
                    };
                }
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }


        }
    }
}