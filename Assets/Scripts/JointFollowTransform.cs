using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointFollowTransform : MonoBehaviour
{
    Transform toFollow;

    private void Awake()
    {
        toFollow = GameObject.Find(this.name).transform;
    }
}
