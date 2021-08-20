using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PicKinetic.Model;
using Hsinpa;
using PicKinetic.View;

namespace PicKinetic.Controller
{
    public class PhotoAlbumCtrl : ObserverPattern.Observer
    {
        [Header("UI")]
        [SerializeField]
        private MainCanvasView MainCanvasView;

        private PhotoAlbumView photoAlbumView;

        private PhotoAlbumModel textureModel;
        private TexturePool texturePool;

        public override void OnNotify(string p_event, params object[] p_objects)
        {
            switch (p_event)
            {
                case EventFlag.Event.GameStart:
                    texturePool = PicKineticAR.Instance.ModelManager.GetModel<TexturePool>();
                    textureModel = PicKineticAR.Instance.ModelManager.GetModel<PhotoAlbumModel>();

                    photoAlbumView = MainCanvasView.GetCanvasWithType<PhotoAlbumView>();

                    break;

                case EventFlag.Event.OnPhotoAlbumOpen:
                    OnPhotoAlbumOpen();
                    break;

                case EventFlag.Event.OnPhotoAlbumClose:
                    MainCanvasView.CloseAll();
                    MainCanvasView.SetMainCanvasState<ARMainUIView>(true, animation: true);
                    break;

                case EventFlag.Event.OnAlbumSummon:

                    break;
            }
        }

        #region UI Event
        private void OnPhotoAlbumOpen() {
            MainCanvasView.CloseAll();

            var photoAlbumView = MainCanvasView.SetMainCanvasState<PhotoAlbumView>(true, animation: true);

            Utilities.UtilityMethod.SetSimpleBtnEvent(photoAlbumView.CloseButton, () => {
                PicKineticAR.Instance.Notify(EventFlag.Event.OnPhotoAlbumClose);
            });

            photoAlbumView.EnableGridLayout(true);
            photoAlbumView.LoadAlbum(textureModel.SRPTextureRoot.TextureData, OnSlotViewCreate);
        }

        private void OnSlotViewCreate(PhotoSlotView slotView) {

            string fullPath = textureModel.GetFullPath(slotView.meshJsonData.mainTexPath, isTempFolder: false);
            texturePool.LoadTexture(fullPath, (Texture2D tex) =>
            {
                slotView.SetSlotImage(tex);
            });

            slotView.SetClickEvent(OnSlotViewClick, OnSlotSummonClick);
        }

        private void OnSlotViewClick(PhotoSlotView slotView) {
                var rectTransform = slotView.GetComponent<RectTransform>();

                Vector3 targetPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 1);

                photoAlbumView.EnableGridLayout(false);
                photoAlbumView.EnlargeSlot(slotView, size: new Vector2(Screen.width * 0.7f, Screen.width * 0.7f), targetPosition);
                //PicKineticAR.Instance.Notify(EventFlag.Event.OnAlbumSummon, slotView.meshJsonData, renderTex);            
        }

        private void OnSlotSummonClick(PhotoSlotView slotView, bool isSummon) {
            if (slotView.SlotTexture != null)
            {
                var renderTex = TextureUtility.TextureToRenderTexture(slotView.SlotTexture);
            }
        }
        #endregion
    }
}