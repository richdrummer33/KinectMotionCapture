using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cubic Interpolation
public class RecorderPlayback : MonoBehaviour
{
    List<PositionTime> recdPts = new List<PositionTime>();
    //public List<Transform> pointTransforms = new List<Transform>();

     Transform mover;

    int index = 1;

    Vector3 interpPosn;
    float t;

    bool play;

    private void Start()
    {
        mover = transform;
    }

    public void StopPlayback()
    {
        play = false;
    }

    public void StartPlayback(List<PositionTime> recordedPoints)
    {
        recdPts = recordedPoints;

        List<Vector3> points = new List<Vector3>();
        List<Vector3> rots = new List<Vector3>();
        List<PositionTime> temp = new List<PositionTime>();

        foreach (PositionTime _pt in recdPts)
        {
            points.Add(_pt.position);
            rots.Add(_pt.rotation);
        }

        Vector3 startPos = recdPts[0].position + (recdPts[0].position - recdPts[1].position);
        Vector3 startRot = recdPts[0].rotation + (recdPts[0].rotation - recdPts[1].rotation);
        PositionTime startPt = new PositionTime(startPos, startRot, -1f);
        
        temp.Add(startPt);
        temp.AddRange(recdPts);

        Vector3 endPos = recdPts[recdPts.Count - 1].position + (recdPts[recdPts.Count - 1].position - recdPts[recdPts.Count - 2].position);
        Vector3 endRot = recdPts[recdPts.Count - 1].rotation + (recdPts[recdPts.Count - 1].rotation - recdPts[recdPts.Count - 2].rotation);
        PositionTime endPt = new PositionTime(endPos, endRot, recdPts[recdPts.Count - 1].time + 1f);

        temp.Add(endPt);

        recdPts = temp;
         
        index = 1;
        play = true;
    }

    void Update() // Actually cubic
    {
        if (play && index < recdPts.Count - 2)
        {
            UpdatePosition();
        }
    }

    Quaternion startRot;

    void UpdatePosition()
    {
        Vector3 p0 = recdPts[index - 1].position;
        Vector3 p1 = recdPts[index + 0].position;
        Vector3 p2 = recdPts[index + 1].position;
        Vector3 p3 = recdPts[index + 2].position;

        float dt = recdPts[index + 1].time - recdPts[index + 0].time;
        Vector3 absDirNorm = recdPts[index + 1].position - recdPts[index + 0].position;

        float t2 = Mathf.Pow(t / dt, 2f); // (1 - Mathf.Cos(t * Mathf.PI)) / 2f;

        Vector3 a0 = p3 - p2 - p0 + p1;
        Vector3 a1 = p0 - p1 - a0;
        Vector3 a2 = p2 - p0;
        Vector3 a3 = p1;

        interpPosn = a0 * t / dt * t2 + a1 * t2 + a2 * t / dt + a3;// p1 * (1 - mu2) + p2 * mu2;

        //float dAngle = Vector3.Angle(interpPosn - mover.position, absDirNorm);

        //float velMod = Mathf.Sin(dAngle / 360f * 2 * Mathf.PI);

        mover.localPosition = interpPosn;

        t += Time.deltaTime;

        transform.localRotation = Quaternion.Lerp(startRot, Quaternion.Euler(recdPts[index + 1].rotation), t / dt);

        if (t / dt >= 1f)
        {
            index++;
            t = 0f;
            startRot = transform.localRotation;
        }
    }
}

