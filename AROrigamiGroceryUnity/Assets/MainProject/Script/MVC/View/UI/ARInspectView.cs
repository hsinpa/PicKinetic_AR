using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using DG.Tweening;

namespace PicKinetic.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ARInspectView : MonoBehaviour, MainViewInterface
    {
        [SerializeField]
        private Text HintText;

        [SerializeField]
        private Button UISaveBtn;

        [SerializeField]
        private CanvasGroup _CanvasGroup;

        public CanvasGroup CanvasGroup => _CanvasGroup;

        private MeshObject meshObject;

        public void SetARInsector(MeshObject meshObject, System.Action btnCallback)
        {
            this.meshObject = meshObject;
            UtilityMethod.SetSimpleBtnEvent(UISaveBtn, btnCallback);
        }

        public void SetActionBtnStyle(Color btnColor, string btnText) {
            UISaveBtn.image.color = btnColor;
            UISaveBtn.GetComponentInChildren<Text>().text = btnText;
        }

        public void PlayHintAnimation(bool play)
        {
            HintText.gameObject.SetActive(play);

            if (play)
                HintText.DOFade(0, 2f).SetLoops(-1, LoopType.Yoyo);
        }

        public void Dispose() {
            meshObject = null;
            HintText.DOKill();
        }
    }
}