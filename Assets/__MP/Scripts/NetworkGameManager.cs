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
        [SerializeField] private GameObject tankPrefab;
        [SerializeField] private MPCameraController camRig;
        [SerializeField] private Transform sp1, sp2;
        [SerializeField] private MPTankManager[] tankManagers;
        [SerializeField] private Text messageText;
        [SerializeField] private Text timerText;

        private bool startGame = false;
        private bool roundEnded = false;
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
        }

        private IEnumerator StartRound()
        {
            ResetTank();
            DisableTankControl();
            camRig.SetStartPositionAndSize();
            CountdownTimer.SetStartTime();
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
            string message = "Round Ended!";
            Color winnerColor = GetWinnerColor();

            message += "\n" + "<color=#" + ColorUtility.ToHtmlStringRGB(winnerColor) + ">" + 
                roundWinner.NickName + "</color> wins the round\n\n";
            
            foreach (var item in PhotonNetwork.PlayerList)
            {
                //winnerColor = (item.IsLocal) ? Color.red : Color.green;

                message += "<color=#" + ColorUtility.ToHtmlStringRGB(winnerColor) + ">" + 
                        item.NickName + "</color> - " + item.GetScore() + "\n";
            }

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

                Debug.Log("One of the tank is dead.");
                roundEnded = true;
            }

            // for checking whether anyone gets a score increament
            if (changedProps.TryGetValue(PunPlayerScores.PlayerScoreProp, out var scoreValue))
            {
                roundWinner = targetPlayer;
            }
        }

        private void TimerExpired()
        {
            startGame = true;
        }

        private void ResetTank()
        {
            tankManagers[0].Reset();
        }

        private void DisableTankControl()
        {
            tankManagers[0].DisableControl();
        }

        private void EnablableTankControl()
        {
            tankManagers[0].EnableControl();
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

    }
}