using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace PicKinetic.View {
    [RequireComponent(typeof(CanvasGroup))]
    public class ARMainUIView : MonoBehaviour, MainViewInterface
    {
        [SerializeField]
        private Button UIAlbumBtn;

        [SerializeField]
        private Button UIScanBtn;

        [SerializeField]
        private CanvasGroup _CanvasGroup;

        public CanvasGroup CanvasGroup => _CanvasGroup;

        public void SetScanBtnEvent(System.Action btnCallback) {
            UtilityMethod.SetSimpleBtnEvent(UIScanBtn, btnCallback);   
        }

        public void SetAlbumBtnEvent(System.Action btnCallback)
        {
            UtilityMethod.SetSimpleBtnEvent(UIAlbumBtn, btnCallback);
        }

        public void Dispose()
        {
        }
    }
}
