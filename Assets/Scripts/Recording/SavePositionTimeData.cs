using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public static class SavePositionTimeData
{
    public static void SaveAllPositionTimes(List<PositionTime> positionTimes, string name)
    {
        if (!Directory.Exists(@"C:\tmp"))
        {
            Directory.CreateDirectory(@"C:\tmp");
        }

        //StreamWriter sw = new StreamWriter(@"C:\tmp\" + name + ".txt");

        using (StreamWriter sw = new StreamWriter(@"C:\tmp\" + name + ".txt"))//File.AppendText(@"C:\tmp\" + name + ".txt")
        {
            sw.WriteLine(positionTimes.Count);

            string comma = ",";

            foreach (PositionTime pt in positionTimes)
            {
                sw.WriteLine(pt.position.x + comma + pt.position.y + comma + pt.position.z + comma + pt.time);
            }

            sw.Close();
        }

    }

    public static List<PositionTime> LoadAllPositionTimes(string name)
    {
        StreamReader sr = new StreamReader(@"C:\tmp\" + name + ".txt");

        int ct = int.Parse(sr.ReadLine()); // 1st line is ct
        Debug.Log("ct " + ct);

        PositionTime[] positionTimes = new PositionTime[ct];
        Debug.Log("positionTimes Len" + positionTimes.Length);

        for (int i = 0; i < ct; i++)
        {
            string line = sr.ReadLine();

            string[] sArray = line.Split(char.Parse(","));

            PositionTime newPt = new PositionTime(Vector3.zero, Vector3.zero, 0f);
            
            Vector3 pos = Vector3.zero;
            float val;

            for (int n = 0; n < 3; n++)
            {
                if (float.TryParse(sArray[n], out val))
                {
                    Debug.Log("posn val " + val);
                    pos[n] = val;
                }
                else
                    Debug.Log("nope posn val");
            }

            newPt.position = pos;
            
            if (float.TryParse(sArray[3], out val)) // Time
            {
                Debug.Log("val " + val);
                newPt.time = val;
            }
            else
                Debug.Log("nope time");

            positionTimes[i] = newPt;

            Debug.Log("position " + positionTimes[i].position + " time " + positionTimes[i].time);
        }

        return positionTimes.ToList<PositionTime>();
    }
}