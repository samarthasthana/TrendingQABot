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

        private static Dictionary<string, string> specimenKeywordPair = new Dictionary<string, string>();

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
                return;
            }
            // Find the most matched value for each candidate and take action 
            foreach (var item in response.response)
            {
                double maxval = item.values.Max();
                var maxidx = item.values.IndexOf(maxval);
                if (maxval > 0.70)
                {
                    // Add to the bucket of that specimen if exists
                    var parentSpecimen = specimen.ElementAt(maxidx);
                    if (buckets.ContainsKey(parentSpecimen))
                    {
                        buckets.TryGetValue(parentSpecimen, out List<string> val);
                        val.Add(item.candidate);
                        buckets[parentSpecimen] = val;
                    }
                    else 
                    {
                        // Else create a new entry and add the first element to the array
                        buckets.Add(parentSpecimen, new List<string> { item.candidate });
                    }
                }
                else 
                {
                    // Add to the specimen list 
                    specimen.Add(item.candidate);
                }
            }
        }

        private static void FindKeywordsForBuckets()
        {
            foreach (var sp in specimen)
            {
                StringBuilder consolidatedText = new StringBuilder();
                consolidatedText.Append(sp);
                if (buckets.ContainsKey(sp))
                {
                    var seperator = " ";
                    buckets.TryGetValue(sp, out List<string> candidates);
                    foreach (var c in candidates)
                    {
                        consolidatedText.Append(seperator);
                        consolidatedText.Append(c);
                    }
                }
                KeywordAnalyzer analyzer = new KeywordAnalyzer();
                var analyzerResult = analyzer.Analyze(consolidatedText.ToString());
                List<Keyword> keywords = (from n in analyzerResult.Keywords select n).Take(2).ToList();
                var generatedKeyword = keywords.Count > 1 ? $"{keywords.First().Word}, {keywords.Last().Word}" : keywords.First().Word;
                specimenKeywordPair.Add(sp, generatedKeyword);
            }
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

        public class SummaryContent
        {
            public Dictionary<string, List<string>> responses;
            public SummaryContent()
            {
                responses = new Dictionary<string, List<string>>();
            }
        }

        public static SummaryContent GetSummaryContent()
        {
            var res = new SummaryContent();
            FindKeywordsForBuckets();
            foreach (var sp in specimen)
            {
                specimenKeywordPair.TryGetValue(sp, out string key);
                if (!String.IsNullOrWhiteSpace(key))
                {
                    buckets.TryGetValue(sp, out List<string> val);
                    if (val != null)
                    {
                        val.Insert(0, sp);
                    }
                    else 
                    {
                        val = new List<string>() { sp };
                    }
                    res.responses.Add(key, val);
                }
            }
            return res;
        }

        public class Response
        {
            public List<ResponseItem> response { get; set; }
        }

        public class ResponseItem
        {
            public string candidate { get; set; }
            public List<double> values { get; set; }
        }

        public class Request
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