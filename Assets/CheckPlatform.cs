using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlatform : MonoBehaviour
{
    public List<GameObject> ArObjects;
    public List<GameObject> KinectObjects;

    // Start is called before the first frame update
    void Awake()
    {
        

#if UNITY_EDITOR
        foreach(GameObject obj in ArObjects)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in KinectObjects)
        {
            obj.SetActive(true);
        }
#else
            foreach(GameObject obj in ArObjects)
        foreach(GameObject obj in ArObjects)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in KinectObjects)
        {
            obj.SetActive(false);
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
