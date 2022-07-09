using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using DG.Tweening;

namespace PicKinetic.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PhotoAlbumView : MonoBehaviour, MainViewInterface
    {
        [SerializeField]
        private CanvasGroup _CanvasGroup;

        public CanvasGroup CanvasGroup => _CanvasGroup;

        [SerializeField]
        private RectTransform _Content;
        private RectTransform Content => _Content;

        [SerializeField]
        private PhotoSlotView MeshTexturePrefab;

        public Button CloseButton;

        private Vector2 originalSize;
        private Vector3 originalPosition;
        private PhotoSlotView currentFocusSlot;

        private List<PhotoSlotView> photoSlotList = new List<PhotoSlotView>();

        private float animDuration = 0.3f;

        public void Dispose()
        {
            UtilityMethod.ClearChildObject(_Content);
            photoSlotList.Clear();
        }

        public void EnableGridLayout(bool enable) {
            _Content.GetComponent<GridLayoutGroup>().enabled = enable;
        }

        public void LoadAlbum(List<StructType.MeshJsonData> meshDataArray, System.Action<PhotoSlotView> OnSlotCreateEvent) {
            Dispose();

            int len = meshDataArray.Count;

            Debug.Log("LoadAlbum count " + len);

            for (int i = 0; i < len; i++) {
                PhotoSlotView gSlotView = UtilityMethod.CreateObjectToParent<PhotoSlotView>(_Content, MeshTexturePrefab);
                gSlotView.SetSlotData(meshDataArray[i]);

                photoSlotList.Add(gSlotView);

                if (OnSlotCreateEvent != null)
                    OnSlotCreateEvent(gSlotView);
            }
        }

        #region Animation
        public void EnlargeSlot(PhotoSlotView slotView, Vector2 size, Vector3 position) {
            this.currentFocusSlot = slotView;

            this.originalPosition = slotView.transform.position;
            this.originalSize = slotView.rect.sizeDelta;
            EnableGridLayout(false);

            EnableAllPhotoSlotBtn(false);
            slotView.rect.DOMove(position, animDuration);
            slotView.rect.DOSizeDelta(size, animDuration).OnComplete(() => {
                slotView.ShowHiddenPanel(true);
            });
        }

        public void ResumeSlot() {
            if (this.currentFocusSlot == null) return;

            this.currentFocusSlot.rect.DOMove(originalPosition, animDuration * 0.7f);
            this.currentFocusSlot.rect.DOSizeDelta(originalSize, animDuration * 0.7f).OnComplete(() => {
                EnableGridLayout(true);
                EnableAllPhotoSlotBtn(true);
            });

            this.currentFocusSlot.ShowHiddenPanel(false);
            this.currentFocusSlot = null;
        }

        #endregion

        private void EnableAllPhotoSlotBtn(bool enable) {
            photoSlotList.ForEach(x => x.SlotButton.enabled = enable);
        }

    }
}