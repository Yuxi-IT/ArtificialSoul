using ArtificialSoul.Memory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialSoul.Cognition
{
    public class MetaCognitive
    {
        private readonly RestClient client;
        private readonly string modelName;
        private readonly string roleDescription;

        public MetaCognitive(string apiKey, string modelName, string roleDescription)
        {
            client = new RestClient("https://api.chatanywhere.tech/v1/chat/completions");
            client.AddDefaultHeader("Authorization", $"Bearer {apiKey}");
            client.AddDefaultHeader("Content-Type", "application/json");

            this.modelName = modelName;
            this.roleDescription = roleDescription;
        }

        public string GenerateResponse(string input, List<DialogueEntry> context = null)
        {
            var request = new RestRequest { Method = Method.Post };

            var messages = new List<object> { new { role = "system", content = roleDescription } };

            if (context != null && context.Count > 0)
            {
                foreach (var entry in context)
                {
                    messages.Add(new { role = "user", content = entry.UserInput });
                    messages.Add(new { role = "assistant", content = entry.ModelCOutput });
                }
            }

            messages.Add(new { role = "user", content = input });

            var requestBody = new { model = modelName, messages };
            string jsonBody = JsonConvert.SerializeObject(requestBody);
            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            RestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return jsonResponse.choices[0].message.content.ToString();
            }

            return $"Error: {response.StatusCode} - {response.ErrorMessage}";
        }
    }
}
