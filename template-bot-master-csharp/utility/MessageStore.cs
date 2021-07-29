using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Teams.TemplateBotCSharp.utility
{
    public static class MessageStore
    {
        private static readonly List<string> messages = new List<string>();

        public static List<string> getMessages()
        {
            return messages;
        }

        public static void addMessages(string message) 
        {
            messages.Add(message);
        }
    }
}