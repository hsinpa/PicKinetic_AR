using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TextureMeshPreview : MonoBehaviour
{
    [SerializeField]
    private Texture2D rawColorTexture;

    [SerializeField]
    private Texture2D previewMaskTexture;

    [SerializeField]
    private MeshFilter meshFiler;

    [SerializeField]
    private MeshRenderer meshRender;

    ImageMaskGeneator imageMaskGeneator;
    MeshGenerator meshGenerator;
    MarchingCube marchingCube;

    int resize = 64;

    public void Start()
    {
        previewMaskTexture = new Texture2D(resize, resize);
        imageMaskGeneator = new ImageMaskGeneator(resize);
        marchingCube = new MarchingCube();
        meshGenerator = GetComponent<MeshGenerator>();

        //var colors = ReadPixel();

        //AssignMesh(colors, maskTexture.width, maskTexture.height);

        GenerateMesh();
    }

    private async void GenerateMesh() {
        var maskColors = await PrepareImageMask(rawColorTexture);

        previewMaskTexture.SetPixels(maskColors);
        previewMaskTexture.Apply();

        AssignMesh(maskColors, resize, resize);
    }

    private async Task<Color[]> PrepareImageMask(Texture2D rawImage) {
        var scaledImage = GPUTextureScale.scaled(rawImage, resize, resize);
        scaledImage.Apply();

        var scaledColor = scaledImage.GetPixels(0, 0, resize, resize);

        return await imageMaskGeneator.AsyncCreateMask(scaledColor, resize, resize);
    }

    private void AssignMesh(Color[] maskImage, int textureWidth, int textureHeight) {
        meshGenerator.GenerateMesh(maskImage, textureWidth, textureHeight, 1);
        Mesh mesh = marchingCube.Calculate(meshGenerator.squareGrid);
        meshFiler.mesh = mesh;
    }

    private Color[] ReadPixel()
    {
        return rawColorTexture.GetPixels(0, 0, rawColorTexture.width, rawColorTexture.height);
    }

}
