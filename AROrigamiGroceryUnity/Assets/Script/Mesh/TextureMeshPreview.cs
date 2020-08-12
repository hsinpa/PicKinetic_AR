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

        meshGenerator.GenerateMesh(colors, maskTexture.width, maskTexture.height, 1);
    }

    private Color[] ReadPixel()
    {
        return maskTexture.GetPixels(0, 0, maskTexture.width, maskTexture.height);
    }

    private void OnDrawGizmos()
    {
        if (meshGenerator != null && meshGenerator.squareGrid != null && meshGenerator.squareGrid.sqaures != null) {

            var square = meshGenerator.squareGrid.sqaures;
            for (int x = 0; x < square.GetLength(0); x++)
            {
                for (int y = 0; y < square.GetLength(1); y++)
                {
                    Gizmos.color = (square[x, y].topLeft.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(square[x, y].topLeft.position, Vector3.one * 0.4f);

                    Gizmos.color = (square[x, y].topRight.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(square[x, y].topRight.position, Vector3.one * 0.4f);

                    Gizmos.color = (square[x, y].bottomRight.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(square[x, y].bottomRight.position, Vector3.one * 0.4f);

                    Gizmos.color = (square[x, y].bottomLeft.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(square[x, y].bottomLeft.position, Vector3.one * 0.4f);

                    Gizmos.color = Color.gray;
                    Gizmos.DrawCube(square[x, y].centerTop.position, Vector3.one * 0.15f);
                    Gizmos.DrawCube(square[x, y].centerRight.position, Vector3.one * 0.15f);
                    Gizmos.DrawCube(square[x, y].centerLeft.position, Vector3.one * 0.15f);
                    Gizmos.DrawCube(square[x, y].centerBottom.position, Vector3.one * 0.15f);
                }
            }
        }   
    }
}
