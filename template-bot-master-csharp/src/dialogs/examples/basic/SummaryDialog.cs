using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Microsoft.Teams.TemplateBotCSharp.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            message.Attachments.Add(GetHeroCard(res.responses, res.totalCount));
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        private static Bot.Connector.Attachment GetHeroCard(Dictionary<string, List<string>> msgs, int totalCount)
        {
            CultureInfo ci = new CultureInfo("en-us");
            var textmsgs = new StringBuilder();
            foreach(var item in msgs)
            {
                var percentage = (float)item.Value.Count / totalCount;
                textmsgs.AppendLine($"Question: {item.Value.First()}");
                textmsgs.AppendLine($"Keywords: {item.Key}");
                textmsgs.AppendLine($"% Occurrence: {percentage.ToString("P", ci)}");
                textmsgs.AppendLine(" ");
            }
            var heroCard = new HeroCard
            {
                Title = "Trending Q&A",
                Subtitle = "List of top questions, keywords and % occurrence",
                Text = textmsgs.ToString()
            };

            return heroCard.ToAttachment();
        }
    }
}