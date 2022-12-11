using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Complete
{
    public class CamRegister : MonoBehaviour
    {
        private void Start()
        {
            MPCameraController camController = FindObjectOfType<MPCameraController>();

            Transform[] camTargets = camController.m_Targets;
            List<Transform> myTargets = camTargets.OfType<Transform>().ToList();
            myTargets.Add(this.transform);

            camController.m_Targets = myTargets.ToArray();
        }
    }
}
