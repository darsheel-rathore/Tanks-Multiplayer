using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Complete
{
    public class MPTankOnBoardManager : MonoBehaviour
    {
        [SerializeField] MPTankMovement movement;
        [SerializeField] MPTankShooting shooting;
        [SerializeField] GameObject canvas;

        [PunRPC]
        public void StartStopControl(bool shouldEnableControls)
        {
            movement.enabled = shouldEnableControls;
            shooting.enabled = shouldEnableControls;

            canvas.SetActive(shouldEnableControls);
        }

        [PunRPC]
        public void ResetTank(Vector3 m_SpawnPoint, Quaternion rot)
        {
            transform.position = m_SpawnPoint;
            transform.rotation = rot;

            this.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
        }

    }
}
