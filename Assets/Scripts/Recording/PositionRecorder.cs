using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PositionRecorder : MonoBehaviour
{
    //[SerializeField]
    //List<PositionTime> recordedPosnPoints = new List<PositionTime>();

    [SerializeField]
    JointMotion recordedMotion;

    public RecorderPlayback playback; // public so you can assign to another object that can mirror this movement
    RecorderRootController recorder;


    [SerializeField]
    int frameStep = 20;

    bool recording;

    int frameCt;

    private void Awake()
    {
        recorder = transform.root.GetComponentInChildren<RecorderRootController>();
    }

    private void OnEnable()
    {
        recorder.OnRecordingStarted += StartRecording;
        recorder.OnRecordingStopped += StopAndSaveRecording;
    }

    private void OnDisable()
    {
        recorder.OnRecordingStarted -= StartRecording;
        recorder.OnRecordingStopped -= StopAndSaveRecording;
    }

    void Update()
    {
        if(recording && frameCt % frameStep == 0)
        {
            PositionTime newPoint = new PositionTime(transform.localPosition, transform.localRotation.eulerAngles, Time.time);
            recordedMotion.AddPoint(newPoint);   //recordedPosnPoints.Add(new PositionTime(transform.localPosition, transform.localRotation.eulerAngles, Time.time));
        }

        frameCt++;
    }

    public void StartRecording(int frameStep)
    {
        Debug.Log("started rec on T");
        recordedMotion = new JointMotion(name); // RESET

        this.frameStep = frameStep;

        recording = true;
    }

    public void StopAndSaveRecording()
    {
        recording = false;

        SavePositionTimeDataJson.AddMotionData(recordedMotion);

        playback.StopPlayback(); // in  case its playing back instead of  recording
        //SavePositionTimeData.SaveAllPositionTimes(recordedPosnPoints, name);

    }

    public void LoadAndPlayRecording(List<PositionTime> lastRecordedPoints) // ()
    {
        //List<PositionTime> lastRecordedPoints = SavePositionTimeData.LoadAllPositionTimes(name);
        
        playback.StartPlayback(lastRecordedPoints);
    }

}
