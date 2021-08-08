using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PicKinetic.View
{
    [RequireComponent(typeof(CanvasGroup))]

    public class LocalBookView : MonoBehaviour, MainViewInterface
    {
        [SerializeField]
        private CanvasGroup _CanvasGroup;

        public CanvasGroup CanvasGroup => _CanvasGroup;

        [SerializeField]
        private GameObject MeshTexturePrefab;

        [SerializeField]
        public void Dispose()
        {
        }
    }
}