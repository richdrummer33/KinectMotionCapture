using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TransformFollow : MonoBehaviourPun, IPunObservable
{
    public static Vector3 feetOffset;
    public static Vector3 footPos;
    Vector3 originStartPos;
    
    public Transform otherToFollow;
    public bool negateOffsetfromParent = false;
    public bool disableRenderers = false;

    float feetCalDuration = 4f;
    public int boneIndex;

    Vector3 offset = Vector3.zero;
    public static float miny = Mathf.Infinity;

    void Update()
    {
        if (otherToFollow && feetCald)
        {
            transform.position = otherToFollow.position + offset + (transform.root.position - originStartPos);
            transform.rotation = otherToFollow.rotation;
        }
    }

    bool feetCald;
    IEnumerator CalFeetHeight()
    {
        float t = 0f;

        while (t < feetCalDuration)
        {
            if (transform.position.y < miny) // Find feet height (lowest pt)
            {
                feetOffset = transform.position - otherToFollow.position;
                footPos = transform.position;
            }

            t += Time.deltaTime;
            yield return null;
        }


        if (negateOffsetfromParent) // Not zero!
            offset = feetOffset; 

        feetCald = true;

        if (disableRenderers)
            GetComponent<MeshRenderer>().enabled = false;
    }

    private void Start()
    {
        originStartPos = transform.root.position;

        StartCoroutine(DelayedUpdateVars());

        StartCoroutine(CalFeetHeight());
    }

    IEnumerator DelayedUpdateVars()
    {
        yield return new WaitForSeconds(5f);
        int temp = boneIndex;
        boneIndex = temp;
        //photonView.RPC("SyncVars", RpcTarget.All, 0);
    }

    /*[PunRPC]
    void SyncVars()
    {
        int temp = boneIndex;
        boneIndex = temp;
    }*/
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(boneIndex); // We own this player: send the others our data
        }
        else
        {
            boneIndex = (int)stream.ReceiveNext(); // Network player, receive data
        }
    }
}
