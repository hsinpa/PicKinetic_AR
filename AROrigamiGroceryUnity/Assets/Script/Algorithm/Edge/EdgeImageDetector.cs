using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AROrigami
{
    public class EdgeImageDetector
    {
        private Material EdgeMaterial;

        private RenderTexture outputTexture;
        private RenderTexture tempTexA;
        private RenderTexture tempTexB;

        private Texture2D outputTex;
        private Rect rectReadPicture;
        int textureSize = 64;

        // Start is called before the first frame update
        public EdgeImageDetector(Material EdgeMaterial)
        {
            this.EdgeMaterial = EdgeMaterial;
            outputTexture = TextureUtility.GetRenderTexture(textureSize);

            tempTexA = TextureUtility.GetRenderTexture(textureSize);
            tempTexB = TextureUtility.GetRenderTexture(textureSize);
            rectReadPicture = new Rect(0, 0, textureSize, textureSize);

            outputTex = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, true);
        }

        public Texture2D GetEdgeTex(Texture2D input)
        {
            //Blur
            Graphics.Blit(input, tempTexA, EdgeMaterial, 0);

            //Gray
            Graphics.Blit(tempTexA, tempTexB, EdgeMaterial, 1);

            //Sobel edge
            Graphics.Blit(tempTexB, tempTexA, EdgeMaterial, 2);

            //Dilatiion
            Graphics.Blit(tempTexA, tempTexB, EdgeMaterial, 3);

            Graphics.Blit(tempTexB, outputTexture, EdgeMaterial, 4);

            RenderTexture.active = outputTexture;
            // Read pixels
            outputTex.ReadPixels(rectReadPicture, 0, 0);
            outputTex.Apply();
            RenderTexture.active = null;

            return outputTex;
        }


    }
}