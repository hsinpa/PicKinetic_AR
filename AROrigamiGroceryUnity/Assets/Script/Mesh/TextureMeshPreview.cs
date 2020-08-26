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
    int startPixelX;
    int startPixelY;

    Camera _camera;
    Vector3 _meshPosition = new Vector2();

    public void Start()
    {
        previewMaskTexture = new Texture2D(resize, resize);
        imageMaskGeneator = new ImageMaskGeneator(resize);
        marchingCube = new MarchingCube();
        meshGenerator = new MeshGenerator();
        edgeImage = new EdgeImageDetector(EdgeMaterial);
        _camera = Camera.main;

        //var colors = ReadPixel();

        //AssignMesh(colors, maskTexture.width, maskTexture.height);

        //GenerateMesh(edgeImage.GetEdgeTex(rawColorTexture));

        //var point = _camera.ScreenToWorldPoint(new Vector3(400f, 700f, _camera.nearClipPlane));
        //Debug.Log("Cam Point " + point);
    }

    public void UpdateScreenInfo(int startPixelX, int startPixelY) {
        this.startPixelX = startPixelX;
        this.startPixelY = startPixelY;
    }

    public async void CaptureEdgeBorderMesh(Texture2D rawTexture) {
        var maskColors = await PrepareImageBorder(edgeImage.GetEdgeTex(rawTexture));

        if (!CheckIfValid(maskColors)) return;
        AssignMesh(maskColors.img, resize, resize, highlightTexture);
        AssignPosition(maskColors);
    }

    public async void CaptureContourMesh(Texture2D rawTexture) {
        var maskColors = await PrepareImageMask(edgeImage.GetEdgeTex(rawTexture));
        if (!CheckIfValid(maskColors)) return;

        AssignMesh(maskColors.img, resize, resize, rawTexture);
        AssignPosition(maskColors);
    }

    private async Task<MooreNeighborhood.MooreNeighborInfo> PrepareImageMask(Texture2D rawImage)
    {
        var scaledColor = rawImage.GetPixels(0, 0, resize, resize);

        return await imageMaskGeneator.AsyncCreateMask(scaledColor, resize, resize);
    }

    private async Task<MooreNeighborhood.MooreNeighborInfo> PrepareImageBorder(Texture2D rawImage)
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

    private void AssignPosition(MooreNeighborhood.MooreNeighborInfo meshInfo) {

        float x = (meshInfo.centerPoint.x * 4) + startPixelX;
        float y = (meshInfo.centerPoint.y * 4) + startPixelY;

        _meshPosition.Set(x, y, _camera.nearClipPlane);
        var point = _camera.ScreenToWorldPoint(_meshPosition);
        point.z = 0;

        meshRender.transform.position = point;
    }

    private bool CheckIfValid(MooreNeighborhood.MooreNeighborInfo meshInfo) {
        return (meshInfo.area > 40);
    }

    private Color[] ReadPixel()
    {
        return rawColorTexture.GetPixels(0, 0, rawColorTexture.width, rawColorTexture.height);
    }

}
