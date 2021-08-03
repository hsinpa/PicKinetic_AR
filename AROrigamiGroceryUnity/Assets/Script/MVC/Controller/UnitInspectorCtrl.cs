using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hsinpa.Utility;
using Hsinpa.Utility.Input;

namespace PicKinetic.Controller
{
    public class UnitInspectorCtrl : ObserverPattern.Observer
    {
        private UnitInspectModule unitInspectModule;

        [SerializeField, Range(0.1f, 200)]
        private float DragThreshold = 0.1f;

        private InputWrapper inputWrapper;

        private MeshObject selectedMeshObject;

        public override void OnNotify(string p_event, params object[] p_objects)
        {
            switch (p_event)
            {

            }
        }

        private void Start()
        {
            Camera camera = Camera.main;
            inputWrapper = new InputWrapper();
            unitInspectModule = new UnitInspectModule(inputWrapper, SetCurrentSelectedObject, SetFaceInfo, ReleaseSelectObject, ProcessVertical, DragThreshold, camera);
        }

        private void Update()
        {
            if (unitInspectModule == null) return;
            inputWrapper.OnUpdate();
            unitInspectModule.OnUpdate();
        }

        private void SetFaceInfo(UnitInspectModule.Face p_face)
        {
        }

        private void SetInspectViewEvent()
        {

        }

        private void ProcessVertical(UnitInspectModule.DragDir dragDir, float ratio, float offset, Vector3 puff_center)
        {
            selectedMeshObject.transform.position = new Vector3(puff_center.x, puff_center.y + offset, puff_center.z);
        }

        private bool SetCurrentSelectedObject(Transform item)
        {
            selectedMeshObject = item.GetComponent<MeshObject>();
            selectedMeshObject.enabled = false;
            unitInspectModule.SetInputSelectObject(item);

            return true;
        }

        private void ReleaseSelectObject(UnitInspectModule.GestureEvent gestureInput)
        {
            if (gestureInput != UnitInspectModule.GestureEvent.None) {

                selectedMeshObject.enabled = true;
                selectedMeshObject = null;

                unitInspectModule.SetInputSelectObject(null);
            }
        }

    }
}