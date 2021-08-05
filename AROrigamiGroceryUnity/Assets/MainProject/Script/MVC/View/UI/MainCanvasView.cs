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

        public T GetCanvasWithType<T>() where T : MainViewInterface
        {
            int canvasIndex = mainCanvasArray.FindIndex(x => x.GetType() == typeof(T));

            if (canvasIndex < 0) return default(T);

            return (T) mainCanvasArray[canvasIndex];
        }

        public T SetMainCanvasState<T>(bool action, bool animation = false) where T : MainViewInterface
        {
            MainViewInterface selectCanvas = GetCanvasWithType<T>();

            if (selectCanvas.CanvasGroup == null) return default(T);

            selectCanvas.CanvasGroup.interactable = (action);
            selectCanvas.CanvasGroup.blocksRaycasts = (action);

            float targetAlpha = (action) ? 1 : 0;

            if (animation) {
                selectCanvas.CanvasGroup.DOFade(targetAlpha, 0.5f);

                return (T)selectCanvas;
            }

            selectCanvas.CanvasGroup.alpha = targetAlpha;

            if (!action)
                selectCanvas.Dispose();

            return (T)selectCanvas;
        }
    }
}