using Azure.AI.OpenAI;
using Bens.AI.Toolkit.Models.Mistral;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bens.AI.Toolkit
{
    public class MistralClient
    {
        public static string MistralUrl { get; set; } = string.Empty;
        public static string MistralCredentials { get; set; } = string.Empty;

        public async Task<string> Ask(string question, string instructions = "You are an assistant.", int max_tokens = 500, decimal temperature = 0.9M)
        {
            var data = new MistralRequest()
            {
                messages = new List<MistralMessage>() {
                    new MistralMessage(){
                        role = "system",
                        content = instructions,
                    },
                    new MistralMessage(){
                        role = "user",
                        content = question,
                    }
                },
                max_tokens = max_tokens,
                temperature = temperature,
            };

            var url = MistralUrl + ".inference.ai.azure.com/v1/chat/completions";
            var key = MistralCredentials;

            string json = JsonConvert.SerializeObject(data);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(key);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await client.PostAsync(url, content);

            if(res.IsSuccessStatusCode)
            {
                string result = await res.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<MistralResponse>(result);

                return response.choices[0].message.content;
            }

            return "error";
        }
    }
}
