using PicKinetic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PicKinetic
{

    public abstract class GeneralCameraView : MonoBehaviour
    {
        [Header("Config")]
        public Material rotateMat;

        private RenderTexture modelTexRenderer;
        private RenderTexture imageProcessRenderer;

        [SerializeField, Range(0, 0.05f)]
        private float _SizeStrength = 1;

        [SerializeField, Range(0, 1)]
        private float _CropSize = 1;

        [SerializeField]
        TextureMeshManager texturePreivew;

        [SerializeField]
        MeshObject meshBorder;

        [SerializeField]
        private MeshObjectManager meshObjManager;

        [SerializeField]
        private MeshIndicator meshIndicator;

        protected Camera _camera;
        TextureUtility TextureUtility;
        TextureUtility.TextureStructure _textureStructure;
        protected TextureUtility.RaycastResult _raycastResult = new TextureUtility.RaycastResult();
        protected Vector3 placementOffset = new Vector3(0, 0.01f, 0);
        int textureSize = 512;

        float timer;
        float timer_step = 0.2f;

        private Texture _sourceTexture;

        public delegate void DebugTextureEvent(Texture rawScaleTex, Texture imgProcessTex);
        public System.Action<bool> OnCamInitProcessEvent;
        public DebugTextureEvent OnDebugTextureEvent;

        protected bool _isEnable = false;
        public bool isEnable => this._isEnable;

        private bool isScanProcessReady = true;

        public virtual void CameraInitProcess() { 
            
        }

        protected void DeviceInitiate(Camera camera, Texture sourceTexture, int sourceTexWidth, int sourceTexHeight)
        {
            _camera = camera;
            _sourceTexture = sourceTexture;

            texturePreivew.SetUp();
            TextureUtility = new TextureUtility();

            _textureStructure = GrabTextureRadius(sourceTexWidth, sourceTexHeight);

            meshIndicator.SetUp(_camera, GetRaycastResult);

            PrepareTexture();

            if (OnCamInitProcessEvent != null)
                OnCamInitProcessEvent(isEnable);
        }

        protected void OnUpdate()
        {
            if (_sourceTexture == null || !isEnable) return;

            meshIndicator.DisplayOnScreenPos(_textureStructure, _CropSize);

            TextureUtility.RotateAndScaleImage(_sourceTexture, modelTexRenderer, rotateMat, _textureStructure, 0);
            TextureUtility.RotateAndScaleImage(_sourceTexture, imageProcessRenderer, rotateMat, _textureStructure, 0);

            texturePreivew.ExecEdgeProcessing(imageProcessRenderer);

            //if (timer > Time.time) return;
            if (!isScanProcessReady) return;

            isScanProcessReady = false;

            PreviewEdgeMesh();

            timer = timer_step + Time.time;
        }

        private void PrepareTexture()
        {
            modelTexRenderer = TextureUtility.GetRenderTexture(textureSize);
            imageProcessRenderer = TextureUtility.GetRenderTexture((int)(textureSize * 0.5f));

            if (OnDebugTextureEvent != null)
                OnDebugTextureEvent(modelTexRenderer, texturePreivew.edgeLineTex);
        }


        private void OnMeshLocDone(TextureMeshManager.MeshLocData meshResult)
        {
            isScanProcessReady = true;
            if (!meshResult.isValid) return;

            MeshIndicator.IndictatorData indictatorData = meshIndicator.GetRelativePosRot(meshResult.screenPoint);

            float sizeMagnitue = (_camera.transform.position - meshResult.meshObject.transform.position).magnitude * _SizeStrength;
            meshResult.meshObject.transform.localScale = new Vector3(sizeMagnitue, sizeMagnitue, sizeMagnitue);

            meshResult.meshObject.SetPosRotation(indictatorData.position, indictatorData.rotation);
        }

        public async void PreviewEdgeMesh()
        {
            texturePreivew.ProcessCSTextureColor();

            OnMeshLocDone(await texturePreivew.CaptureEdgeBorderMesh(imageProcessRenderer.width, meshBorder, _textureStructure));
        }

        public async void TakeAPhoto()
        {
            MeshObject meshObject = meshObjManager.CreateMeshObj(meshBorder.transform.position, meshBorder.transform.rotation, true);

            OnMeshLocDone(await texturePreivew.CaptureContourMesh(modelTexRenderer, meshObject, _textureStructure));
        }

        public StructType.GrabTextures GetCurrentTexturesClone() {
            return new StructType.GrabTextures
            {
                mainTex = TextureUtility.TextureToTexture2D(modelTexRenderer),
                processedTex = TextureUtility.TextureToTexture2D(imageProcessRenderer)
            };
        }

        protected virtual TextureUtility.RaycastResult GetRaycastResult(Vector2 screenPos)
        {
            return default(TextureUtility.RaycastResult);
        }

        protected  TextureUtility.TextureStructure GrabTextureRadius(int width, int height)
        {
            return TextureUtility.GrabTextureRadius(width, height, _CropSize);
        }

        public virtual void EnableProcess(bool enable)
        {
            _isEnable = enable;
        }

    }
}