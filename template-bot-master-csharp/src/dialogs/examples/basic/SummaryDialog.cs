using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Microsoft.Teams.TemplateBotCSharp.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    public class SummaryDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var messages = MessageStore.getMessages();
            var message = context.MakeMessage();
            message.Attachments.Add(GetHeroCard(messages));
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        private static Bot.Connector.Attachment GetHeroCard(List<string> msgs)
        {
            var txtmsg = msgs.ToString();
            var heroCard = new HeroCard
            {
                Title = "This is the title",
                Subtitle = "Some sub title",
                Text = txtmsg
            };

            return heroCard.ToAttachment();
        }
    }
}