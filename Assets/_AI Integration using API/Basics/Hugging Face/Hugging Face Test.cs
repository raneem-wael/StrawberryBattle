using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

[System.Serializable]
public class huggingFaceRequest
{
    public string inputs;
}

[System.Serializable]
public class huggingFaceResponse
{
    public string generated_text;
}

public class HuggingFaceTest : MonoBehaviour
{
    [SerializeField] private string huggingFaceApiKey = "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"; 

    private const string API_URL = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";

    // Call this method to make an API request
    public void MakehuggingFaceRequest(string inputText)
    {
        StartCoroutine(SendRequest(inputText));
    }

    private IEnumerator SendRequest(string inputText)
    {
        huggingFaceRequest requestData = new huggingFaceRequest { inputs = inputText };
        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + huggingFaceApiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;

                // Fix JSON array handling
                responseText = "{\"responses\":" + responseText + "}";

                huggingFaceResponseWrapper wrapper = JsonUtility.FromJson<huggingFaceResponseWrapper>(responseText);

                if (wrapper.responses.Length > 0)
                {
                    Debug.Log("Generated Text: " + wrapper.responses[0].generated_text);
                }
                else
                {
                    Debug.LogError("Error: No response received.");
                }
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}

[System.Serializable]
public class huggingFaceResponseWrapper
{
    public huggingFaceResponse[] responses;
}
