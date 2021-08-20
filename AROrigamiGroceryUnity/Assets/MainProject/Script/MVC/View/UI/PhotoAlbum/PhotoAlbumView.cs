using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
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
        private float animDuration = 0.4f;

        public void Dispose()
        {
            UtilityMethod.ClearChildObject(_Content);
        }

        public void EnableGridLayout(bool enable) {
            _Content.GetComponent<GridLayoutGroup>().enabled = enable;
        }

        public void LoadAlbum(List<StructType.MeshJsonData> meshDataArray, System.Action<PhotoSlotView> OnSlotCreateEvent) {
            Dispose();

            int len = meshDataArray.Count;

            for (int i = 0; i < len; i++) {
                PhotoSlotView gSlotView = UtilityMethod.CreateObjectToParent<PhotoSlotView>(_Content, MeshTexturePrefab);
                gSlotView.SetSlotData(meshDataArray[i]);

                if (OnSlotCreateEvent != null)
                    OnSlotCreateEvent(gSlotView);
            }
        }


        #region Animation
        public void EnlargeSlot(PhotoSlotView slotView, Vector2 size, Vector3 position) {
            this.currentFocusSlot = slotView;

            this.originalPosition = slotView.rect.anchoredPosition;
            this.originalSize = slotView.rect.sizeDelta;

            slotView.rect.DOMove(position, animDuration);
            slotView.rect.DOSizeDelta(size, animDuration);
        }

        public void ResumeSlot() {
            if (this.currentFocusSlot == null) return;

            this.currentFocusSlot.rect.DOMove(originalPosition, animDuration * 0.3f);
            this.currentFocusSlot.rect.DOSizeDelta(originalSize, animDuration * 0.3f);

            this.currentFocusSlot = null;
        }

        #endregion
    }
}