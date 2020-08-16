using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class MocapVoiceListener : MonoBehaviour
{
    PhraseRecognizer recognizer;
    Dictionary<string, System.Action> actions = new Dictionary<string, System.Action>();
    public ConfidenceLevel conf = ConfidenceLevel.Low;

    RecorderRootController recorder;
    RecenterAvatar recenter;

    void Start()
    {
        recorder = GetComponentInChildren<RecorderRootController>();
        recenter = GetComponent<RecenterAvatar>();

        actions.Add("start", StartRec);
        actions.Add("stop", StopRec);
        actions.Add("play", Play);
        actions.Add("reset", ResetPosition);

        Debug.Log("ACTIONS: " + actions.Keys.ToArray()[0]);
        Debug.Log("PhraseRecognitionSystem.isSupported: " + PhraseRecognitionSystem.isSupported);

        recognizer = new KeywordRecognizer(actions.Keys.ToArray(), conf);
        recognizer.OnPhraseRecognized += RecognizedWords;
        recognizer.Start();
    }

    void RecognizedWords(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Recognized: " + args.text);
        actions[args.text].Invoke();
    }

    void StartRec()
    {
        recorder.StartRecording();
    }

    void StopRec()
    {
        recorder.StopRecording();
    }

    void Play()
    {
        recorder.LoadAndPlayRecording();
    }

    void ResetPosition()
    {
        recenter.Recenter();
    }

    private void OnApplicationQuit()
    {
        if (recognizer.IsRunning)
            recognizer.Stop();
    }
}
