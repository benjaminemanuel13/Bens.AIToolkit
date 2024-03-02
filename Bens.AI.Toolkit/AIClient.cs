using Azure.AI.OpenAI;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BensAIToolkit.Models;
using BensAIToolkit.Models.Assistant;
using Azure.Search.Documents;
using System.Net.Http.Headers;

namespace Bens.AI.Toolkit
{
    public class AIClient
    {
        public static string AzureUrl { get; set; } = string.Empty;
        public static string AzureCredentials { get; set; } = string.Empty;
        public static string AzureDeployment { get; set; } = string.Empty;

        public static string SearchUrl { get; set; } = string.Empty;
        public static string SearchCredentials { get; set; } = string.Empty;
        public static string SearchIndex { get; set; } = string.Empty;

        private WordDocumentService _word = new WordDocumentService();
        private OCRDocumentService _ocr = new OCRDocumentService();

        public static string CognitiveKey
        {
            set
            {
                OCRDocumentService.CognitiveKey = value;
            }
        }

        public static string CognitiveEndpoint
        {
            set
            {
                OCRDocumentService.CognitiveEndpoint = value;
            }
        }

        // CHAT

        public async Task<string> Ask(string text)
        {
            OpenAIClient client = new OpenAIClient(new Uri(AzureUrl), new AzureKeyCredential(AzureCredentials));

            try
            {
                Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(AzureDeployment, new ChatCompletionsOptions()
                {
                    Messages =
                    {
                        new ChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information."),
                        new ChatMessage(ChatRole.User, text)
                    },
                    Temperature = (float)0,
                    MaxTokens = 800,
                    NucleusSamplingFactor = (float)0.95,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0,
                });

                ChatCompletions completions = responseWithoutStream.Value;

                return completions.Choices[0].Message.Content;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<AzureAnswer> AskDocuments(string text, List<AzureMessage> conversation = null)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            int count = 0;

            AzureAnswer answer = null;

            var alts = await GetAlternatives(text, "four");

            List<string> alternatives = new List<string>() {
                text
            };
            alternatives.AddRange(alts);

            while (count < 5)
            {
                if (conversation == null || count > 0)
                {
                    conversation = new List<AzureMessage>
                    {
                        new AzureMessage(){
                            role = "system",
                            content = "You are an AI assistant that helps people find information.",
                        },
                        new AzureMessage(){
                            role = "user",
                            content = alternatives[count++]
                        }
                    };
                }
                else
                {
                    count++;
                }

                AzureRequest req = new AzureRequest()
                {
                    top_p = 1.0,
                    temperature = 0,
                    max_tokens = 800,
                    dataSources = new List<AzureDataSource>() {
                    new AzureDataSource(){
                        type = "AzureCognitiveSearch",
                        queryType = "semantic",
                        semanticConfiguration = "default",
                        parameters = new AzureDataSourceParameters(){
                            endpoint = SearchUrl,
                            key = SearchCredentials,
                            indexName = SearchIndex
                        }
                    }
                },
                    messages = conversation
                };

                answer = new AzureAnswer()
                {
                    Answers = new List<string>(),
                    FilePaths = new List<string>(),
                    FileUrls = new List<string>(),
                };

                string json = JsonConvert.SerializeObject(req);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                var resp = await client.PostAsync(AzureUrl + "openai/deployments/" + AzureDeployment + "/extensions/chat/completions?api-version=2023-06-01-preview", content);

                bool good = true;

                if (resp.IsSuccessStatusCode)
                {
                    json = await resp.Content.ReadAsStringAsync();

                    AzureResponse response = JsonConvert.DeserializeObject<AzureResponse>(json);
                    answer.Response = response;

                    foreach (var choice in response.choices)
                    {
                        if (choice.messages != null)
                        {
                            foreach (var message in choice.messages)
                            {
                                try
                                {
                                    string conjson = message.content;

                                    AzureContent con = JsonConvert.DeserializeObject<AzureContent>(conjson);

                                    if (con.citations != null)
                                    {
                                        // Success?
                                        if (con.citations.Count > 0)
                                        {
                                            foreach (var cit in con.citations)
                                            {
                                                answer.FilePaths.Add(cit.filepath);
                                                answer.FileUrls.Add(cit.url);
                                            }
                                        }
                                        else
                                        {
                                            good = false;
                                        }
                                    }
                                }
                                catch
                                {
                                    answer.Answers.Add(message.content);
                                }
                            }
                        }
                    }

                    if (good)
                        return answer;
                }
            }
            return answer;
        }

        private async Task<List<string>> GetAlternatives(string question, string number = "ten")
        {
            string q = "formatted as a comma seperated list with no numbering, give me " + number + " alternatives for the following: \"" + question + "\"";

            var resp = await NewWay(q);

            if (resp != null)
            {
                resp = resp.Replace(" \"", "").Replace("\"", "").Replace("'", "").ToLower();

                string[] response = resp.Split(",");
                return response.ToList();
            }

            return null;
        }

        private async Task<string> NewWay(string text)
        {
            OpenAIClient client = new OpenAIClient(new Uri(AzureUrl), new AzureKeyCredential(AzureCredentials));

            Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(
                AzureDeployment,
                new ChatCompletionsOptions()
                {
                    Messages =
                    {
                        new ChatMessage(ChatRole.System, @"You are an AI assistant that helps people find information."),
                        new ChatMessage(ChatRole.User, text),

                    },
                    Temperature = (float)0,
                    MaxTokens = 800,
                    NucleusSamplingFactor = (float)1,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0,
                });

            ChatCompletions completions = responseWithoutStream.Value;

            return completions.Choices[0].Message.Content;
        }

        public async void UploadDocument(string id, string title, string url, string path, string filename)
        {
            AzureKeyCredential cred = new AzureKeyCredential(SearchCredentials);

            SearchClient client = new SearchClient(new Uri(SearchUrl), SearchIndex, cred);

            string data = string.Empty;

            if (filename.ToLower().EndsWith(".docx"))
            {
                data = _word.TextFromWord(path + filename);
            }
            else if (filename.ToLower().EndsWith(".pdf"))
            {
                data = await _ocr.Recognise(url, filename);
            }
            else if (filename.ToLower().EndsWith(".jpg") || filename.ToLower().EndsWith(".jpeg") || filename.ToLower().EndsWith(".png"))
            {
                data = await _ocr.Recognise(url, filename,true);
            }
            else if (filename.ToLower().EndsWith(".txt") || filename.ToLower().EndsWith(".json") || filename.ToLower().EndsWith(".csv"))
            {
                var stream = File.Open(path + filename, FileMode.Open);
                StreamReader reader = new StreamReader(stream);

                data = reader.ReadToEnd();

                reader.Close();
                stream.Close();
            }

            AzureDocument document = new AzureDocument()
            {
                chunk_id = "0",
                content = data,
                title = title,
                filepath = filename,
                id = id,
                url = url
            };

            List<AzureDocument> documents = new List<AzureDocument>() { document };

            client.UploadDocuments<AzureDocument>(documents);
        }
            // ASSISTANTS

        public async Task<AssistantNewResponse> CreateAssistantAsync(string name, string instructions)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            string url = AzureUrl + "/openai/assistants?api-version=2024-02-15-preview";
            string deployment = AzureDeployment;

            var req = new AssistantRequest()
            {
                instructions = instructions,
                name = name,
                tools = new List<AssistantTool> { new AssistantTool() },
                model = deployment
            };

            string json = JsonConvert.SerializeObject(req);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await client.PostAsync(url, content);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();
                data = data.Replace("object", "_object");

                var resp = JsonConvert.DeserializeObject<AssistantNewResponse>(data);

                return resp;
            }

