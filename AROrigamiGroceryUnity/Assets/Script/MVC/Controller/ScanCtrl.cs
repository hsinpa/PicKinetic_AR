using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        private Button UIScanBtn;

        [SerializeField]
        private Button UIARSessionBtn;

        [Header("System")]
        [SerializeField]
        private GeneralCameraView generalCameraView;


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
            }
        }

        private void ScanInit() {
            generalCameraView.OnCamInitProcessEvent += OnCamInitProcessEvent;
            generalCameraView.OnDebugTextureEvent += OnDebugTextureEvent;

            UIScanBtn.onClick.AddListener(() => OnScanBtnClick());

            //Test only
            UIARSessionBtn.onClick.AddListener(() => {
                generalCameraView.EnableProcess(!generalCameraView.isEnable);
            });

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

        private void OnScanBtnClick()
        {
            StructType.GrabTextures grabTextures = generalCameraView.GetCurrentTexturesClone();

            PicKineticAR.Instance.models.textureModel.SaveTextureToDisk("ScanPhoto.jpg", grabTextures.mainTex);

            TextureUtility.Dispose2D(grabTextures.mainTex);
            TextureUtility.Dispose2D(grabTextures.processedTex);

            generalCameraView.TakeAPhoto();
        }

        #endregion

        private void OnDestroy()
        {
            generalCameraView.OnCamInitProcessEvent -= OnCamInitProcessEvent;
        }

    }
}