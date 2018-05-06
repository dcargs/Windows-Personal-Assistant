using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Windows_Personal_Assistant
{
    class ServerConnection
    {
        private static string url = "http://ec2-34-218-76-81.us-west-2.compute.amazonaws.com:55554";
        private string session_token = "!QAZ@WSX#EDC1qaz2wsx3edc";
        private static Dictionary<string, string> eventActions;
        private static Dictionary<string, string> taskActions;
        private static Dictionary<string, string> noteActions;
        protected HttpClient client;

        public ServerConnection()
        {
            client = new HttpClient();
            eventActions = new Dictionary<string, string>()
                {
                    {"create","/event/create"},
                    {"get", "/event/get"},
                    {"edit","/event/edit"},
                    {"delete","/event/delete"}
                };
            taskActions = new Dictionary<string, string>()
                {
                    {"create","/task/create"},
                    {"get", "/task/get"},
                    {"edit","/task/edit"},
                    {"delete","/task/delete"}
                };
            noteActions = new Dictionary<string, string>()
                {
                    {"create","/note/create"},
                    {"get", "/note/get"},
                    {"edit","/note/edit"},
                    {"delete","/note/delete"}
                };
        }

        public async Task<string> Post_DataAsync(Dictionary<string, string> data, string mainAction, string secondaryAction)
        {
            string post_url = url;
            var values = new Dictionary<string, string> { { "session_token", session_token } };

            switch (mainAction)
            {
                case "task":
                    post_url += taskActions[secondaryAction];
                    switch (secondaryAction)
                    {
                        case "create":
                            values.Add("title", data["title"]);
                            values.Add("due_date", data["due_date"]);
                            values.Add("description", data["description"]);
                            break;

                        case "get":
                            break;

                        case "edit":
                            values.Add("id", data["id"]);
                            values.Add("title", data["title"]);
                            values.Add("due_date", data["due_date"]);
                            values.Add("description", data["description"]);
                            break;

                        case "delete":
                            values.Add("id", data["id"]);
                            break;

                        default:
                            return null;
                    }
                    break;


                case "note":
                    post_url += noteActions[secondaryAction];
                    switch (secondaryAction)
                    {
                        case "create":
                            values.Add("title", data["title"]);
                            values.Add("body", data["body"]);
                            break;

                        case "get":
                            break;

                        case "edit":
                            values.Add("id", data["id"]);
                            values.Add("title", data["title"]);
                            values.Add("body", data["body"]);
                            break;

                        case "delete":
                            values.Add("id", data["id"]);
                            break;

                        default:
                            return null;
                    }
                    break;

                case "event":
                    post_url += eventActions[secondaryAction];
                    switch (secondaryAction)
                    {
                        case "create":
                            values.Add("title", data["title"]);
                            values.Add("date", data["date"]);
                            break;

                        case "get":
                            break;

                        case "edit":
                            values.Add("id", data["id"]);
                            values.Add("title", data["title"]);
                            values.Add("date", data["date"]);
                            break;

                        case "delete":
                            values.Add("id", data["id"]);
                            break;

                        default:
                            return null;
                    }
                    break;

                default:
                    return null;
            }
            
            var response = await client.PostAsync(post_url, new FormUrlEncodedContent(values));
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

    }
}