            return null;
        }

        public async Task<AssistantThread> CreateThreadAsync()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            string url = AzureUrl + "/openai/threads?api-version=2024-02-15-preview";

            StringContent content = new StringContent("", Encoding.UTF8, "application/json");

            var res = await client.PostAsync(url, content);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();

                var resp = JsonConvert.DeserializeObject<AssistantThread>(data);

                return resp;
            }

            return null;
        }

        public async Task<AssistantMessage> AddQuestionToThreadAsync(string question, string threadId)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            string url = AzureUrl + "/openai/threads/" + threadId + "/messages?api-version=2024-02-15-preview";

            var req = new AssistantQuestion()
            {
                content = question
            };

            string json = JsonConvert.SerializeObject(req);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await client.PostAsync(url, content);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();

                var resp = JsonConvert.DeserializeObject<AssistantMessage>(data);

                return resp;
            }

            return null;
        }

        public async Task<AssistantRun> RunThreadAsync(string assistantId, string threadId)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            string url = AzureUrl + "/openai/threads/" + threadId + "/runs?api-version=2024-02-15-preview";

            var req = new AssistantThreadStart()
            {
                assistant_id = assistantId,
            };

            string json = JsonConvert.SerializeObject(req);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await client.PostAsync(url, content);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();

                var resp = JsonConvert.DeserializeObject<AssistantRun>(data);

                return resp;
            }

            return null;
        }

        public async Task<AssistantReponse> GetResponseAsync(string runId, string threadId)
        {
            while ((await RunStatusAsync(runId, threadId)).status != "completed")
            {
                Thread.Sleep(2000);
            }

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            string url = AzureUrl + "/openai/threads/" + threadId + "/messages?api-version=2024-02-15-preview";

            var res = await client.GetAsync(url);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();

                var resp = JsonConvert.DeserializeObject<AssistantReponse>(data);

                return resp;
            }

            return null;
        }

        private async Task<AssistantRunStatus> RunStatusAsync(string runId, string threadId)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            string url = AzureUrl + "/openai/threads/" + threadId + "/runs/" + runId + "?api-version=2024-02-15-preview";

            var res = await client.GetAsync(url);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();

                var resp = JsonConvert.DeserializeObject<AssistantRunStatus>(data);

                return resp;
            }

            return null;
        }

        public async Task<AssistantList> ListAssistantsAsync()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            string url = AzureUrl + "/openai/assistants?api-version=2024-02-15-preview";

            var res = await client.GetAsync(url);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();

                var resp = JsonConvert.DeserializeObject<AssistantList>(data);

                return resp;
            }

            return null;
        }

        public async Task<AssistantRuns> ListRunsAsync(string threadId)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            string url = AzureUrl + "/openai/threads/" + threadId + "/runs?api-version=2024-02-15-preview";

            var res = await client.GetAsync(url);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();

                var resp = JsonConvert.DeserializeObject<AssistantRuns>(data);

                return resp;
            }

            return null;
        }

        public async Task<AssistantReponse> ListMessagesAsync(string threadId)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            string url = AzureUrl + "/openai/threads/" + threadId + "/messages?api-version=2024-02-15-preview";

            var res = await client.GetAsync(url);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();

                var resp = JsonConvert.DeserializeObject<AssistantReponse>(data);

                return resp;
            }

            return null;
        }

        public async Task<string> UploadAssistantFileAsync(string path, string type = "text/csv")
        {
            MultipartFormDataContent content = new MultipartFormDataContent();

            var fileContent = new StreamContent(File.OpenRead(path));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(type);
            content.Add(fileContent, "file", System.IO.Path.GetFileName(path));

            var jsonContent = new StringContent("assistants", Encoding.UTF8, "text/plain");
            content.Add(jsonContent, "purpose");

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", AzureCredentials);

            string url = AzureUrl + "/openai/files?api-version=2024-02-15-preview";

            var res = await client.PostAsync(url, content);

            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadAsStringAsync();

                var resp = JsonConvert.DeserializeObject<AssistantFile>(data);

                return resp.id;
            }

            return null;
        }
    }
}
