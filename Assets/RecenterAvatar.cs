using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecenterAvatar : MonoBehaviour
{
    [SerializeField] Transform centerPoint;
    Transform avatar;

    private void Start()
    {
        avatar = GetComponentInChildren<RecorderRootController>().transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Period))
            Recenter();
    }

    public void Recenter()
    {
        GetComponentInChildren<AvatarController>().resetInitialTransform(centerPoint.position, centerPoint.rotation.eulerAngles); // .ResetOffsetPos(centerPoint.position + (centerPoint.position - avatar.position));
     //   transform.position = centerPoint.position + (centerPoint.position - avatar.position);
    }
}
