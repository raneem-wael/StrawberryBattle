using System;
using System.IO;
using UnityEngine;

public class MicrophoneRecorder : MonoBehaviour
{
    private AudioClip _recordedClip;
    private string _filePath;
    private bool _isRecording = false;

    public string GetFilePath() => _filePath;

    public void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone detected!");
            return;
        }

        _recordedClip = Microphone.Start(null, false, 5, 44100);
        _isRecording = true;
        Debug.Log("Recording started...");
    }

    public void StopRecording()
    {
        if (!_isRecording) return;

        Microphone.End(null);
        _isRecording = false;
        Debug.Log("Recording stopped.");

        _filePath = Application.persistentDataPath + "/recorded_audio.wav";
        SaveWavFile(_recordedClip, _filePath);
    }

    private void SaveWavFile(AudioClip clip, string filePath)
    {
        if (clip == null)
        {
            Debug.LogError("No audio clip to save!");
            return;
        }

        // Convert AudioClip to WAV format
        byte[] wavData = ConvertAudioClipToWav(clip);
        File.WriteAllBytes(filePath, wavData);
        Debug.Log($"Saved WAV file to: {filePath}");
    }

    private byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        int sampleCount = clip.samples * clip.channels;
        int fileSize = 44 + sampleCount * 2;

        // WAV Header
        writer.Write(new char[] { 'R', 'I', 'F', 'F' });
        writer.Write(fileSize - 8);
        writer.Write(new char[] { 'W', 'A', 'V', 'E' });
        writer.Write(new char[] { 'f', 'm', 't', ' ' });
        writer.Write(16);
        writer.Write((ushort)1);
        writer.Write((ushort)clip.channels);
        writer.Write(clip.frequency);
        writer.Write(clip.frequency * clip.channels * 2);
        writer.Write((ushort)(clip.channels * 2));
        writer.Write((ushort)16);
        writer.Write(new char[] { 'd', 'a', 't', 'a' });
        writer.Write(sampleCount * 2);

        // Audio Data
        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);
        foreach (float sample in samples)
        {
            short intSample = (short)(sample * short.MaxValue);
            writer.Write(intSample);
        }

        return stream.ToArray();
    }
}
