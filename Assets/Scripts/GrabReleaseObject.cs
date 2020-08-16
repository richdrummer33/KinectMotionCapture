using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabReleaseObject : MonoBehaviour, InteractionListenerInterface
{
    public int playerIndex = 0;

    [Tooltip("Whether the left hand interaction is allowed by the respective InteractionManager.")]
    public bool leftHandInteraction = true;

    [Tooltip("Whether the right hand interaction is allowed by the respective InteractionManager.")]
    public bool rightHandInteraction = true;

    [Tooltip("Interaction manager instance, used to detect hand interactions. If left empty, it will be the first interaction manager found in the scene.")]
    private InteractionManager interactionManager;

    public GameObject heldObject;
    public GameObject touchedObject;
    GameObject pickedUpFrom;

    // hand interaction variables
    //private bool isLeftHandDrag = false;
    private InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;

    void Start()
    {
        if (interactionManager == null)
        {
            //interactionManager = InteractionManager.Instance;
            interactionManager = GetInteractionManager();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Grabbable")
        {
            touchedObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == touchedObject)
        {
            touchedObject = null;
        }
    }

    // tries to locate a proper interaction manager in the scene
    private InteractionManager GetInteractionManager()
    {
        // find the proper interaction manager
        MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

        foreach (MonoBehaviour monoScript in monoScripts)
        {
            if ((monoScript is InteractionManager) && monoScript.enabled)
            {
                InteractionManager manager = (InteractionManager)monoScript;

                if (manager.playerIndex == playerIndex && manager.leftHandInteraction == leftHandInteraction && manager.rightHandInteraction == rightHandInteraction)
                {
                    return manager;
                }
            }
        }

        // not found
        return null;
    }
    InteractionManager.HandEventType lastType;
    bool gripped = false;
    // Update is called once per frame
    void Update()
    {
        if (interactionManager != null && interactionManager.IsInteractionInited())
        {
            // no object is currently selected or dragged.
            bool bHandIntAllowed = (leftHandInteraction && interactionManager.IsLeftHandPrimary()) || (rightHandInteraction && interactionManager.IsRightHandPrimary());

            // check if there is an underlying object to be selected
            if (lastHandEvent == InteractionManager.HandEventType.Grip && !gripped)
            {
                Debug.Log("Tried");
                Grab();
                gripped = true;
            }
            else if (lastHandEvent == InteractionManager.HandEventType.Release && gripped)
            {
                Debug.Log("Tried Rel");
                Release();
                gripped = false;
            }
        }
    }

    public void HandGripDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
    {
        //if (!isHandInteracting || !interactionManager)
         //   return;
        if (userId != interactionManager.GetUserID())
            return;

        lastHandEvent = InteractionManager.HandEventType.Grip;
    }

    public void HandReleaseDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
    {
        //if (!isHandInteracting || !interactionManager)
          //  return;
        if (userId != interactionManager.GetUserID())
            return;

        lastHandEvent = InteractionManager.HandEventType.Release;
    }

    public Transform grabTransform;

    private void Grab()
    {
        if (touchedObject && !heldObject)
        {
            pickedUpFrom = new GameObject("Orig Position");
            pickedUpFrom.transform.position = touchedObject.transform.position;
            pickedUpFrom.transform.rotation = touchedObject.transform.rotation;
            pickedUpFrom.transform.parent = touchedObject.transform.parent;

            touchedObject.transform.parent = grabTransform;
            touchedObject.transform.position = grabTransform.transform.position;
            touchedObject.transform.rotation = grabTransform.transform.rotation;

            heldObject = touchedObject;
        }
    }

    private void Release()
    {
        if (heldObject)
        {
            heldObject.transform.position = pickedUpFrom.transform.position;
            heldObject.transform.rotation = pickedUpFrom.transform.rotation;
            heldObject.transform.parent = pickedUpFrom.transform;

            heldObject = null;

            Destroy(pickedUpFrom);
        }
    }


    public bool HandClickDetected(long userId, int userIndex, bool isRightHand, Vector3 handScreenPos)
    {
        return true;
    }

}
