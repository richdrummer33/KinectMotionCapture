using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// eSCOLA: By Richard Beare 4/7/2020
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    
    bool isTeacher = false;

    public GameObject kinectCharacterPrefab;
    public GameObject remoteTeacherTrackerPrefab;

    public bool debug;

    #region Unity

    public void Awake()
    {
        Instance = this;

        InitializeScene();
    }

    #endregion

    #region Photon Messages

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        // SceneManager.LoadScene(0);
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    #endregion

    #region Private Methods

    void InitializeScene()
    {
        if (!debug)
        {
            if (PhotonNetwork.IsMasterClient)
                Instantiate(kinectCharacterPrefab, Vector3.zero, Quaternion.identity);
            #if UNITY_EDITOR || UNITY_STANDALONE
            else
                Instantiate(remoteTeacherTrackerPrefab, new Vector3(10f, 5f, 10f), Quaternion.identity);
            #endif
        }
        else
            Instantiate(kinectCharacterPrefab, Vector3.zero, Quaternion.identity);
        //Debug.Log("PhotonNetwork : Loading Level : " + "BALBALBA");
        //PhotonNetwork.LoadLevel("BALBALBA");
    }

    #endregion

    #region Photon Messages

    public override void OnConnected()
    {
        base.OnConnected();

        Debug.Log("connected");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        Debug.Log("OnPhotonPlayerConnected() " + newPlayer.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.IsMasterClient); // called before OnPhotonPlayerDisconnected

          
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.Log("OnPhotonPlayerDisconnected() " + otherPlayer.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.IsMasterClient); // called before OnPhotonPlayerDisconnected

          
        }
    }


    #endregion
}
