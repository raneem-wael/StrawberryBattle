//using System;
//using System.IO;
//using System.Threading.Tasks;
//using HuggingFace.API;
//using Newtonsoft.Json;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;

//public class SpeechRecognitionTest : MonoBehaviour
//{
//    [SerializeField] private Button startButton;
//    [SerializeField] private Button stopButton;
//    [SerializeField] private TextMeshProUGUI text;

//    private AudioClip clip;
//    private byte[] bytes;
//    private bool recording;
//    private const string API_URL = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";
//    private void Start()
//    {
//        startButton.onClick.AddListener(StartRecording);
//        stopButton.onClick.AddListener(StopRecording);
//        stopButton.interactable = false;
//    }

//    private void Update()
//    {
//        if (recording && Microphone.GetPosition(null) >= clip.samples)
//        {
//            StopRecording();
//        }
//    }

//    private void StartRecording()
//    {
//        text.color = Color.white;
//        text.text = "Recording...";
//        startButton.interactable = false;
//        stopButton.interactable = true;
//        clip = Microphone.Start(null, false, 10, 44100);
//        recording = true;
//    }

//    private void StopRecording()
//    {
//        var position = Microphone.GetPosition(null);
//        Microphone.End(null);
//        var samples = new float[position * clip.channels];
//        clip.GetData(samples, 0);
//        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
//        recording = false;
//        SendRecording();
//    }

//    private void SendRecording()
//    {
//        text.color = Color.yellow;
//        text.text = "Sending...";
//        stopButton.interactable = false;
//        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, async response => {
//            text.color = Color.black;
//            text.text = "You : " + response + "\n";
//            Debug.Log(response);
//            _=  sendResponseToAPI(response);
            
//            startButton.interactable = true;
//        }, error => {
//            text.color = Color.red;
//            text.text = error;
//            startButton.interactable = true;
//        });
        
//    }

//    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
//    {
//        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
//        {
//            using (var writer = new BinaryWriter(memoryStream))
//            {
//                writer.Write("RIFF".ToCharArray());
//                writer.Write(36 + samples.Length * 2);
//                writer.Write("WAVE".ToCharArray());
//                writer.Write("fmt ".ToCharArray());
//                writer.Write(16);
//                writer.Write((ushort)1);
//                writer.Write((ushort)channels);
//                writer.Write(frequency);
//                writer.Write(frequency * channels * 2);
//                writer.Write((ushort)(channels * 2));
//                writer.Write((ushort)16);
//                writer.Write("data".ToCharArray());
//                writer.Write(samples.Length * 2);

//                foreach (var sample in samples)
//                {
//                    writer.Write((short)(sample * short.MaxValue));
//                }
//            }
//            return memoryStream.ToArray();
//        }
//    }
//    public async Task sendResponseToAPI(string response)
//    {
//        try
//        {
//            using UnityWebRequest request = UnityWebRequest.Post(API_URL, "{\"inputs\": \"" + response + "\"}", "application/json");
//            request.SetRequestHeader("Authorization", "Bearer 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");
//            var operation = request.SendWebRequest();

//            while (!operation.isDone)
//                await Task.Yield();
//            if (request.result != UnityWebRequest.Result.Success)
//            {
//                Debug.LogError($"Error: {request.error}");
//                return;
//            }

//            string jsonResponse = request.downloadHandler.text;
//            var joke = JsonConvert.DeserializeObject<ChuckNorrisJoke[]>(jsonResponse);
            
//            Debug.Log(joke);
//            text.text = "Bot : " + joke[0].generated_text;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"Error fetching joke: {e.Message}");
//            //return e.Message.ToString();
//        }
//    }

//}
