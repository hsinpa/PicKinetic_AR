using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteToMeshFilter : MonoBehaviour
{
    public Sprite targetSprite;
    public MeshFilter meshFilter;

    public void Start()
    {
        meshFilter.mesh = SpriteToMesh(targetSprite);
    }

    private Mesh SpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(Array.ConvertAll(sprite.vertices, i => (Vector3)i).ToList());
        mesh.SetUVs(0, sprite.uv.ToList());
        mesh.SetTriangles(Array.ConvertAll(sprite.triangles, i => (int)i), 0);

        return mesh;
    }

}
