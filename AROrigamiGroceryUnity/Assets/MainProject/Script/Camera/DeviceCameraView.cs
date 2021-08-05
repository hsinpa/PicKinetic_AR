using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

namespace PicKinetic
{
    public class DeviceCameraView : GeneralCameraView
    {
        [Header("AR Foundation")]
        [SerializeField]
        private ARFoundationSystem arFoundationSystem;

        private RenderTexture arBackgroundRenderer;

        List<ARRaycastHit> aRRaycastHits = new List<ARRaycastHit>();
        private CommandBuffer commandBuffer;

        public override void CameraInitProcess() {
            base.CameraInitProcess();

            StartCoroutine(
            arFoundationSystem.CheckARAvailability((bool arSupport) =>
            {
                EnableProcess(arSupport);
                SetupDeviceConfig();
            }));
        }

        private void SetupDeviceConfig()
        {
            arBackgroundRenderer = TextureUtility.GetRenderTexture(Screen.width, Screen.height, 24);

            commandBuffer = new CommandBuffer();
            commandBuffer.name = "AR Camera Background Blit Pass";

            base.DeviceInitiate(Camera.main, GetARCameraTex(), Screen.width, Screen.height);
        }

        void Update()
        {
            if (!isEnable) return;

            UpdateCameraTex();
            OnUpdate();
        }

        private void UpdateCameraTex()
        {
            if (arBackgroundRenderer != null && arFoundationSystem.arCameraBG.material != null && commandBuffer != null)
            {
                //var commandBuffer = new CommandBuffer();
                //commandBuffer.name = "AR Camera Background Blit Pass";
                commandBuffer.Clear();

                var texture = !arFoundationSystem.arCameraBG.material.HasProperty("_MainTex") ? null : arFoundationSystem.arCameraBG.material.GetTexture("_MainTex");
                Graphics.SetRenderTarget(arBackgroundRenderer.colorBuffer, arBackgroundRenderer.depthBuffer);
                commandBuffer.ClearRenderTarget(true, false, Color.clear);
                commandBuffer.Blit(texture, BuiltinRenderTextureType.CurrentActive, arFoundationSystem.arCameraBG.material);
                Graphics.ExecuteCommandBuffer(commandBuffer);
            }
        }

        private Texture GetARCameraTex()
        {
            //If AR Foundation is available
            if (arFoundationSystem.arCameraBG.enabled && arBackgroundRenderer != null)
            {
                return arBackgroundRenderer;
            }

            return null;
        }

        protected override TextureUtility.RaycastResult GetRaycastResult(Vector2 screenPos)
        {
            var screenCenter = _camera.ViewportToScreenPoint(screenPos);
            bool hasHitSomething = arFoundationSystem.arRaycastManager.Raycast(screenCenter, aRRaycastHits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

            _raycastResult.hasHit = hasHitSomething;

            if (hasHitSomething)
            {
                _raycastResult.hitPoint = aRRaycastHits[0].pose.position + placementOffset;
                _raycastResult.hitRotation = aRRaycastHits[0].pose.rotation;
            }

            return _raycastResult;
        }

        public override void EnableProcess(bool enable)
        {
            base.EnableProcess(enable);

            arFoundationSystem.EnableARSession(enable);
        }
    }
}