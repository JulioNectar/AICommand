using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

namespace AICommand
{

    static class OpenAIUtil
    {
        static string CreateChatRequestBody(string prompt)
        {
            var msg = new OpenAI.RequestMessage();
            msg.role = "user";
            msg.content = prompt;

            var req = new OpenAI.Request();
            // req.model = "gpt-3";
            // req.model = "gpt-3.5";
            req.model = "gpt-3.5-turbo";
            // req.model = "gpt-3-";
            // req.model = "code-cushman-001";
            // req.model = "gpt-3.5-turbo";
            // req.model = " gpt-4-32k-0314";
            req.messages = new[] { msg };

            return JsonUtility.ToJson(req);
        }

        public static string InvokeChat(string prompt)
        {
            var settings = AICommandSettings.instance;

            // POST
            using var post = UnityWebRequest.Post
              (OpenAI.Api.Url, CreateChatRequestBody(prompt), "application/json");

            // Request timeout setting
            post.timeout = settings.timeout;

            // API key authorization
            post.SetRequestHeader("Authorization", "Bearer " + settings.apiKey);

            // Request start
            var req = post.SendWebRequest();

            // Progress bar (Totally fake! Don't try this at home.)
            for (var progress = 0.0f; !req.isDone; progress += 0.01f)
            {
                EditorUtility.DisplayProgressBar
                  ("Gerador de Assets", "Gerando...", progress);
                System.Threading.Thread.Sleep(100);
                progress += 0.01f;
            }
            EditorUtility.ClearProgressBar();

            // Debug.Log(post.downloadHandler.text);

            // Response extraction
            var json = post.downloadHandler.text;
            var data = JsonUtility.FromJson<OpenAI.Response>(json);
            Debug.Log(data);
            return data.choices[0].message.content;
        }
    }

} // namespace AICommand

