using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteTeacherAvatarController : MonoBehaviour
{
    GameObject punRigTracker;

    // Variable to hold all them bones. It will initialize the same size as initialRotations.
    protected Transform[] bones;
    protected Transform[] fingerBones;

    private Animator animatorComponent = null;

    private void Awake()
    {
        bones = new Transform[31];

        animatorComponent = GetComponent<Animator>();

        StartCoroutine(GetRigTracker());
    }

    IEnumerator GetRigTracker()
    {
        while (punRigTracker == null)
        {
            punRigTracker = GameObject.FindGameObjectWithTag("Rig Tracker");

            yield return new WaitForSeconds(1f);
        }

        MapBones(punRigTracker);
    }

    protected virtual void MapBones(GameObject punRigTracker)
    {
        // get bone transforms from the animator component
        //Animator animatorComponent = GetComponent<Animator>();

        TransformFollow[] punRigTrackerTransforms = punRigTracker.GetComponentsInChildren<TransformFollow>();

        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            if (!AvatarController.boneIndex2MecanimMap.ContainsKey(boneIndex))
                continue;
            
            bones[boneIndex] = animatorComponent ? animatorComponent.GetBoneTransform(AvatarController.boneIndex2MecanimMap[boneIndex]) : null;

            if (bones[boneIndex] != null)
            {
                foreach (TransformFollow toFollow in punRigTrackerTransforms)
                {
                    if (toFollow.boneIndex == boneIndex)
                    {
                        TransformFollow follower = bones[boneIndex].gameObject.AddComponent<TransformFollow>();
                        follower.otherToFollow = toFollow.transform;
                        follower.negateOffsetfromParent = true;
                        break;
                    }
                }
            }
        }
    }
}
