using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AROrigami {
    public class Mesh2DTo3D
    {
        public Mesh Convert(Mesh targetMesh) {

            var vertices = VerticesTo3D(targetMesh.vertices);
            var triangles = TriangleTo3D(targetMesh.triangles);
            var uv = UVTo3D(targetMesh.uv);

            targetMesh.Clear();

            targetMesh.vertices = vertices;

            targetMesh.triangles = triangles;

            targetMesh.uv = uv;

            return targetMesh;
        }

        private Vector3[] VerticesTo3D(Vector3[] vertices) {
            int originCount = vertices.Length;
            int newCount = originCount * 2;
            Vector3[] newVertices = new Vector3[newCount];

            //Copy the front vert
            System.Array.Copy(vertices, newVertices, originCount);

            Vector3 dirDiff = new Vector3(0, 0, -0.1f);

            for (int i = originCount; i < newCount; i++)
            {
                newVertices[i] = vertices[i - originCount] + dirDiff;
            }

            return newVertices;
        }

        private int[] TriangleTo3D(int[] triangle)
        {
            int originCount = triangle.Length;
            int newCount = originCount * 2;
            int[] newTrigs = new int[newCount];

            //Copy the front vert
            System.Array.Copy(triangle, newTrigs, originCount);

            for (int i = originCount; i < newCount; i++)
            {
                newTrigs[i] = newTrigs[i - originCount] + originCount;
            }

            return newTrigs;
        }


        private Vector2[] UVTo3D(Vector2[] uvArray)
        {
            int originCount = uvArray.Length;
            int newCount = originCount * 2;
            Vector2[] newUV = new Vector2[newCount];

            //Copy the front vert
            System.Array.Copy(uvArray, newUV, originCount);
            System.Array.Copy(uvArray, 0, newUV, originCount, originCount);

            return newUV;
        }

    }
}
