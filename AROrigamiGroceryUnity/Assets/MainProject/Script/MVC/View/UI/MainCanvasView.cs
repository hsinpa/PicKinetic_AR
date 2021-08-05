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

        [Header("Main AR UI")]
        [SerializeField]
        private ARMainUIView ARMainUIView;

        [SerializeField]
        private ARInspectView ARInspectView;

        private List<MainViewInterface> mainCanvasArray;

        private void Awake()
        {
            mainCanvasArray = new List<MainViewInterface>() { ARMainUIView, ARInspectView };
        }

        public void EnableDebugPanel(bool enable)
        {
            DebugPanel.gameObject.SetActive(enable);
        }

        public T SetMainCanvasState<T>(bool action, bool animation = false) where T : MainViewInterface
        {
            int canvasIndex = mainCanvasArray.FindIndex(x=>x.GetType() == typeof(T));

            if (canvasIndex < 0) return default(T);

            mainCanvasArray[canvasIndex].CanvasGroup.interactable = (action);
            mainCanvasArray[canvasIndex].CanvasGroup.blocksRaycasts = (action);

            float targetAlpha = (action) ? 1 : 0;

            if (animation) {
                mainCanvasArray[canvasIndex].CanvasGroup.DOFade(targetAlpha, 0.5f);

                return (T) mainCanvasArray[canvasIndex];
            }

            mainCanvasArray[canvasIndex].CanvasGroup.alpha = targetAlpha;

            if (!action)
                mainCanvasArray[canvasIndex].Dispose();

            return (T) mainCanvasArray[canvasIndex];
        }
    }
}