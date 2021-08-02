using Microsoft.Bot.Builder.Dialogs;
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
            var res = QuestionHandler.GetSummaryContent();
            var message = context.MakeMessage();
            message.Attachments.Add(GetHeroCard(res.responses));
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        private static Bot.Connector.Attachment GetHeroCard(Dictionary<string, List<string>> msgs)
        {
            var textmsgs = new StringBuilder();
            foreach(var item in msgs)
            {
                textmsgs.AppendLine($"Question: {item.Value.First()}");
                textmsgs.AppendLine($"Keywords: {item.Key}");
                textmsgs.AppendLine($"Count: {item.Value.Count}");
                textmsgs.AppendLine(" ");
            }
            var heroCard = new HeroCard
            {
                Title = "Trending Q&A",
                Subtitle = "List of top questions, keywords and count",
                Text = textmsgs.ToString()
            };

            return heroCard.ToAttachment();
        }
    }
}