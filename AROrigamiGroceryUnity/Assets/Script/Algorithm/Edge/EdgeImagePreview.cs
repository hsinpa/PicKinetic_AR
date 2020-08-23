using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AROrigami {

    public class EdgeImagePreview : MonoBehaviour
    {
        [SerializeField]
        private Material EdgeMaterial;

        [SerializeField]
        private Texture2D inputImage;

        [SerializeField]
        private Texture2D outputTex;

        // Start is called before the first frame update
        void Start()
        {
            EdgeImageDetector edgeImage = new EdgeImageDetector(EdgeMaterial);
            outputTex = edgeImage.GetEdgeTex(inputImage);
        }
    }
}