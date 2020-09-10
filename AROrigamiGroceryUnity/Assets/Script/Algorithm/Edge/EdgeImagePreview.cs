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

        [SerializeField]
        private Rect sourceRect;

        private MeshRenderer meshRenderer;
        private EdgeImageDetector edgeImage;

        // Start is called before the first frame update
        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();

            edgeImage = new EdgeImageDetector(EdgeMaterial);

            meshRenderer.material.SetTexture("_MainTex", outputTex);
        }

        private void Update()
        {
            outputTex = edgeImage.GetEdgeTex(inputImage);

            //TextureStructure t_structure = GrabTextureRadius(inputImage, 0.7f);
            //outputTex = new Texture2D(t_structure.width, t_structure.height);
            //outputTex.SetPixels(t_structure.colors);
            outputTex.Apply();
            meshRenderer.material.SetTexture("_MainTex", outputTex);
        }

        private TextureStructure GrabTextureRadius(Texture2D p_texture, float ratio) {
            TextureStructure textureStructure = new TextureStructure();
            int width = p_texture.width;
            int height = p_texture.height;

            int criteria = (width > height) ? width : height;
            int radius = (int)((criteria * ratio) / 2f);
            int size = radius * 2;
            Vector2Int center = new Vector2Int((int)(width / 2f), (int)(height / 2f));

            textureStructure.width = size;
            textureStructure.height = size;
            textureStructure.colors = p_texture.GetPixels(center.x - radius, center.y - radius, size, size);

            return textureStructure;
        }

        private struct TextureStructure {
            public Color[] colors;
            public int width;
            public int height;
        }

    }
}