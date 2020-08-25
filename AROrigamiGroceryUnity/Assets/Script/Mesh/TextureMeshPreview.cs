using AROrigami;
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
    private Texture2D highlightTexture;

    [SerializeField]
    private MeshFilter meshFiler;

    [SerializeField]
    private MeshRenderer meshRender;

    [SerializeField]
    private Material EdgeMaterial;

    ImageMaskGeneator imageMaskGeneator;
    MeshGenerator meshGenerator;
    MarchingCube marchingCube;

    EdgeImageDetector edgeImage;

    int resize = 64;

    public void Start()
    {
        previewMaskTexture = new Texture2D(resize, resize);
        imageMaskGeneator = new ImageMaskGeneator(resize);
        marchingCube = new MarchingCube();
        meshGenerator = new MeshGenerator();
        edgeImage = new EdgeImageDetector(EdgeMaterial);

        //var colors = ReadPixel();

        //AssignMesh(colors, maskTexture.width, maskTexture.height);

        //GenerateMesh(edgeImage.GetEdgeTex(rawColorTexture));
    }

    public async void CaptureEdgeBorderMesh(Texture2D rawTexture) {
        var maskColors = await PrepareImageBorder(edgeImage.GetEdgeTex(rawTexture));

        AssignMesh(maskColors, resize, resize, highlightTexture);
    }

    public async void CaptureContourMesh(Texture2D rawTexture) {
        var maskColors = await PrepareImageMask(edgeImage.GetEdgeTex(rawTexture));

        AssignMesh(maskColors, resize, resize, rawTexture);
    }

    private async Task<Color[]> PrepareImageMask(Texture2D rawImage)
    {
        var scaledColor = rawImage.GetPixels(0, 0, resize, resize);

        return await imageMaskGeneator.AsyncCreateMask(scaledColor, resize, resize);
    }

    private async Task<Color[]> PrepareImageBorder(Texture2D rawImage)
    {
        var scaledColor = rawImage.GetPixels(0, 0, resize, resize);

        return await imageMaskGeneator.AsyncCreateBorder(scaledColor, resize, resize);
    }

    private void AssignMesh(Color[] maskImage, int textureWidth, int textureHeight, Texture2D matTex)
    {
        previewMaskTexture.SetPixels(maskImage);
        previewMaskTexture.Apply();

        meshGenerator.GenerateMesh(maskImage, textureWidth, textureHeight, 1);
        Mesh mesh = marchingCube.Calculate(meshGenerator.squareGrid);
        meshFiler.mesh = mesh;

        meshRender.material.SetTexture("_MainTex", matTex);
    }

    private Color[] ReadPixel()
    {
        return rawColorTexture.GetPixels(0, 0, rawColorTexture.width, rawColorTexture.height);
    }

}
