﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Microsoft.Teams.TemplateBotCSharp.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
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
            var messages = QuestionHandler.GetMessages();
            var message = context.MakeMessage();
            message.Attachments.Add(GetHeroCard(messages));
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        private static Bot.Connector.Attachment GetHeroCard(List<string> msgs)
        {
            var textmsgs = new StringBuilder();
            foreach(string msg in msgs)
            {
                textmsgs.AppendLine(msg);
            }
            var txtmsg = msgs.ToString();
            var heroCard = new HeroCard
            {
                Title = "This is the title",
                Subtitle = "Some sub title",
                Text = textmsgs.ToString()
            };

            return heroCard.ToAttachment();
        }
    }
}