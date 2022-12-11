using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Complete
{
    public class NetworkGameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject tankPrefab;
        [SerializeField] private MPCameraController camRig;
        [SerializeField] private Transform sp1, sp2;
        [SerializeField] private TankManager[] tankManagers;

        private void Start()
        {
            tankManagers[0].m_SpawnPoint = PhotonNetwork.IsMasterClient ? sp1 : sp2;

            tankManagers[0].m_Instance = PhotonNetwork.Instantiate(tankPrefab.name,
                tankManagers[0].m_SpawnPoint.position,
                tankManagers[0].m_SpawnPoint.rotation);
            tankManagers[0].m_PlayerNumber = 1;

            //SetCameraTargets();
        }

        private void SetCameraTargets()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Transform[] targets = new Transform[players.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = players[i].transform;
            }
            camRig.m_Targets = targets;
        }


    }
}