using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SavePositionTimeDataJson 
{
    static MocapData allMotionData = new MocapData();
    static string filePath;

    public static void AddMotionData(JointMotion newMotionData)
    {
        allMotionData.motionData.Add(newMotionData);
    }
    
    public static void SaveJsonData(string characterName, string filePath)
    {
        string _json = JsonUtility.ToJson(allMotionData); // List of all save data inside
        File.WriteAllText(filePath, _json);
        Debug.Log("<color=black>-Json- File Saved to: [" + filePath + "]</color>");
        allMotionData = new MocapData();
    }
    public static MocapData GetJsonData(string filePath)
    {
        if (File.Exists(filePath))
        {
            string rawFileData = File.ReadAllText(filePath);
            Debug.Log("Loaded json data " + filePath);
            return JsonUtility.FromJson<MocapData>(rawFileData);
        }

        return null;
    }

}
