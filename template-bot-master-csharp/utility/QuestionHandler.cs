using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.Teams.TemplateBotCSharp.utility
{
    public static class QuestionHandler
    {
        private static readonly List<string> messages = new List<string>();

        private static readonly List<string> specimen = new List<string>();

        private static readonly Dictionary<string, List<string>> buckets = new Dictionary<string, List<string>>();

        public static List<string> getMessages()
        {
            return messages;
        }

        public static void addMessages(string message) 
        {
            messages.Add(message);
        }

        public static void processNewMessage(string message)
        {
            //// make a call to the azure endpoint 
            //var score = new List<float>();
            //find the 


            //// 
        }

        private static async void callSimilarityEndpoint(string message)
        {
            var request = new SimilarityRequestObject( specimen, new List<string> { message});
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://sentencesimilarityapi-v4.azurewebsites.net/check_similar", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                }
            }
        
        }

        private class SimilarityRequestObject
        {
            public List<string> specimen { get; set; }
            public List<string> candidates { get; set;}

            public SimilarityRequestObject(List<string> specimen, List<string> candidates)
            {
                this.specimen = specimen;
                this.candidates = candidates;
            }
        
        }
    }
}