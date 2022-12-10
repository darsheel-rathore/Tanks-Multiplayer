using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI statusText;


    #region Unity Methods

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    private void Update()
    {
        statusText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    #endregion


    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        CreateOrJoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        string nickName = "P_" + Random.Range(0, 1000);
        PhotonNetwork.LocalPlayer.NickName = nickName;

        Debug.Log($"Joined: {PhotonNetwork.CurrentRoom.Name} || as {PhotonNetwork.NickName}");
    }

    #endregion


    #region Private Methods

    private void CreateOrJoinRandomRoom()
    {
        string roomName = "R_" + Random.Range(0, 1000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = false,
            MaxPlayers = (byte)2
        };

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    #endregion
}
