using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

[Serializable]
public class HuggingFaceRequest
{
    public string inputs;
}

[Serializable]
public class HuggingFaceResponse
{
    public string generated_text;
}

public class HuggingFaceClass : MonoBehaviour
{
    [SerializeField] private string huggingFaceApiKey = "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"; 

    private const string API_URL = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";
    

    
    // Call this method when you want to make an API request
    public void MakeHuggingFaceRequest(string inputText)
    {
        StartCoroutine(SendRequest(inputText));
    }

    private IEnumerator SendRequest(string inputText)
    {
        // Create the request object
        HuggingFaceRequest requestData = new HuggingFaceRequest
        {
            inputs = inputText
        };

        // Convert request to JSON
        string jsonData = JsonUtility.ToJson(requestData);

        // Create the web request
        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + huggingFaceApiKey);

            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the response
                string responseText = request.downloadHandler.text;
                HuggingFaceResponse response = JsonUtility.FromJson<HuggingFaceResponse>(responseText);
                
                // Use the generated text
                Debug.Log("Generated Text: " + response.generated_text);
                // You can do something with the response here
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    
}