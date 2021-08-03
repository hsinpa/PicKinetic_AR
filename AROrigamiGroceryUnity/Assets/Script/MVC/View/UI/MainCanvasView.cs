using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PicKinetic.Controller
{
    public class MainCanvasView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform DebugPanel;

        [SerializeField]
        private CanvasGroup MainARGroup;

        private List<CanvasGroup> mainCanvasArray;

        private void Awake()
        {
            mainCanvasArray = new List<CanvasGroup>() { MainARGroup };
        }

        public void EnableDebugPanel(bool enable)
        {
            DebugPanel.gameObject.SetActive(enable);
        }

        public bool SetMainCanvasState(string canvasName, bool action, bool animation = false) {

            int canvasIndex = mainCanvasArray.FindIndex(x=>x.name == canvasName);

            if (canvasIndex < 0) return false;

            mainCanvasArray[canvasIndex].interactable = (action);
            mainCanvasArray[canvasIndex].blocksRaycasts = (action);

            float targetAlpha = (action) ? 1 : 0;

            if (animation) {
                mainCanvasArray[canvasIndex].DOFade(targetAlpha, 0.5f);

                return true;
            }

            mainCanvasArray[canvasIndex].alpha = targetAlpha;

            return true;
        }
    }
}