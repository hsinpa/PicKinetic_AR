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
    private Material EdgeMaterial;

    ImageMaskGeneator imageMaskGeneator;
    MeshGenerator meshGenerator;
    MarchingCube marchingCube;

    EdgeImageDetector edgeImage;

    public System.Action<Texture2D> OnEdgeTexUpdate;

    private Vector3[] TestBorderArray;

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
    }

    public void UpdateScreenInfo(int startPixelX, int startPixelY) {
        this.startPixelX = startPixelX;
        this.startPixelY = startPixelY;
    }

    public async void CaptureEdgeBorderMesh(Texture2D rawTexture, MeshObject meshObject) {

        var edgeTex = edgeImage.GetEdgeTex(rawTexture);

        if (OnEdgeTexUpdate != null)
            OnEdgeTexUpdate(edgeTex);

        var maskColors = await PrepareImageBorder(edgeTex);

        if (!CheckIfValid(maskColors)) return;
        AssignMesh(maskColors.img, resize, resize, highlightTexture, meshObject);
        AssignPosition(maskColors, meshObject);
    }

    public async void CaptureContourMesh(Texture2D rawTexture, MeshObject meshObject) {
        var edgeTex = edgeImage.GetEdgeTex(rawTexture);

        previewMaskTexture.SetPixels(edgeTex.GetPixels());
        previewMaskTexture.Apply();

        var maskColors = await PrepareImageMask(edgeTex);
        if (!CheckIfValid(maskColors)) return;

        AssignMesh(maskColors.img, resize, resize, rawTexture, meshObject);
        AssignPosition(maskColors, meshObject);
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

    private void AssignMesh(Color[] maskImage, int textureWidth, int textureHeight, Texture2D matTex, MeshObject meshObject)
    {
        meshGenerator.GenerateMesh(maskImage, textureWidth, textureHeight, 1);
        Mesh mesh = marchingCube.Calculate(meshGenerator.squareGrid, meshObject.mesh);

        Debug.Log(mesh.normals.Length);
        TestBorderArray = mesh.normals;

        if (mesh != null)
            meshObject.SetMesh(mesh, matTex, matTex.width);
    }

    private void AssignPosition(MooreNeighborhood.MooreNeighborInfo meshInfo, MeshObject meshObject) {

        float x = (meshInfo.centerPoint.x * 4) + startPixelX;
        float y = (meshInfo.centerPoint.y * 4) + startPixelY;

        _meshPosition.Set(x, y, _camera.nearClipPlane);
        var point = _camera.ScreenToWorldPoint(_meshPosition);
        point.z = 0;

        if (meshObject != null)
            meshObject.transform.position = point;
    }

    private bool CheckIfValid(MooreNeighborhood.MooreNeighborInfo meshInfo) {
        return (meshInfo.area > 40);
    }

    private Color[] ReadPixel()
    {
        return rawColorTexture.GetPixels(0, 0, rawColorTexture.width, rawColorTexture.height);
    }


    private void OnDrawGizmosSelected()
    {
        if (TestBorderArray != null) {
            int count = TestBorderArray.Length;
            for (int i = 0; i < count; i++) {

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(TestBorderArray[i], 0.01f);
            }
        }
    }
}
