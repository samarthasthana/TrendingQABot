using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Teams.TemplateBotCSharp.utility
{
    public static class QuestionHandler
    {
        private static List<string> messages = new List<string>();

        private static List<string> specimen = new List<string>();

        private static Dictionary<string, List<string>> buckets = new Dictionary<string, List<string>>();

        public static List<string> GetMessages()
        {
            return messages;
        }

        public static void AddMessages(string message) 
        {
            messages.Add(message);
        }

        public static async void ProcessNewMessage(string message)
        {
            Response response;
            if (specimen.Count > 0)
            {
                response = await CallSimilarityEndpoint(message).ConfigureAwait(false);
            }
            else 
            {
                // Directly add the question to specimen list as its empty
                specimen.Add(message);
            }

            // find the most matched value 
            // if the most matched value is less than 0.5 then add to specimen 
            // else add the value to matched specimens dictionary array 
            //
        }

        private static async Task<Response> CallSimilarityEndpoint(string message)
        {
            var candidate = new List<string>
            {
                message
            };
            Response responseObj = null;
            var request = new Request( specimen, candidate);
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://sentencesimilarityapi-v4.azurewebsites.net/check_similar", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    responseObj = JsonConvert.DeserializeObject<Response>(apiResponse);
                }
            }
            return responseObj;
        }

        private class Response
        {
            public List<ResponseItem> response { get; set; }
        }

        private class ResponseItem
        {
            public string candidate { get; set; }
            public List<double> values { get; set; }
        }

        private class Request
        {
            public List<string> specimen { get; set; }
            public List<string> candidates { get; set;}

            public Request(List<string> specimen, List<string> candidates)
            {
                this.specimen = specimen;
                this.candidates = candidates;
            }
        
        }
    }
}