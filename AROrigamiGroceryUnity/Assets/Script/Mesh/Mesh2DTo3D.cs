﻿using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AROrigami {
    public class Mesh2DTo3D
    {
        public Mesh Convert(Mesh targetMesh, Vector3[] borderVert) {

            var vertInfo = VerticesTo3D(targetMesh.vertices, borderVert);
            var triangles = TriangleTo3D(targetMesh.triangles, targetMesh.vertices.Length, vertInfo.borderStartCount, vertInfo.borderVertexCount);
            var uv = UVTo3D(targetMesh.uv, vertInfo.borderVertexCount);

            targetMesh.Clear();

            targetMesh.vertices = vertInfo.vectors;

            targetMesh.triangles = triangles;

            targetMesh.uv = uv;

            targetMesh.colors = vertInfo.colors;

            targetMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000);

            targetMesh.RecalculateNormals();

            return targetMesh;
        }

        private VertInfo VerticesTo3D(Vector3[] vertices, Vector3[] borderVertices) {
            VertInfo vertInfo = new VertInfo();
            int faceVertCount = vertices.Length;
            int borderCount = borderVertices.Length;

            int doubleFaceVertCount = faceVertCount * 2;
            int doubleBorderVertCount = borderCount * 2;

            Vector3[] newVertices = new Vector3[doubleFaceVertCount + doubleBorderVertCount];
            Color[] colors = new Color[doubleFaceVertCount + doubleBorderVertCount];
            Color texTypeTwo = new Color(1, 1, 1);

            //Copy the front vert
            System.Array.Copy(vertices, newVertices, faceVertCount);

            Vector3 dirDiff = new Vector3(0, -1, 0);
            for (int i = faceVertCount; i < doubleFaceVertCount; i++)
            {
                newVertices[i] = vertices[i - faceVertCount] + dirDiff;
            }

            //Create border vert
            for (int i = 0; i < borderCount; i++)
            {
                newVertices[doubleFaceVertCount + (i * 2)] = borderVertices[i];
                newVertices[doubleFaceVertCount + (i * 2) + 1] = borderVertices[i] + dirDiff;

                colors[doubleFaceVertCount + (i * 2)] = texTypeTwo;
                colors[doubleFaceVertCount + (i * 2) + 1] = texTypeTwo;
            }

            vertInfo.colors = colors;
            vertInfo.vectors = newVertices;
            vertInfo.borderVertexCount = borderCount;
            vertInfo.borderStartCount = doubleFaceVertCount;

            Debug.Log("faceVertCount " + faceVertCount);
            Debug.Log("doubleFaceVertCount " + doubleFaceVertCount);

            return vertInfo;
        }

        private int[] TriangleTo3D(int[] triangle, int vertexLength, int startBorderIndex, int numOfBorderVertex)
        {
            int originCount = triangle.Length;
            int newCount = originCount * 2;
            int trigCountPerVert = 6;

            int[] newTrigs = new int[newCount + (numOfBorderVertex * trigCountPerVert)];

            //Copy the front vert
            System.Array.Copy(triangle, newTrigs, originCount);

            for (int i = originCount; i < newCount; i++)
            {
                newTrigs[i] = newTrigs[i - originCount] + vertexLength;
            }

            //Create Wall
            for (int i = 0; i < numOfBorderVertex; i++)
            {
                int previousID = i - 1;
                if (i == 0)
                {
                    previousID = numOfBorderVertex - 1;
                }

                int baseIndex = i * 6;

                int A = (i * 2) + startBorderIndex;
                int B = (i * 2) + 1 + startBorderIndex;
                int C = (previousID * 2) + startBorderIndex;
                int D = (previousID * 2) + 1 + startBorderIndex;


                newTrigs[newCount + baseIndex] = C;
                newTrigs[newCount + baseIndex + 1] = D;
                newTrigs[newCount + baseIndex + 2] = B;

                newTrigs[newCount + baseIndex + 3] = B;
                newTrigs[newCount + baseIndex + 4] = A;
                newTrigs[newCount + baseIndex + 5] = C;
            }

            return newTrigs;
        }


        private Vector2[] UVTo3D(Vector2[] uvArray, int numOfBorderVertex)
        {
            int originCount = uvArray.Length;
            int newCount = originCount * 2;
            int doubleBorderCount = numOfBorderVertex * 2;
            Vector2[] newUV = new Vector2[newCount + doubleBorderCount];

            //Copy the front vert
            System.Array.Copy(uvArray, newUV, originCount);
            System.Array.Copy(uvArray, 0, newUV, originCount, originCount);

            //Assign border UV 
            int faceVert = 4;
            Vector2 BottomLeft = new Vector2(0, 0), BottomRight = new Vector2(0.3f, 0), TopLeft = new Vector2(0, 0.3f), TopRIght = new Vector2(0.3f, 0.3f);

            for (int i = 0; i < doubleBorderCount; i += faceVert)
            {
                int baseIndex = (newCount) + (i);

                newUV[baseIndex] = BottomLeft;
                newUV[baseIndex + 1] = BottomRight;
                newUV[baseIndex + 2] = TopLeft;
                newUV[baseIndex + 3] = TopRIght;
            }

            return newUV;
        }

        private struct VertInfo {
            public Vector3[] vectors;
            public Color[] colors;
            public int borderVertexCount;
            public int borderStartCount;
        }

    }
}
