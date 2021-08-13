using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PicKinetic.View
{
    public class PhotoSlotView : MonoBehaviour
    {
        [SerializeField]
        private RawImage SlotRawImage;

        private StructType.MeshJsonData _meshJsonData;

        public void SetSlotData(StructType.MeshJsonData meshJsonData) {
            this._meshJsonData = meshJsonData;
        }

        public void SetSlotImage(Texture slotTex) { 
            
        }
    }
}