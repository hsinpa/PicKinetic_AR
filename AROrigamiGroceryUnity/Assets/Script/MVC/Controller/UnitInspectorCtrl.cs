using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hsinpa.Utility;

namespace PicKinetic.Controller
{
    public class UnitInspectorCtrl : ObserverPattern.Observer
    {
        private UnitInspectModule unitInspectModule;

        [SerializeField, Range(0.1f, 200)]
        private float DragThreshold = 0.1f;

        public override void OnNotify(string p_event, params object[] p_objects)
        {
            switch (p_event)
            {
            }
        }

        private void Start()
        {
            Camera camera = Camera.main;
            unitInspectModule = new UnitInspectModule(SetCurrentSelectedObject, SetFaceInfo, ReleaseSelectObject, ProcessVertical, DragThreshold, camera);
        }

        private void SetFaceInfo(UnitInspectModule.Face p_face)
        {
        }

        private void SetInspectViewEvent()
        {

        }

        private void ProcessVertical(UnitInspectModule.DragDir dragDir, float ratio, float offset, Vector3 puff_center)
        {
            Debug.Log("Vertical ratio " + ratio + ", offset "+ offset);
        }

        private bool SetCurrentSelectedObject(GameObject item)
        {
            return false;
        }

        private void ReleaseSelectObject(UnitInspectModule.GestureEvent gestureInput)
        {

        }

    }
}