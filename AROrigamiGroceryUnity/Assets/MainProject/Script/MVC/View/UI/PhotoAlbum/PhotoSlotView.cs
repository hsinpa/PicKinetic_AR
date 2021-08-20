using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace PicKinetic.View
{
    public class PhotoSlotView : MonoBehaviour
    {
        [SerializeField]
        private RawImage SlotRawImage;
        public Texture SlotTexture => SlotRawImage.texture;

        [SerializeField]
        private Button SlotButton;

        [Header("Hidden Panel")]
        [SerializeField]
        private RectTransform HiddenTransform;

        [SerializeField]
        private Button SummonButton;

        [SerializeField]
        private Button ReturnButton;

        private StructType.MeshJsonData _meshJsonData;
        public StructType.MeshJsonData meshJsonData => _meshJsonData;

        public RectTransform rect => this.GetComponent<RectTransform>();

        public void SetSlotData(StructType.MeshJsonData meshJsonData) {
            this._meshJsonData = meshJsonData;

        }

        public void SetSlotImage(Texture slotTex) {
            SlotRawImage.texture = slotTex;
        }

        public void SetClickEvent(System.Action<PhotoSlotView> ClickEvent, System.Action<PhotoSlotView, bool> SummonEvent) {
            UtilityMethod.SetSimpleBtnEvent(SlotButton, () => ClickEvent(this));

            UtilityMethod.SetSimpleBtnEvent(SummonButton, () => SummonEvent(this, true));
            UtilityMethod.SetSimpleBtnEvent(ReturnButton, () => SummonEvent(this, false));
        }

        public void ShowHiddenPanel(bool show) {
            HiddenTransform.gameObject.SetActive(show);
        }



    }
}