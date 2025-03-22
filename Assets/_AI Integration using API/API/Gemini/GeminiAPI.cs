using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

public class GeminiAPI : MonoBehaviour
{
    private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta/models/";

    [SerializeField] private string _modelName = "gemini-2.0-flash";
    [SerializeField] private string _apiKey;
    [SerializeField, TextArea(3, 10)] private string _systemInstructions;
    [SerializeField] private bool _enableChatHistory = true;
    [SerializeField] public List<Content> _chatHistory = new List<Content>();

    private void Awake()
    {
        InitializeChatHistory();
    }

    private void InitializeChatHistory()
    {
        _chatHistory.Clear();
    }

    private Content CreateMessageObject(string role, string text)
    {
        return new Content { role = role, parts = new List<Part> { new Part { text = text } } };
    }

    public void ClearChatHistory()
    {
        InitializeChatHistory();
    }

    public async Task<string> GenerateContentAsync(string prompt)
    {
        string url = $"{BASE_URL}{_modelName}:generateContent?key={_apiKey}";
        var requestBody = CreateRequestBody(prompt);
        string jsonData = JsonUtility.ToJson(requestBody);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        try
        {
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API Request failed: {request.error}\nResponse: {request.downloadHandler.text}");
                return null;
            }

            var responseJObject = JObject.Parse(request.downloadHandler.text);
            string aiResponse = responseJObject["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            if (_enableChatHistory && !string.IsNullOrEmpty(aiResponse))
            {
                _chatHistory.Add(CreateMessageObject("model", aiResponse));
            }

            return aiResponse;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during API request: {e.Message}");
            return null;
        }
    }

    private GeminiRequestBody CreateRequestBody(string prompt)
    {
        var requestBody = new GeminiRequestBody
        {
            contents = new List<Content>(),
            generationConfig = new GenerationConfig()
        };

        if (!string.IsNullOrEmpty(_systemInstructions))
        {
            requestBody.systemInstruction = new Content { role = "system", parts = new List<Part> { new Part { text = _systemInstructions } } };
        }

        if (_enableChatHistory)
        {
            requestBody.contents = _chatHistory.Select(msg => new Content { role = msg.role, parts = new List<Part> { new Part { text = msg.parts[0].text } } }).ToList();
            requestBody.contents.Add(new Content { role = "user", parts = new List<Part> { new Part { text = prompt } } });
        }
        else
        {
            requestBody.contents.Add(new Content { role = "user", parts = new List<Part> { new Part { text = prompt } } });
        }

        return requestBody;
    }

    [Serializable] private struct GeminiRequestBody { public List<Content> contents; public Content systemInstruction; public GenerationConfig generationConfig; }
    [Serializable] public struct Content { public string role; public List<Part> parts; }
    [Serializable] public struct Part { public string text; }
    [Serializable] public struct GenerationConfig { public string responseMimeType; }
}
