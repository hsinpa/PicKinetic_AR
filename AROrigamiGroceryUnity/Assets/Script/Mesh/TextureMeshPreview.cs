using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMeshPreview : MonoBehaviour
{
    [SerializeField]
    private Texture2D maskTexture;

    [SerializeField]
    private MeshFilter meshFiler;

    [SerializeField]
    private MeshRenderer meshRender;

    MeshGenerator meshGenerator;

    public void Start()
    {
        MarchingCube marchingCube = new MarchingCube();
        meshGenerator = GetComponent<MeshGenerator>();

        var colors = ReadPixel();

        meshGenerator.GenerateMesh(colors, maskTexture.width, maskTexture.height, 1);

        Mesh mesh = marchingCube.Calculate(meshGenerator.squareGrid);
        meshFiler.mesh = mesh;
    }

    private Color[] ReadPixel()
    {
        return maskTexture.GetPixels(0, 0, maskTexture.width, maskTexture.height);
    }
}
