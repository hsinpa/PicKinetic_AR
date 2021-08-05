using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PicKinetic
{
    public class EdgeImageDetector
    {
        private Material EdgeMaterial;

        private RenderTexture outputTexture;
        private RenderTexture tempTexA;
        private RenderTexture tempTexB;
        private RenderTexture dynamicSobel;

        int textureSize = 64;

        public EdgeImageDetector(Material EdgeMaterial)
        {
            this.EdgeMaterial = EdgeMaterial;
            outputTexture = TextureUtility.GetRenderTexture(textureSize);

            tempTexA = TextureUtility.GetRenderTexture(textureSize);
            tempTexB = TextureUtility.GetRenderTexture(textureSize);
            dynamicSobel = TextureUtility.GetRenderTexture(textureSize);

            this.EdgeMaterial.SetTexture("_BlendTex", dynamicSobel);
        }

        public RenderTexture GetEdgeTex(RenderTexture input)
        {
            //Blur
            Graphics.Blit(input, tempTexA, EdgeMaterial, 0);

            //Gray
            Graphics.Blit(tempTexA, tempTexB, EdgeMaterial, 1);

            //Sobel edge
            Graphics.Blit(tempTexB, tempTexA, EdgeMaterial, 2);

            //Sharp
            Graphics.Blit(tempTexA, tempTexB, EdgeMaterial, 3);

            ////Blend
            Graphics.Blit(tempTexB, tempTexA, EdgeMaterial, 4);

            Graphics.Blit(tempTexA, dynamicSobel);

            ////Dilation
            Graphics.Blit(tempTexA, outputTexture, EdgeMaterial, 6);

            return outputTexture;
        }

    }
}