using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace PicKinetic
{
    public class ARFoundationSystem : MonoBehaviour
    {
        [Header("AR Foundation")]
        [SerializeField]
        private ARRaycastManager _arRaycastManager;
        public ARRaycastManager arRaycastManager => _arRaycastManager;

        [SerializeField]
        private ARCameraBackground _arCameraBG;
        public ARCameraBackground arCameraBG => _arCameraBG;

        [SerializeField]
        private ARSession _arSession;
        public ARSession arSession => _arSession;

        public IEnumerator CheckARAvailability(System.Action<bool> callback)
        {
            if ((ARSession.state == ARSessionState.None) ||
                (ARSession.state == ARSessionState.CheckingAvailability))
            {
                yield return ARSession.CheckAvailability();
            }

            callback(ARSession.state != ARSessionState.Unsupported);
            //if (ARSession.state == ARSessionState.Unsupported)
            //{
            //    // Start some fallback experience for unsupported devices
            //    _arSession.enabled = false;
            //}
            //else
            //{
            //    // Start the AR session
            //    _arSession.enabled = true;
            //}

            ////SetupDeviceConfig();
        }

        public void EnableARSession(bool enable) {
            _arSession.enabled = enable;
        }



    }
}