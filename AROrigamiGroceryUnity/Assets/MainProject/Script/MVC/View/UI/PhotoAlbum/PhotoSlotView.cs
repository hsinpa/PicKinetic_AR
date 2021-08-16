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

        private StructType.MeshJsonData _meshJsonData;
        public StructType.MeshJsonData meshJsonData => _meshJsonData;

        public void SetSlotData(StructType.MeshJsonData meshJsonData) {
            this._meshJsonData = meshJsonData;

        }

        public void SetSlotImage(Texture slotTex) {
            SlotRawImage.texture = slotTex;
        }

        public void SetClickEvent(System.Action<PhotoSlotView> ClickEvent) {
            UtilityMethod.SetSimpleBtnEvent(SlotButton, () => ClickEvent(this));
        }

    }
}