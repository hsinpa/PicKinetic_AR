using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MarchingCube
{

    Dictionary<int, MeshGenerator.Node[]> marchingCubeLookupTable = new Dictionary<int, MeshGenerator.Node[]>();
    List<int> triangles = new List<int>();
    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();

    float radiusW, radiusH;

    public MarchingCube() {

    }


    public Mesh Calculate(MeshGenerator.SquareGrid squareGrid)
    {
        Reset();

        var square = squareGrid.sqaures;
        radiusW = squareGrid.mapWidth / 2f;
        radiusH = squareGrid.mapHeight / 2f;

        for (int x = 0; x < square.GetLength(0); x++)
        {
            for (int y = 0; y < square.GetLength(1); y++)
            {
                TriangulateSquare(square[x,y]);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    private void TriangulateSquare(MeshGenerator.Square square) {
        switch (square.configuration) {
            // 1 points
            case 1:
                MeshFromPoints(square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 2:
                MeshFromPoints(square.centerRight, square.bottomRight, square.centerBottom);
                break;
            case 4:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;
            // 2 points
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 3 points
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 4 points
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;

        }
    }

    private void MeshFromPoints(params MeshGenerator.Node[] points) {
        AssignVertices(points);

        if (points.Length >= 3) 
            CreateTriangle(points[0], points[1], points[2]);

        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);

        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);

        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);

    }

    private void AssignVertices(MeshGenerator.Node[] points) { 
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1) {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);

                uv.Add(new Vector2( ((points[i].position.x / radiusW) + 1) * 0.5f, ((points[i].position.z / radiusH) + 1) * 0.5f));
            }
        }
    }

    private void Reset()
    {
        triangles.Clear();
        vertices.Clear();
        uv.Clear();
    }

    private void CreateTriangle(MeshGenerator.Node a, MeshGenerator.Node b, MeshGenerator.Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

    }

}
