using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private int mpLevelIndex;

    #region Unity Methods

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
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
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        string nickName = "P_" + Random.Range(0, 1000);
        PhotonNetwork.LocalPlayer.NickName = nickName;

        Debug.Log($"Joined: {PhotonNetwork.CurrentRoom.Name} " +
            $"as {PhotonNetwork.NickName} " +
            $"in region: {PhotonNetwork.CloudRegion}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log($"Joining random failed because: {message}, attempting to create");
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("Creating room failed, trying again.");
        CreateRoom();
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        int playerCount = PhotonNetwork.PlayerList.Length;

        if (playerCount > 1)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            PhotonNetwork.LoadLevel(mpLevelIndex);
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }


    #endregion


    #region Private Methods

    private void CreateRoom()
    {
        string roomName = "R_" + Random.Range(0, 1000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)2
        };

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    #endregion
}
