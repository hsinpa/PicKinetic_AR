﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

namespace PicKinetic { 

    public class EditorCameraView : GeneralCameraView
    {
        [Header("Editor Specific")]

        public Texture2D inputTex;
        public GameObject fakeDetectionPlane;

        public void Start()
        {
            fakeDetectionPlane.gameObject.SetActive(false);
        }

        public override void CameraInitProcess()
        {
            base.CameraInitProcess();

            EnableProcess(true);
            DeviceInitiate(Camera.main, inputTex, inputTex.width, inputTex.height);
            fakeDetectionPlane.gameObject.SetActive(true);
        }

        void Update()
        {
            if (!isEnable) return;

            OnUpdate();
        }

        protected override TextureUtility.RaycastResult GetRaycastResult(Vector2 screenPos)
        {
            screenPos.Set(screenPos.x * Screen.width, screenPos.y * Screen.height);
            Ray ray = _camera.ScreenPointToRay(screenPos);
            RaycastHit hit;

            _raycastResult.hasHit = Physics.Raycast(ray, out hit, 100.0f, ParameterFlag.ColliderLayer.FloorLayer);
            _raycastResult.hitPoint = hit.point + new Vector3(0, 0.01f, 0);

            return _raycastResult;
        }
    }
}