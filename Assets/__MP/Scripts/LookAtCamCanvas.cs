using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamCanvas : MonoBehaviourPun
{
    Camera cam;

    void Start()
    {
        if (!this.photonView.IsMine)
        {
            this.gameObject.SetActive(false);
            return;
        }
        cam = Camera.main;
        
    }

    void Update()
    {
        transform.LookAt(cam.transform);
    }
}
