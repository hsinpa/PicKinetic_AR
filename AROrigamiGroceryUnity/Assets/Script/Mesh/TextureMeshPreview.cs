using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMeshPreview : MonoBehaviour
{
    [SerializeField]
    private Texture2D maskTexture;

    MeshGenerator meshGenerator;

    public void Start()
    {
        meshGenerator = GetComponent<MeshGenerator>();

        var colors = ReadPixel();
        Debug.Log(colors.Length);
    }

    private Color[] ReadPixel()
    {
        return maskTexture.GetPixels(0, 0, maskTexture.width, maskTexture.height);
    }
}
