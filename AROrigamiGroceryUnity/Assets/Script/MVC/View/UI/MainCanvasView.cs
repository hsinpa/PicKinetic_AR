using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PicKinetic.View
{
    public class MainCanvasView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform DebugPanel;

        [SerializeField]
        private ARMainUIView ARMainUIView;

        private List<MainViewInterface> mainCanvasArray;

        private void Awake()
        {
            mainCanvasArray = new List<MainViewInterface>() { ARMainUIView };
        }

        public void EnableDebugPanel(bool enable)
        {
            DebugPanel.gameObject.SetActive(enable);
        }

        public bool SetMainCanvasState<T>(bool action, bool animation = false) where T : MainViewInterface
        {

            int canvasIndex = mainCanvasArray.FindIndex(x=>x.GetType() == typeof(T));

            if (canvasIndex < 0) return false;

            mainCanvasArray[canvasIndex].CanvasGroup.interactable = (action);
            mainCanvasArray[canvasIndex].CanvasGroup.blocksRaycasts = (action);

            float targetAlpha = (action) ? 1 : 0;

            if (animation) {
                mainCanvasArray[canvasIndex].CanvasGroup.DOFade(targetAlpha, 0.5f);

                return true;
            }

            mainCanvasArray[canvasIndex].CanvasGroup.alpha = targetAlpha;

            return true;
        }
    }
}