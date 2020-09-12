using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MarchingCube
{
    List<int> triangles = new List<int>();
    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();

    HashSet<Vector3> borders = new HashSet<Vector3>();

    MarchingCubeResult marchingCubeResult = new MarchingCubeResult();
    float radiusW, radiusH;

    public MarchingCube() {
    }


    public MarchingCubeResult Calculate(MeshGenerator.SquareGrid squareGrid)
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

        try
        {
            //mesh.Clear();

            //mesh.SetVertices(vertices);
            //mesh.SetTriangles(triangles, 0);
            //mesh.SetUVs(0, uv);
            marchingCubeResult.borderVertices = borders;
            marchingCubeResult.vertices = vertices.ToArray();
            marchingCubeResult.uv = uv.ToArray();
            marchingCubeResult.triangles = triangles.ToArray();
        }
        catch { 
        
        }
        //mesh.vertices = (vertices).ToArray();
        //mesh.triangles = (triangles).ToArray();
        //mesh.uv = (uv).ToArray();

        //mesh.SetVertices(vertices);
        //mesh.SetTriangles(triangles, 0);
        //mesh.SetUVs(0, uv);

        //if (mesh != null)
        //    mesh.RecalculateNormals();

        return marchingCubeResult;
    }

    private void TriangulateSquare(MeshGenerator.Square square) {
        switch (square.configuration) {
            // 1 points
            case 1:
                MeshFromPoints(square.centerBottom, square.bottomLeft, square.centerLeft);
                AddBorder(square.centerLeft.position);
                AddBorder(square.centerBottom.position);
                break;
            case 2:
                MeshFromPoints(square.centerRight, square.bottomRight, square.centerBottom);
                AddBorder(square.centerRight.position);
                AddBorder(square.centerBottom.position);

                break;
            case 4:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight);
                AddBorder(square.centerTop.position);
                AddBorder(square.centerRight.position);

                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                AddBorder(square.centerLeft.position);
                AddBorder(square.centerTop.position);

                break;
            // 2 points
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                AddBorder(square.centerLeft.position);
                AddBorder(square.centerRight.position);

                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                AddBorder(square.centerBottom.position);
                AddBorder(square.centerTop.position);

                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                AddBorder(square.centerTop.position);
                AddBorder(square.centerBottom.position);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                AddBorder(square.centerRight.position);
                AddBorder(square.centerLeft.position);

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
                AddBorder(square.centerLeft.position);
                AddBorder(square.centerTop.position);

                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                AddBorder(square.centerTop.position);
                AddBorder(square.centerRight.position);

                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                AddBorder(square.centerRight.position);
                AddBorder(square.centerBottom.position);
                
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                AddBorder(square.centerBottom.position);
                AddBorder(square.centerLeft.position);
                break;

            // 4 points
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;

        }
    }

    private void AddBorder(Vector3 vector)
    {
        if (!borders.Contains(vector))
            borders.Add(vector);
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
        borders.Clear();
    }

    private void CreateTriangle(MeshGenerator.Node a, MeshGenerator.Node b, MeshGenerator.Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

    }

    public struct MarchingCubeResult {
        public int[] triangles;
        public Vector3[] vertices;
        public Vector2[] uv;
        public HashSet<Vector3> borderVertices;
    }

}
