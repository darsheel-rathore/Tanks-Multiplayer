using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Complete
{
    public class NetworkGameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private int roundsToWinMatch = 3;
        [SerializeField] private GameObject tankPrefab;
        [SerializeField] private MPCameraController camRig;
        [SerializeField] private Transform sp1, sp2;
        [SerializeField] private MPTankManager[] tankManagers;
        [SerializeField] private Text messageText;
        [SerializeField] private Text timerText;

        private bool startGame = false;
        private bool roundEnded = false;
        private bool gameOver = false;
        private bool shouldLeave = false;

        private int roundsWonByAny = 0;
        private Player roundWinner;

        #region Unity Methods

        public override void OnEnable()
        {
            base.OnEnable();
            CountdownTimer.OnCountdownTimerHasExpired += TimerExpired;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            CountdownTimer.OnCountdownTimerHasExpired -= TimerExpired;
        }

        private void Start()
        {
            tankManagers[0].m_SpawnPoint = PhotonNetwork.IsMasterClient ? sp1 : sp2;

            tankManagers[0].m_Instance = PhotonNetwork.Instantiate(tankPrefab.name,
                tankManagers[0].m_SpawnPoint.position,
                tankManagers[0].m_SpawnPoint.rotation);

            tankManagers[0].m_PlayerNumber = 1;

            tankManagers[0].Setup();

            StartCoroutine(StartGameLoop());
        }

        #endregion


        private IEnumerator StartGameLoop()
        {
            yield return StartCoroutine(StartRound());

            yield return StartCoroutine(RoundPlaying());

            RoundEnding();

            // the player will leave the room in round ending
            // under certain conditions so no need to worry about 
            // the coroutine.
            StartCoroutine(StartGameLoop());
        }

        private IEnumerator StartRound()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CountdownTimer.SetStartTime();
            }

            ResetTank();
            DisableTankControl();
            camRig.SetStartPositionAndSize();
            while(!startGame)
            {
                yield return null;
            }
        }

        private IEnumerator RoundPlaying()
        {
            EnablableTankControl();
            messageText.text = "";

            while(!roundEnded)
            {
                yield return null;
            }
        }

        private void RoundEnding()
        {
            DisableTankControl();
            string message = string.Empty;
            Color winnerColor = GetWinnerColor();

            if (HasAnyoneWonTheMatch())
            {
                message = "Match Ended!";
                message += "\n" + "<color=#" + ColorUtility.ToHtmlStringRGB(winnerColor) + ">" +
                    roundWinner.NickName + "</color> wins the match\n\n";
                StartCoroutine(StartLeavingProcess());
            }
            else
            {
                message = "Round Ended!";

                message += "\n" + "<color=#" + ColorUtility.ToHtmlStringRGB(winnerColor) + ">" +
                    roundWinner.NickName + "</color> wins the round\n\n";

                foreach (var item in PhotonNetwork.PlayerList)
                {
                    winnerColor = (item.IsLocal) ? Color.red : Color.green;

                    message += "<color=#" + ColorUtility.ToHtmlStringRGB(winnerColor) + ">" +
                            item.NickName + "</color> - " + item.GetScore() + "\n";
                }
            }

            CountdownTimer.messageToShow = "Round";
            startGame = false;
            messageText.text = message;
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            // for checking if anyone died
            if (changedProps.TryGetValue(MPTankHealth.GetHealthProp(), out var value))
            {
                float healthValue = (float)value;
                if (healthValue > 0f) return;

                roundEnded = true;
            }

            // for checking whether anyone gets a score increament
            if (changedProps.TryGetValue(PunPlayerScores.PlayerScoreProp, out var scoreValue))
            {
                roundWinner = targetPlayer;

                int roundsWon = (int)scoreValue;
                roundsWonByAny = roundsWon <= roundsWonByAny ? roundsWonByAny : roundsWon;
            }
        }

        private IEnumerator StartLeavingProcess()
        {
            while(!shouldLeave)
            {
                yield return null;
            }
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            PhotonNetwork.LocalPlayer.CustomProperties.Clear();
            PhotonNetwork.LoadLevel(0);
            StopAllCoroutines();
        }

        private void TimerExpired()
        {
            startGame = true;
            shouldLeave = HasAnyoneWonTheMatch();
        }

        private void ResetTank()
        {
            tankManagers[0].m_Instance.GetComponent<PhotonView>().
                RPC("ResetTank", RpcTarget.All, tankManagers[0].m_SpawnPoint.position, 
                tankManagers[0].m_SpawnPoint.rotation);
        }

        private void DisableTankControl()
        {
            tankManagers[0].m_Instance.GetComponent<PhotonView>().
                RPC("StartStopControl", RpcTarget.All, false);
        }

        private void EnablableTankControl()
        {
            tankManagers[0].m_Instance.GetComponent<PhotonView>().
                RPC("StartStopControl", RpcTarget.All, true);
        }

        private Color GetWinnerColor()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player.activeSelf)
                {
                    Color realColor = player.GetComponentInChildren<MeshRenderer>().material.color;
                    return realColor;
                }
            }

            return Color.white;
        }

        private bool HasAnyoneWonTheMatch() => roundsWonByAny >= roundsToWinMatch;

    }
}