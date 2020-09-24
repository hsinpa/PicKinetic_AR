using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCube
{

    private Mesh _mesh;
    public Mesh mesh => _mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;


    public MeshCube() {
        _mesh = new Mesh();
        vertices = new Vector3[4];
        triangles = new int[6];
        uv = new Vector2[4];
    }

    public Mesh CreateMesh(Vector3[] cornerPoints) {
        _mesh.Clear();
        vertices[0] = new Vector3(-0.5f, 0, -0.5f); // A
        vertices[1] = new Vector3(-0.5f, 0, 0.5f); // B
        vertices[2] = new Vector3(0.5f, 0, 0.5f); // C
        vertices[3] = new Vector3(0.5f, 0, -0.5f); // D

        //vertices[3] = new Vector3(0.5f, 0, 0); // A
        //vertices[4] = new Vector3(-0.5f, 1, 0); // B
        //vertices[5] = new Vector3(0.5f, 1, 0); // D

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 0;

        for (int i = 0; i < 4; i++)
        {
            uv[i] = new Vector2(vertices[i].x + 0.5f, vertices[i].z + 0.5f);
        }

        _mesh.SetVertices(vertices);
        _mesh.SetTriangles(triangles, 0);
        _mesh.SetUVs(0, uv);

        return _mesh;
    }

}
