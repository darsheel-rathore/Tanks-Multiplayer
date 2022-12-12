using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text statusText;
    [SerializeField] private int mpLevelIndex;

    [SerializeField] private InputField nickNameText;

    [SerializeField] private GameObject loadingAnimation;
    [SerializeField] private GameObject inputs;

    private bool whetherInRoom = false;
    private string statusTextValue = string.Empty;

    #region Unity Methods

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    private void Update()
    {
        statusTextValue = PhotonNetwork.NetworkClientState.ToString();
        statusTextValue = whetherInRoom ? statusTextValue + " \nWaiting for 1 more tank to battle!" : statusTextValue;

        statusText.text = statusTextValue;
    }

    #endregion


    #region Public Callbacks

    public void _BattleBtn()
    {
        PhotonNetwork.JoinRandomRoom();

        // disable input and enable status text
        statusText.enabled = true;
        loadingAnimation.SetActive(true);
        inputs.SetActive(false);
    }

    #endregion



    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        statusText.enabled = false;
        loadingAnimation.SetActive(false);
        inputs.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        whetherInRoom = true;
        base.OnJoinedRoom();

        string nickName = nickNameText.text;

        nickName = string.IsNullOrEmpty(nickName) ? "P_" + Random.Range(0, 1000) : nickName;
        PhotonNetwork.LocalPlayer.NickName = nickName;

        //Debug.Log($"Joined: {PhotonNetwork.CurrentRoom.Name} " +
        //    $"as {PhotonNetwork.NickName} " +
        //    $"in region: {PhotonNetwork.CloudRegion}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        //Debug.Log($"Joining random failed because: {message}, attempting to create");
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        //Debug.Log("Creating room failed, trying again.");
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
