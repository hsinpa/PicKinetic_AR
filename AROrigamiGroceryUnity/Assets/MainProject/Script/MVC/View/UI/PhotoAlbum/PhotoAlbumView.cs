using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace PicKinetic.View
{

    [RequireComponent(typeof(CanvasGroup))]
    public class PhotoAlbumView : MonoBehaviour, MainViewInterface
    {
        [SerializeField]
        private CanvasGroup _CanvasGroup;

        public CanvasGroup CanvasGroup => _CanvasGroup;

        [SerializeField]
        private RectTransform Content;

        [SerializeField]
        private PhotoSlotView MeshTexturePrefab;

        public Button CloseButton;

        public void Dispose()
        {
            UtilityMethod.ClearChildObject(Content);
        }


        public void LoadAlbum(List<StructType.MeshJsonData> meshDataArray, System.Action<PhotoSlotView> OnSlotCreateEvent) {
            Dispose();

            int len = meshDataArray.Count;

            for (int i = 0; i < len; i++) {
                PhotoSlotView gSlotView = UtilityMethod.CreateObjectToParent<PhotoSlotView>(Content, MeshTexturePrefab);
                gSlotView.SetSlotData(meshDataArray[i]);

                if (OnSlotCreateEvent != null)
                    OnSlotCreateEvent(gSlotView);
            }
        }
    }
}