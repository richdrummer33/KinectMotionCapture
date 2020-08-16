using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToggleGestureListener : MonoBehaviour
{
    public List<GameObject> objectsToToggle = new List<GameObject>();
    int index = 0;
    private ToggleActiveGestureListener gestureListener;

    public int numTogglesAllowed = int.MaxValue;
    int numToggles;

    // Start is called before the first frame update
    void Start()
    {
        // get the gestures listener
        gestureListener = ToggleActiveGestureListener.Instance;
    }

    void Update()
    {
        // dont run Update() if there is no gesture listener
        if (!gestureListener)
            return;

        if (gestureListener.IsSwipeUp() && numToggles < numTogglesAllowed)
        {
            objectsToToggle[index].SetActive(!objectsToToggle[index].activeSelf);

            index = (index + 1) % objectsToToggle.Count;
            numToggles++;
        }
    }
}
