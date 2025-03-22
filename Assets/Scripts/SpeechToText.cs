using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using TMPro;

public class SpeechToText : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI _recognizedText;
    [SerializeField] private TextMeshProUGUI _aiResponseText;
    [SerializeField] private Button _recordButton;
    [SerializeField] private GeminiAPI _geminiAPI; // Reference to the GeminiAPI script

    private MicrophoneRecorder _microphoneRecorder;

    private const string STT_API_URL = "https://api-inference.huggingface.co/models/openai/whisper-base";
    private string API_TOKEN = "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";

    private bool _isRecording = false;

    private void Awake()
    {
    _microphoneRecorder = gameObject.AddComponent<MicrophoneRecorder>();
       
        if (_recordButton == null)
        {
            Debug.LogError("Record Button is not assigned in the Inspector!");
            return;
        }

        _recordButton.onClick.AddListener(OnRecordButtonClick);
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) {

            ProcessSpeechCommand("carrot");
        }
        if (Input.GetKey(KeyCode.RightArrow)) {

            ProcessSpeechCommand("banana");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {

            ProcessSpeechCommand("attack");
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {

            ProcessSpeechCommand("shield");
        }
    }


    private void OnDestroy()
    {
        _recordButton.onClick.RemoveListener(OnRecordButtonClick);
    }

    private async void OnRecordButtonClick()
    {
        if (!_isRecording)
        {
            _isRecording = true;
            _recordButton.GetComponentInChildren<Text>().text = "Stop Recording";
            _microphoneRecorder.StartRecording();
        }
        else
        {
            _isRecording = false;
            _recordButton.GetComponentInChildren<Text>().text = "Start Recording";
            _microphoneRecorder.StopRecording();

            string filePath = _microphoneRecorder.GetFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                string transcribedText = await SendSpeechToTextRequest(filePath);
                if (!string.IsNullOrEmpty(transcribedText))
                {
                    await SendTextToGemini(transcribedText);
                }
            }
        }
    }

    private async Task<string> SendSpeechToTextRequest(string audioFilePath)
    {
        try
        {
            await Task.Delay(5000); // Avoid sending requests too frequently

            byte[] audioData = File.ReadAllBytes(audioFilePath);

            using UnityWebRequest request = new UnityWebRequest(STT_API_URL, "POST");
            request.uploadHandler = new UploadHandlerRaw(audioData);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "audio/wav");
            request.SetRequestHeader("Authorization", "Bearer " + API_TOKEN);

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Speech-to-Text Error: {request.error}");
                _recognizedText.text = "Error transcribing speech!";
                return null;
            }

            // Parse JSON response
            string jsonResponse = request.downloadHandler.text;
            JObject responseObj = JObject.Parse(jsonResponse);
            string transcribedText = responseObj["text"]?.ToString();

            _recognizedText.text = transcribedText;
            return transcribedText;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
            _recognizedText.text = "Failed to transcribe speech";
            return null;
        }
    }

    private async Task SendTextToGemini(string prompt)
    {
        if (_geminiAPI == null)
        {
            Debug.LogError("GeminiAPI reference is missing!");
            _aiResponseText.text = "AI system is unavailable.";
            return;
        }

        // Check if the speech contains a skill command
        bool skillActivated = ProcessSpeechCommand(prompt);

        // If a skill was activated, don't call Gemini API
        if (!skillActivated)
        {
            _aiResponseText.text = "Thinking...";
            string response = await _geminiAPI.GenerateContentAsync(prompt);
            _aiResponseText.text = response ?? "Failed to get AI response";
        }
    }


    private bool ProcessSpeechCommand(string text)
    {
        CharacterSkills skills = FindObjectOfType<CharacterSkills>();
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();

        if (skills == null)
        {
            Debug.LogError("CharacterSkills script is missing!");
            return false;
        }
        if (enemyManager == null)
        {
            Debug.LogError("EnemyManager script is missing!");
            return false;
        }

        text = text.ToLower(); // Convert to lowercase for easier matching

        // **Enemy Selection Logic**
        if (text.Contains("carrot"))
        {
            if (enemyManager.HasRemainingEnemy("carrot"))
            {
                enemyManager.SetTarget("carrot");
                _aiResponseText.text = "Target set to Carrot!";
            }
            else
            {
                _aiResponseText.text = "No carrots left to attack!";
            }
            return true;
        }
        else if (text.Contains("banana"))
        {
            if (enemyManager.HasRemainingEnemy("banana"))
            {
                enemyManager.SetTarget("banana");
                _aiResponseText.text = "Target set to Banana!";
            }
            else
            {
                _aiResponseText.text = "No bananas left to attack!";
            }
            return true;
        }

        // **Skill Activation Logic**
        if (text.Contains("fireball") || text.Contains("attack")||text.Contains("shoot"))
        {
            skills.UseSkillOne();
            _aiResponseText.text = "Skill Fireball Activated!";
            return true;
        }
        else if (text.Contains("shield") || text.Contains("defend"))
        {
            skills.UseSkillTwo();
            _aiResponseText.text = "Skill Shield Activated!";
            return true;
        }

        return false; // No match, let Gemini generate response
    }


}


