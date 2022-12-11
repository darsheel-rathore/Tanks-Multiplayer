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
        [SerializeField] private TankManager[] tankManagers;
        [SerializeField] private Text messageText;
        [SerializeField] private Text timerText;

        private bool startGame = false;

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

            RoundPlaying();
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

        private void RoundPlaying()
        {
            EnablableTankControl();
            messageText.text = "";
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
    }
}