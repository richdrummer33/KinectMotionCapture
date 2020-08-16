using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using System.IO;

public class RecorderRootController : MonoBehaviour
{
    enum RecordState { Stopped, Recording, Playing }
    RecordState state = RecordState.Stopped;

    public List<PositionRecorder> toRecord = new List<PositionRecorder>();

    [SerializeField] bool reload;
    [SerializeField] int frameStep = 20;

    MocapData recordedMocapData = new MocapData();

    public delegate void StopRecordingEvent();
    public StopRecordingEvent OnRecordingStopped;

    public delegate void StartRecordingEvent(int frameStep);
    public StartRecordingEvent OnRecordingStarted;

    public delegate void PlaybackEvent(Dictionary<string, List<PositionTime>> jointMotionData);
    public PlaybackEvent PlaybackNow;

    public Text statusText;

    string filePath;
    string fileNameBase;

    public List<AudioClip> dialogueClips;
    int dialogueIndex = 0;
    AudioSource source;
    public float startDelayDialogue = 2f;

    [Tooltip("The animation file # that you want to play")]
    [SerializeField] int animationIndex = 0;

    List<string> animFilePaths = new List<string>();

    private void Start()
    {
        fileNameBase = "_" + name + "_mocapData";
        filePath = Application.persistentDataPath + "/MocapRecordings/" + name + "/";

        Directory.CreateDirectory(filePath);

        animFilePaths = Directory.GetFiles(filePath).ToList(); // refresh
        animFilePaths.Sort(HelperMethods.SortByNum);
        animationIndex = animFilePaths.Count - 1;

        Debug.Log("There are " + animFilePaths.Count + " animations available for " + name);

        GameObject recorderTextGo = GameObject.Find("Recorder Status Text");

        if (recorderTextGo)
        {
            statusText = recorderTextGo.GetComponent<Text>();
            statusText.text = "READY";
        }

        source = GetComponent<AudioSource>();
    }

    private void OnValidate()
    {
        if (reload)
        {
            reload = false;

            toRecord = new List<PositionRecorder>();

            Transform[] allTs = gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform t in allTs)
            {
                PositionRecorder pr = null;
                RecorderPlayback rp = null;

                pr = t.GetComponent<PositionRecorder>();
                if (!pr)
                    pr = t.gameObject.AddComponent<PositionRecorder>();

                rp = t.GetComponent<RecorderPlayback>();
                if (!rp)
                    rp = t.gameObject.AddComponent<RecorderPlayback>();

                if (pr && rp)
                    pr.playback = rp;

                toRecord.Add(pr);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            StartRecording();
        else if (Input.GetKeyDown(KeyCode.S))
            StopRecording();
        else if (Input.GetKeyDown(KeyCode.P))
            LoadAndPlayRecording();
    }

    public void StartRecording()
    {
        Debug.Log("started rec");
        StartCoroutine(StartRecordDelayed());
    }

    string clipPathName = "";
    void RecordNow()
    {
        statusText.text = "RECORDING";
        statusText.color = Color.red;
        OnRecordingStarted?.Invoke(frameStep);

        state = RecordState.Recording;
    }

    public void StopRecording()
    {

        Debug.Log("stopped rec");
        statusText.text = "STOPPED";
        statusText.color = Color.white;
        OnRecordingStopped?.Invoke();

        if (state == RecordState.Recording)
        {
            animationIndex++;
            string savePath = filePath + animationIndex + fileNameBase + clipPathName + ".json";
            SavePositionTimeDataJson.SaveJsonData(gameObject.name, savePath);

            animFilePaths = Directory.GetFiles(filePath).ToList(); // refresh
            animFilePaths.Sort(HelperMethods.SortByNum);
        }

        source.Stop();

        state = RecordState.Stopped;
    }

    public void LoadAndPlayRecording()
    {
        Debug.Log("playback rec");
        statusText.text = "PLAYING";
        statusText.color = Color.green;

        if (animationIndex < 0)
        {
            Debug.Log("No anims for " + name);
            return;
        }

        string path = animFilePaths[animationIndex];
        MocapData loadedMocapData = SavePositionTimeDataJson.GetJsonData(path); // (filePath)

        Dictionary<string, List<PositionTime>> jointMotionDict = loadedMocapData.motionData.ToDictionary(t => t.jointName, t => t.recordedPoints);

        List<PositionTime> jointMotion = null;

        foreach (PositionRecorder rec in toRecord)
        {
            jointMotionDict.TryGetValue(rec.name, out jointMotion);

            if (jointMotion != null)
                rec.LoadAndPlayRecording(jointMotion);
            else
                Debug.Log("ERROR: no data for " + rec.name);
        }

        source.PlayOneShot(dialogueClips[(dialogueIndex - 1) % dialogueClips.Count]);

        state = RecordState.Playing;
    }

    IEnumerator StartRecordDelayed()
    {
        float t = 4f;
        
        while(t > 0f)
        {
            yield return new WaitForSeconds(1f);

            t -= 1f;

            source.Play();
            statusText.text = t.ToString();

            // if (t <= startDelayDialogue) RecordNow();
        }

        RecordNow();

        if (source && dialogueClips.Count > 0)
        {
            source.PlayOneShot(dialogueClips[dialogueIndex]);
            clipPathName = dialogueClips[dialogueIndex].name;
            dialogueIndex = (dialogueIndex + 1) % dialogueClips.Count;
        }
    }
}

[Serializable]
public class MocapData // Extra data container layer on top of JointMotion, for Json serialization purposes
{
    public List<JointMotion> motionData = new List<JointMotion>();
}

[Serializable]
public class JointMotion
{
    public List<PositionTime> recordedPoints = new List<PositionTime>();
    public string jointName;

    public JointMotion(string jointName)
    {
        recordedPoints = new List<PositionTime>();
        this.jointName = jointName;
    }

    public void AddPoint(PositionTime newPoint)
    {
        recordedPoints.Add(newPoint);
    }
}

[Serializable]
public class PositionTime
{
    public Vector3 position;
    public Vector3 rotation;
    public float time;

    public PositionTime(Vector3 position, Vector3 rotation, float time)
    {
        this.position = position;
        this.rotation = rotation;
        this.time = time;
    }
}

public static class HelperMethods
{
    public static int SortByNum(string s1, string s2)
    {
        string[] s1n = s1.Split('/');
        Debug.Log("s1n[s1n.Length - 1] " + s1n[s1n.Length - 1]);
        string num1s = s1n[s1n.Length - 1].Split('_')[0];
        int num1 = int.Parse(num1s);

        string[] s2n = s2.Split('/');
        string num2s = s2n[s2n.Length - 1].Split('_')[0];
        int num2 = int.Parse(num2s);

        return num1.CompareTo(num2);
    }
}