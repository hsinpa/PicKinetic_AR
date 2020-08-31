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

            targetMesh.RecalculateNormals();

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
            int[] newTrigs = new int[newCount + (originCount * 4)];
            int verticeCount = 4;

            //Copy the front vert
            System.Array.Copy(triangle, newTrigs, originCount);

            for (int i = originCount; i < newCount; i++)
            {
                newTrigs[i] = newTrigs[i - originCount] + verticeCount;
            }



            Debug.Log(originCount);

            for (int i = 0; i < verticeCount; i ++)
            {
                int previousID = i - 1;
                if (i == 0) {
                    previousID = verticeCount - 1;
                }

                int baseIndex = i * 6;
                newTrigs[newCount + baseIndex] = previousID;
                newTrigs[newCount + baseIndex + 1] = previousID + verticeCount;
                newTrigs[newCount + baseIndex + 2] = i + verticeCount;

                newTrigs[newCount + baseIndex + 3] = i + verticeCount;
                newTrigs[newCount + baseIndex + 4] = i;
                newTrigs[newCount + baseIndex + 5] = previousID ;
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
