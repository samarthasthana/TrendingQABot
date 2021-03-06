using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Teams.TemplateBotCSharp.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    public class CaptureQuestionsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            string token = string.Empty;

            if (context.Activity.Type == "message") 
            {
                var currentMessage = context.Activity.AsMessageActivity().Text;
                QuestionHandler.AddMessages(currentMessage);
                QuestionHandler.ProcessNewMessage(currentMessage);
            }
            await context.PostAsync(context.MakeMessage());
            context.Done<object>(null);
        }
    }
}