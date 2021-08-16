using Hsinpa.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PicKinetic.View;
using PicKinetic.Model;

namespace PicKinetic.Controller {
    public class ScanCtrl : ObserverPattern.Observer
    {
        [Header("Debug")]
        [SerializeField]
        private RawImage RawTexDebug;

        [SerializeField]
        private RawImage ImgProcessTexDebug;
        [Header("UI")]
        [SerializeField]
        private MainCanvasView MainCanvasView;

        [Header("System")]
        [SerializeField]
        private GeneralCameraView generalCameraView;

        private PhotoAlbumModel texModel;
        private ARMainUIView arMainUIView;

        public override void OnNotify(string p_event, params object[] p_objects)
        {
            switch (p_event)
            {
                case EventFlag.Event.GameStart:
                {
                        ScanInit();
                }
                break;

                case EventFlag.Event.OnARDisable:
                    {
                    }
                    break;


                case EventFlag.Event.OnAREnable:
                    {
                    }
                    break;

                case EventFlag.Event.OnAlbumSummon:
                    if (p_objects.Length == 2)
                        OnAlbumSummonEvent((StructType.MeshJsonData) p_objects[0], (RenderTexture)p_objects[1]);

                    break;
            }
        }

        private void ScanInit() {
            texModel = PicKineticAR.Instance.ModelManager.GetModel<PhotoAlbumModel>();

            generalCameraView.OnCamInitProcessEvent += OnCamInitProcessEvent;
            generalCameraView.OnDebugTextureEvent += OnDebugTextureEvent;

            this.arMainUIView = MainCanvasView.SetMainCanvasState<ARMainUIView>(true, false);
            this.arMainUIView.SetScanBtnEvent(OnScanBtnClick);
            this.arMainUIView.SetAlbumBtnEvent(OnAlbumBtnClick);

            //Test only
            //UIARSessionBtn.onClick.AddListener(() => {
            //    generalCameraView.EnableProcess(!generalCameraView.isEnable);
            //});

            generalCameraView.CameraInitProcess();
        }
            
        #region Event
        private void OnCamInitProcessEvent(bool success) { 
            
        }

        private void OnDebugTextureEvent(Texture scaleTex, Texture imgProcessTex)
        {
            RawTexDebug.texture = scaleTex;
            ImgProcessTexDebug.texture = imgProcessTex;
        }

        private async void OnScanBtnClick()
        {
            var grabTexStruct = await generalCameraView.GetCurrentTexturesClone();

            var saveJson = texModel.SaveTempMesh(grabTexStruct.mainTex, grabTexStruct.processedTex);

            if (grabTexStruct.meshObject != null)
                grabTexStruct.meshObject.SetMeshJsonData(saveJson);

            //Only discard mainTex, since processTex is actively use by TextureMeshManager
            TextureUtility.Dispose2D(grabTexStruct.mainTex);
        }

        private void OnAlbumBtnClick()
        {
            PicKineticAR.Instance.Notify(EventFlag.Event.OnPhotoAlbumOpen);
        }

        private async void OnAlbumSummonEvent(StructType.MeshJsonData meshData, RenderTexture renderTexture) {
            await generalCameraView.LoadAlbumTextureToMesh(renderTexture);

        }
        #endregion

        private void OnDestroy()
        {
            generalCameraView.OnCamInitProcessEvent -= OnCamInitProcessEvent;
        }

    }
}